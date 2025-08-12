// Scripts/Dialog/DialogEnums.cs


public enum Dialog_CharPos
{
    Left = 0,               // 왼쪽 위치
    Center = 1,             // 중앙 위치
    Right = 2,              // 오른쪽 위치

    New = -1,               // 새로 등장하는 캐릭터 (기존 위치 무시)
    None = -2,              // 위치 지정하지 않음
    Clear = -3              // 해당 위치의 캐릭터 제거
}

public enum Dialog_ScreenEffect
{
    None = 1,
    ShakeVertical = 2,
    ShakeHorizontal = 3,
    Shake = 4,
    ClearAll = 5,
    FadeOutAll = 6,
    OtherColorEnable = 7,
    OtherColorDisable = 8,
    AllColorEnable = 9,
    AllColorDisable = 10,
    MoveUp = 11,
    MoveDown = 12,
    FadeIn = 13,
    FadeOut = 14,
    ColorEnable = 15,
    ColorDisable = 16,
}

public enum Dialog_CharEffect
{
    None = 1,
    ShakeVertical = 2,
    ShakeHorizontal = 3,
    Shake = 4,
    Jump = 5,
    MoveOut2Left = 6,
    MoveOut2Right = 7,
    MoveLeft2Out = 8,
    MoveRight2Out = 9,
    MoveVertical = 10,
    MoveUp = 11,
    MoveDown = 12,
    FadeIn = 13,
    FadeOut = 14,
    ColorEnable = 15,
    ColorDisable = 16,
}




//public enum Dialog_ScreenEffect
//{
//    ShakeVertical,          // 화면을 위아래로 흔들기
//    ShakeHorizontal,        // 화면을 좌우로 흔들기
//    Shake,                  // 화면을 랜덤 방향으로 흔들기
//    None,                   // 아무 효과 없음
//    ClearAll,               // 모든 화면 요소 제거
//    FadeOutAll,             // 전체 화면을 서서히 사라지게 하기
//    OtherColorEnable,       // 배경색 변경 효과 (다른 색상 적용)
//    OtherColorDisable,      // 원래 배경색으로 복구
//    AllColorEnable,         // 모든 캐릭터 색상 변화 적용 (ex. 어두워지기)
//    AllColorDisable         // 모든 캐릭터 색상 원상복구
//}

//public enum Dialog_CharEffect
//{
//    Jump,                   // 캐릭터가 점프 (짧게 위로 튐)
//    ShakeVertical,          // 캐릭터를 위아래로 흔듦
//    ShakeHorizontal,        // 캐릭터를 좌우로 흔듦
//    Shake,                  // 캐릭터를 랜덤 방향으로 흔듦

//    MoveOut2Left,           // 화면 밖으로 왼쪽으로 나감
//    MoveOut2Right,          // 화면 밖으로 오른쪽으로 나감
//    MoveLeft2Out,           // 왼쪽에서 나가게 함
//    MoveRight2Out,          // 오른쪽에서 나가게 함

//    MoveVertical,           // 위 또는 아래로 움직임
//    MoveUp,                 // 위로 이동
//    MoveDown,               // 아래로 이동

//    FadeIn,                 // 서서히 나타남
//    FadeOut,                // 서서히 사라짐

//    ColorEnable,            // 색상 강조 (밝히기)
//    ColorDisable,           // 색상 비활성화 (어둡게 하기)

//    None                    // 효과 없음
//}


