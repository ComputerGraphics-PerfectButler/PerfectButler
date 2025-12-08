using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PerfectButler.GameSystem;

/// <summary>
/// 고양이의 상호작용 및 자동 패턴 재생을 관리하는 클래스
/// </summary>
public class CatInteractionManager : MonoBehaviour
{
    public static CatInteractionManager Instance { get; private set; }

    [Header("References")]
    public PatternPlayer patternPlayer;
    public PatternDataManager patternDataManager;
    public RoomDecoManager roomDecoManager;

    [Header("Pattern Settings")]
    public float patternChangeInterval = 10f; // 패턴 변경 간격(초)
    
    [Header("Interaction Settings")]
    public float interactionRange = 2f; // 상호작용 가능 범위
    public Transform playerTransform; // 플레이어 Transform

    // 패턴 리스트
    private List<MovementPattern> allPatterns;
    private List<MovementPattern> availablePatterns; // 현재 사용 가능한 패턴들
    
    // 가구 개수에 따른 패턴 인덱스 (0-based)
    private readonly int[] basePatternIndices = { 0, 1, 3, 6, 9 }; // 1, 2, 4, 7, 10번 패턴
    private readonly int[] twoFurniturePatternIndices = { 4, 5, 7 }; // 5, 6, 8번 패턴
    private readonly int[] fourFurniturePatternIndices = { 8 }; // 9번 패턴
    private const int feedingPatternIndex = 2; // 3번 패턴 (밥주기 전용)

    private int activeFurnitureCount = 0;
    private bool isPatternPlaying = false;
    private bool isWaitingForNextPattern = false;

    // 상호작용 가능 여부
    public bool IsPlayerInRange { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 컴포넌트 자동 찾기
        if (patternPlayer == null)
            patternPlayer = GetComponent<PatternPlayer>();
        
        if (patternDataManager == null)
            patternDataManager = FindObjectOfType<PatternDataManager>();

        // 플레이어 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        // 패턴 로드
        LoadPatterns();

        // 가구 개수 확인 및 패턴 시작
        UpdateActiveFurnitureCount();
        StartRandomPatternLoop();
    }

    void Update()
    {
        // 플레이어와의 거리 체크
        CheckPlayerDistance();
    }

    /// <summary>
    /// 플레이어와의 거리를 체크하여 상호작용 가능 여부 판단
    /// </summary>
    void CheckPlayerDistance()
    {
        if (playerTransform == null)
        {
            IsPlayerInRange = false;
            return;
        }

        Vector3 catPos = transform.position;
        Vector3 playerPos = playerTransform.position;

        // Y축 제외하고 XZ 평면에서의 거리 계산
        float distance = Vector2.Distance(
            new Vector2(catPos.x, catPos.z),
            new Vector2(playerPos.x, playerPos.z)
        );

        IsPlayerInRange = distance <= interactionRange;
    }

    /// <summary>
    /// 패턴 데이터 로드
    /// </summary>
    void LoadPatterns()
    {
        if (patternDataManager == null)
        {
            Debug.LogError("PatternDataManager를 찾을 수 없습니다!");
            return;
        }

        allPatterns = patternDataManager.LoadAndGetPatterns();

        if (allPatterns == null || allPatterns.Count == 0)
        {
            Debug.LogError("로드된 패턴이 없습니다!");
            return;
        }

        Debug.Log($"총 {allPatterns.Count}개의 패턴을 로드했습니다.");
        UpdateAvailablePatterns();
    }

    /// <summary>
    /// 활성화된 가구 개수 업데이트
    /// </summary>
    public void UpdateActiveFurnitureCount()
    {
        if (roomDecoManager == null || roomDecoManager.furnitureList == null)
        {
            activeFurnitureCount = 0;
            return;
        }

        int count = 0;
        foreach (GameObject furniture in roomDecoManager.furnitureList)
        {
            if (furniture != null && furniture.activeSelf)
            {
                count++;
            }
        }

        if (activeFurnitureCount != count)
        {
            activeFurnitureCount = count;
            Debug.Log($"활성화된 가구 개수: {activeFurnitureCount}");
            UpdateAvailablePatterns();
        }
    }

    /// <summary>
    /// 가구 개수에 따라 사용 가능한 패턴 업데이트
    /// </summary>
    void UpdateAvailablePatterns()
    {
        if (allPatterns == null || allPatterns.Count == 0)
            return;

        availablePatterns = new List<MovementPattern>();

        // 기본 패턴 추가 (1, 2, 4, 7, 10번)
        foreach (int index in basePatternIndices)
        {
            if (index < allPatterns.Count)
            {
                availablePatterns.Add(allPatterns[index]);
            }
        }

        // 가구 2개 이상: 5, 6, 8번 패턴 추가
        if (activeFurnitureCount >= 2)
        {
            foreach (int index in twoFurniturePatternIndices)
            {
                if (index < allPatterns.Count)
                {
                    availablePatterns.Add(allPatterns[index]);
                }
            }
        }

        // 가구 4개 이상: 9번 패턴 추가
        if (activeFurnitureCount >= 4)
        {
            foreach (int index in fourFurniturePatternIndices)
            {
                if (index < allPatterns.Count)
                {
                    availablePatterns.Add(allPatterns[index]);
                }
            }
        }

        Debug.Log($"사용 가능한 패턴 개수: {availablePatterns.Count}");
    }

    /// <summary>
    /// 랜덤 패턴 무한 반복 시작
    /// </summary>
    void StartRandomPatternLoop()
    {
        if (!isWaitingForNextPattern)
        {
            StartCoroutine(RandomPatternLoopCoroutine());
        }
    }

    IEnumerator RandomPatternLoopCoroutine()
    {
        isWaitingForNextPattern = true;

        while (true)
        {
            // 사용 가능한 패턴이 있는지 확인
            if (availablePatterns == null || availablePatterns.Count == 0)
            {
                Debug.LogWarning("사용 가능한 패턴이 없습니다!");
                yield return new WaitForSeconds(patternChangeInterval);
                continue;
            }

            // 랜덤 패턴 선택 및 재생
            MovementPattern randomPattern = availablePatterns[Random.Range(0, availablePatterns.Count)];
            PlayPattern(randomPattern);

            // 패턴이 끝날 때까지 대기
            while (isPatternPlaying)
            {
                yield return null;
            }

            // 다음 패턴까지 대기
            yield return new WaitForSeconds(patternChangeInterval);

            // 가구 개수 업데이트 체크
            UpdateActiveFurnitureCount();
        }
    }

    /// <summary>
    /// 특정 패턴 재생
    /// </summary>
    public void PlayPattern(MovementPattern pattern)
    {
        if (pattern == null || patternPlayer == null)
            return;

        isPatternPlaying = true;
        patternPlayer.PlayPattern(pattern);
        StartCoroutine(WaitForPatternEnd());
    }

    IEnumerator WaitForPatternEnd()
    {
        // PatternPlayer의 재생이 끝날 때까지 대기
        while (patternPlayer.IsPlaying)
        {
            yield return null;
        }

        isPatternPlaying = false;
    }

    /// <summary>
    /// 밥주기 액션 - 3번 패턴 재생
    /// </summary>
    public void OnFeedAction()
    {
        if (allPatterns == null || feedingPatternIndex >= allPatterns.Count)
        {
            Debug.LogWarning("밥주기 패턴을 찾을 수 없습니다!");
            return;
        }

        // CatStats에 밥주기 처리
        if (CatStats.Instance != null)
        {
            bool success = CatStats.Instance.TryPerformAction(
                StatType.Hunger, 
                25f, 
                ActionExpReward.FEED_CAT, 
                "밥주기"
            );

            if (success)
            {
                // 현재 진행 중인 패턴 중지
                StopCurrentPattern();

                // 3번 패턴 재생
                MovementPattern feedingPattern = allPatterns[feedingPatternIndex];
                PlayPattern(feedingPattern);

                Debug.Log("밥주기 완료! 3번 패턴 재생");
            }
        }
    }

    /// <summary>
    /// 놀아주기 액션 - 미니게임 연결
    /// </summary>
    public void OnPlayAction()
    {
        if (CatStats.Instance != null)
        {
            // 쿨타임 체크만 수행 (스탯과 경험치는 미니게임 완료 후 지급)
            if (!CatStats.Instance.CanPerformAction(StatType.Fun))
            {
                float remaining = CatStats.Instance.GetRemainingCooltime(StatType.Fun);
                Debug.Log($"놀아주기 쿨타임 중... {remaining:F1}초 남음");
                return;
            }

            Debug.Log("놀아주기 시작! 미니게임 선택 팝업 표시");

            // 미니게임 선택 팝업 찾기 및 표시
            PerfectButler.UI.MiniGameSelectionPopup popup = FindObjectOfType<PerfectButler.UI.MiniGameSelectionPopup>();
            if (popup != null)
            {
                popup.ShowPopup();
            }
            else
            {
                Debug.LogWarning("MiniGameSelectionPopup을 찾을 수 없습니다!");
            }
        }
    }

    /// <summary>
    /// 병원 보내기 액션
    /// </summary>
    public void OnHospitalAction()
    {
        if (CatStats.Instance != null)
        {
            bool success = CatStats.Instance.TryPerformAction(
                StatType.Health, 
                100f, 
                ActionExpReward.HOSPITAL_VISIT, 
                "병원 보내기"
            );

            if (success)
            {
                Debug.Log("병원 보내기 완료!");
            }
        }
    }



    /// <summary>
    /// 현재 재생 중인 패턴 중지
    /// </summary>
    void StopCurrentPattern()
    {
        if (patternPlayer != null && isPatternPlaying)
        {
            patternPlayer.StopPlayback();
            isPatternPlaying = false;
            StopAllCoroutines();
            StartRandomPatternLoop(); // 다시 루프 시작
        }
    }

    /// <summary>
    /// 디버그용: 상호작용 범위 표시
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
