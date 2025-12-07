using UnityEngine;
using PerfectButler.GameSystem; // 회원님이 만드신 코드 사용!

public class GameLevelManager : MonoBehaviour
{
    [Header("연결 정보")]
    public RoomDecoManager decoManager; // 방금 만든 가구 매니저 연결

    [Header("현재 상태 확인용")]
    public int currentLevel = 0;
    public float currentExp = 0;

    void Start()
    {
        // 게임 시작 시 현재 레벨에 맞춰 가구 세팅
        UpdateLevelState();
    }

    // 경험치 획득 함수 (밥주기, 청소하기 등에서 이 함수를 부르면 됨)
    public void AddExp(float amount)
    {
        // 만약 이미 만렙이면 경험치 안 오름
        if (currentLevel >= LevelData.MAX_LEVEL) return;

        currentExp += amount;
        Debug.Log($"경험치 획득! 현재: {currentExp}/{LevelData.EXP_PER_LEVEL}");

        // 경험치가 100(설정값) 넘으면 레벨업
        if (currentExp >= LevelData.EXP_PER_LEVEL)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        // 경험치 남은 거 이월 (예: 110점이면 레벨업 후 10점부터 시작)
        currentExp -= LevelData.EXP_PER_LEVEL;
        
        currentLevel++;
        
        // 최대 레벨 제한
        if (currentLevel > LevelData.MAX_LEVEL)
            currentLevel = LevelData.MAX_LEVEL;

        Debug.Log($"🎉 레벨 업! 현재: {LevelData.LevelNames[currentLevel]}");

        // ★ 여기서 가구 매니저에게 신호를 보냅니다!
        UpdateLevelState();
    }

    void UpdateLevelState()
    {
        if (decoManager != null)
        {
            decoManager.SetFurnitureVisibility(currentLevel);
        }
    }
    
    // 테스트용: 키보드 'Space' 누르면 밥주기 경험치 획득
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddExp(ActionExpReward.FEED_CAT); // 회원님이 정의한 밥주기 경험치(5f) 사용
        }
    }
}