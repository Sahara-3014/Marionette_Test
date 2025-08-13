using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class IntroSequenceController : MonoBehaviour
{
    public enum CommandType
    {
        TYPE_TEXT_CHAR, 
        SPAM_TEXT_LINES,       
        WAIT,                   
        PLAY_SOUND,           
        PLAY_DIRECTION_SET 
    }

    [System.Serializable]
    public class SequenceCommand
    {
        [Tooltip("이 단계에서 실행할 명령의 종류를 선택")]
        public CommandType commandType;

        [Header("Text Settings")]
        [Tooltip("타이핑할 내용.")]
        [TextArea(3, 10)]
        public string textContent = "";

        [Tooltip("TYPE_TEXT_CHAR: 초당 글자 수.")]
        public float charTypingSpeed = 50f;

        [Tooltip("SPAM_TEXT_LINES: 초당 줄 수.")]
        public float lineSpamSpeed = 20f;

        [Header("Wait Settings")]
        [Tooltip("WAIT: 대기할 시간(초)")]
        public float waitDuration = 1.0f;

        [Header("Sound Settings")]
        [Tooltip("PLAY_SOUND: 재생할 오디오 클립")]
        public AudioClip audioClip;

        [Header("Direction Set Settings")]
        [Tooltip("PLAY_DIRECTION_SET: EffectManager로 재생할 DirectionSetSO 파일")]
        public DirectionSetSO directionSetToPlay; 
    }

    [Header("필수 연결 대상")]
    [Tooltip("텍스트를 표시할 TextMeshPro UI 컴포넌트")]
    public TextMeshProUGUI textComponent;
    [Tooltip("스크롤을 제어할 Scroll Rect 컴포넌트")]
    public ScrollRect scrollRect;

    [Header("시퀀스 명령 목록 (여기에 연출을 설계하세요)")]
    public List<SequenceCommand> sequence;

    [Header("기본 타이핑 사운드 (선택 사항)")]
    public AudioClip defaultTypingSound;

    private AudioSource audioSource;
    private Coroutine runningSequence;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (textComponent == null || scrollRect == null)
        {
            Debug.LogError("필수 컴포넌트(Text Component 또는 Scroll Rect)가 연결되지 않았습니다!");
            this.enabled = false;
            return;
        }
        if (runningSequence != null)
        {
            StopCoroutine(runningSequence);
        }
        runningSequence = StartCoroutine(RunSequence());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        runningSequence = null;
    }

    private IEnumerator RunSequence()
    {
        textComponent.text = ""; 

        foreach (var command in sequence)
        {
            if (EffectManager.Instance == null && command.commandType == CommandType.PLAY_DIRECTION_SET)
            {
                Debug.LogError("EffectManager 인스턴스를 찾을 수 없어 PLAY_DIRECTION_SET 명령을 실행할 수 없습니다.");
                continue; // 다음 명령으로 넘어감
            }

            switch (command.commandType)
            {
                case CommandType.TYPE_TEXT_CHAR:
                    yield return StartCoroutine(TypeTextChar(command));
                    break;
                case CommandType.SPAM_TEXT_LINES:
                    yield return StartCoroutine(SpamTextByLine(command));
                    break;
                case CommandType.WAIT:
                    yield return new WaitForSeconds(command.waitDuration);
                    break;
                case CommandType.PLAY_SOUND:
                    if (command.audioClip != null)
                        audioSource.PlayOneShot(command.audioClip);
                    break;
                case CommandType.PLAY_DIRECTION_SET:
                    if (command.directionSetToPlay != null)
                        EffectManager.Instance.PlayDirectionSetBySO(command.directionSetToPlay);
                    break;
            }
        }
    }

    private IEnumerator TypeTextChar(SequenceCommand command)
    {
        if (textComponent.text.Length > 0 && !textComponent.text.EndsWith("\n\n"))
        {
            textComponent.text += "\n";
        }

        float delay = 1f / Mathf.Max(0.1f, command.charTypingSpeed);
        foreach (char c in command.textContent)
        {
            if (c != '\n' && defaultTypingSound != null)
            {
                audioSource.PlayOneShot(defaultTypingSound, 0.6f);
            }

            textComponent.text += c;
            ForceScrollToBottom();
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator SpamTextByLine(SequenceCommand command)
    {
        if (textComponent.text.Length > 0 && !textComponent.text.EndsWith("\n\n"))
        {
            textComponent.text += "\n";
        }

        string[] lines = command.textContent.Split('\n');
        float delay = 1f / Mathf.Max(0.1f, command.lineSpamSpeed);

        foreach (string line in lines)
        {
            if (defaultTypingSound != null)
            {
                audioSource.PlayOneShot(defaultTypingSound, 0.6f);
            }

            textComponent.text += line + "\n"; 
            ForceScrollToBottom();
            yield return new WaitForSeconds(delay);
        }
    }

    private void ForceScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}