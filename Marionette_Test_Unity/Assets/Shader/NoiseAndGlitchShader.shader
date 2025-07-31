// Unlit shader for creating a combination of noise, scanlines, and glitch effects.
// 노이즈, 스캔라인, 글리치 효과를 조합하기 위한 언릿 셰이더입니다.
Shader "Custom/NoiseAndGlitchShader"
{
    // 인스펙터에서 조절할 수 있는 프로퍼티들
    Properties
    {
        [Header(Noise Settings)]
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.5    // 노이즈 강도
        _NoiseSpeed ("Noise Speed", Range(0, 100)) = 50          // 노이즈 변화 속도

        [Header(Scanline Settings)]
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.1 // 스캔라인 강도
        _ScanlineFrequency ("Scanline Frequency", Range(1, 2000)) = 800 // 스캔라인 갯수

        [Header(Glitch Settings)]
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.1 // 글리치 강도 (화면 찢어짐)
        _GlitchBlockIntensity ("Glitch Block Intensity", Range(0, 1)) = 0.1 // 글리치 블록 강도
        _GlitchBlockColor ("Glitch Block Color", Color) = (1, 0, 1, 1) // 글리치 블록 색상 (보라색 기본)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            // C#에서 제어할 수 있도록 변수 선언
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            // 프로퍼티 변수 선언
            sampler2D _MainTex;
            float _NoiseIntensity;
            float _NoiseSpeed;
            float _ScanlineIntensity;
            float _ScanlineFrequency;
            float _GlitchIntensity;
            float _GlitchBlockIntensity;
            float4 _GlitchBlockColor;


            // 간단한 2D 랜덤 함수
            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 화면 텍스쳐 (Blit으로 전달받음)
                half4 color = tex2D(_MainTex, IN.uv);

                // 1. 노이즈 효과
                float noise = (random(IN.uv + _Time.y * _NoiseSpeed) - 0.5) * _NoiseIntensity;
                color.rgb += noise;

                // 2. 스캔라인 효과
                float scanline = sin(IN.uv.y * _ScanlineFrequency) * _ScanlineIntensity;
                color.rgb -= scanline;

                // 3. 글리치 효과 (화면 찢어짐)
                float glitch = random(float2(_Time.y, IN.uv.y)) * _GlitchIntensity;
                if (glitch > 0.95) // 낮은 확률로 화면을 크게 찢음
                {
                    float offset = random(float2(IN.uv.y, _Time.y * 10.0)) * 0.2 - 0.1;
                    color.rgb = tex2D(_MainTex, float2(IN.uv.x + offset, IN.uv.y)).rgb;
                }

                // 4. 글리치 블록 효과 (색상 있는 네모난 노이즈)
                float blockNoise = random(floor(IN.uv * float2(50, 200)) + _Time.y);
                if (blockNoise > (1.0 - _GlitchBlockIntensity))
                {
                    float isBlock = step(0.99, blockNoise); // 블록을 드문드문 표시
                    color.rgb = lerp(color.rgb, _GlitchBlockColor.rgb, isBlock * _GlitchBlockColor.a);
                }

                // 5. 비네트 효과 (화면 가장자리 어둡게)
                float vignette = length(IN.uv - 0.5);
                color.rgb *= 1.0 - (vignette * 0.5);

                return color;
            }
            ENDHLSL
        }
    }
}