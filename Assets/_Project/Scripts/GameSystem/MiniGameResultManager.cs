using UnityEngine;

namespace PerfectButler.GameSystem
{
    /// <summary>
    /// 미니게임 완료 후 결과를 저장하고 전달하는 정적 클래스
    /// </summary>
    public static class MiniGameResultManager
    {
        // 마지막 미니게임 결과
        public static MiniGameResult LastResult { get; private set; } = MiniGameResult.Normal;
        
        // 결과 저장 (미니게임 씬에서 호출)
        public static void SaveResult(MiniGameResult result)
        {
            LastResult = result;
            Debug.Log($"미니게임 결과 저장: {result}");
        }
        
        // 결과 초기화 (메인 씬에서 결과 사용 후 호출)
        public static void ClearResult()
        {
            LastResult = MiniGameResult.Normal;
        }
    }
    
    /// <summary>
    /// 미니게임 타입 정의
    /// </summary>
    public enum MiniGameType
    {
        FishingRod,   // 낚싯대 흔들기
        WhackAMole    // 두더지잡기
    }
}