using UnityEngine;

namespace PerfectButler.GameSystem
{
    [System.Serializable]
    public class LevelData
    {
        public static readonly string[] LevelNames = 
        {
            "왕초보 집사",
            "초보 집사", 
            "괜찮은 집사",
            "숙련된 집사",
            "완벽한 집사"
        };
        
        public const int MAX_LEVEL = 4; // 0-4 레벨 (5단계)
        public const float EXP_PER_LEVEL = 100f;
        public const float EXP_GAIN_PER_SECOND = 0.3f; // 사용 안함 (행동 기반으로 변경)
        public const float STAT_THRESHOLD_FOR_EXP = 80f; // 사용 안함
    }
    
    // 행동별 경험치 정의
    public static class ActionExpReward
    {
        public const float FEED_CAT = 5f;        // 밥주기
        public const float CLEAN_DUST = 3f;      // 먼지 치우기  
        public const float PLAY_MINIGAME = 10f;  // 미니게임 (결과에 따라 차등)
        public const float HOSPITAL_VISIT = 15f; // 병원 보내기 (큰 행동이라 많이)
    }
    
    // 행동별 쿨타임 정의
    public static class ActionCooltime
    {
        public const float FEED_CAT = 60f;       // 밥주기: 1분
        public const float CLEAN_DUST = 0f;      // 청소: 0초
        public const float PLAY_MINIGAME = 60f;  // 놀아주기: 1분 (미니게임이라 조금 길게)
        public const float HOSPITAL_VISIT = 180f; // 병원: 3분 (큰 행동이니 길게)
    }
    
    // 미니게임 결과별 차등 경험치
    public enum MiniGameResult
    {
        Fail = 5,     // 실패해도 경험치는 줌
        Normal = 10,  // 보통
        Perfect = 20  // 완벽
    }
}