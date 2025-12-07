using UnityEngine;
using PerfectButler.GameSystem;

/// <summary>
/// 청소기(Vacuum Cleaner)와의 상호작용을 처리하는 클래스
/// </summary>
public class VacuumCleanerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRange = 2f; // 상호작용 범위
    public Transform playerTransform; // 플레이어 Transform

    [Header("Cleaning Settings")]
    public float cleanlinessIncrease = 10f; // 청소 시 증가하는 청결도

    // 상호작용 가능 여부
    public bool IsPlayerInRange { get; private set; }
    private bool isCatInRange = false;

    void Start()
    {
        // 플레이어 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    void Update()
    {
        // 플레이어와의 거리 체크
        CheckPlayerDistance();

        // 고양이와의 거리 체크
        CheckCatDistance();

        // 상호작용 가능 여부 업데이트
        UpdateInteractionAvailability();
    }

    /// <summary>
    /// 플레이어와의 거리를 체크
    /// </summary>
    void CheckPlayerDistance()
    {
        if (playerTransform == null)
        {
            IsPlayerInRange = false;
            return;
        }

        Vector3 vacuumPos = transform.position;
        Vector3 playerPos = playerTransform.position;

        // Y축 제외하고 XZ 평면에서의 거리 계산
        float distance = Vector2.Distance(
            new Vector2(vacuumPos.x, vacuumPos.z),
            new Vector2(playerPos.x, playerPos.z)
        );

        IsPlayerInRange = distance <= interactionRange;
    }

    /// <summary>
    /// 고양이와의 거리를 체크
    /// </summary>
    void CheckCatDistance()
    {
        CatInteractionManager catManager = CatInteractionManager.Instance;
        if (catManager == null)
        {
            isCatInRange = false;
            return;
        }

        Vector3 vacuumPos = transform.position;
        Vector3 catPos = catManager.transform.position;

        // Y축 제외하고 XZ 평면에서의 거리 계산
        float distance = Vector2.Distance(
            new Vector2(vacuumPos.x, vacuumPos.z),
            new Vector2(catPos.x, catPos.z)
        );

        // 고양이가 상호작용 범위 내에 있는지 확인
        isCatInRange = catManager.IsPlayerInRange;
    }

    /// <summary>
    /// 상호작용 가능 여부 업데이트
    /// 플레이어가 범위 내에 있고, 고양이가 상호작용 범위에 없을 때만 가능
    /// </summary>
    void UpdateInteractionAvailability()
    {
        // 플레이어가 청소기 근처에 있고, 고양이가 플레이어 근처에 없을 때만 청소 가능
        bool canInteract = IsPlayerInRange && !isCatInRange;
        
        // 상태가 바뀌었을 때만 로그 출력
        if (canInteract != IsPlayerInRange)
        {
            if (canInteract)
            {
                Debug.Log("청소기 사용 가능!");
            }
        }
    }

    /// <summary>
    /// 청소 액션 수행
    /// </summary>
    public void PerformCleanAction()
    {
        // 고양이가 근처에 있으면 청소 불가
        if (isCatInRange)
        {
            Debug.Log("고양이가 근처에 있어서 청소할 수 없습니다!");
            return;
        }

        if (CatStats.Instance != null)
        {
            bool success = CatStats.Instance.TryPerformAction(
                StatType.Cleanliness,
                cleanlinessIncrease,
                ActionExpReward.CLEAN_DUST,
                "청소하기"
            );

            if (success)
            {
                Debug.Log("청소 완료!");
                // TODO: 청소 이펙트 재생 (파티클, 사운드 등)
            }
        }
    }

    /// <summary>
    /// 디버그용: 상호작용 범위 표시
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
