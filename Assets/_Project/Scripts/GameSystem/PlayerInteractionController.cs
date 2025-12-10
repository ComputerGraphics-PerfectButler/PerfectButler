using UnityEngine;

/// <summary>
/// 플레이어의 상호작용 입력을 처리하는 클래스
/// PlayerMovement.cs에 추가하거나 별도 컴포넌트로 사용
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("References")]
    public CatInteractionManager catInteractionManager;
    public VacuumCleanerInteraction vacuumCleaner;
    public InteractionUIManager uiManager;
    public InteractionHintUI hintUI; // E 힌트 UI

    // 현재 상호작용 가능한 대상
    private enum InteractionTarget
    {
        None,
        Cat,
        VacuumCleaner
    }

    private InteractionTarget currentTarget = InteractionTarget.None;

    void Start()
    {
        // 자동으로 찾기
        if (catInteractionManager == null)
            catInteractionManager = FindObjectOfType<CatInteractionManager>();

        if (vacuumCleaner == null)
            vacuumCleaner = FindObjectOfType<VacuumCleanerInteraction>();

        if (uiManager == null)
            uiManager = FindObjectOfType<InteractionUIManager>();

        if (hintUI == null)
            hintUI = FindObjectOfType<InteractionHintUI>();
    }

    void Update()
    {
        // 현재 상호작용 가능한 대상 확인
        UpdateInteractionTarget();

        // E 키 입력 처리
        if (Input.GetKeyDown(interactionKey))
        {
            HandleInteractionKeyPress();
        }
    }

    /// <summary>
    /// 현재 상호작용 가능한 대상 확인
    /// </summary>
    void UpdateInteractionTarget()
    {
        bool catInRange = catInteractionManager != null && catInteractionManager.IsPlayerInRange;
        bool vacuumInRange = vacuumCleaner != null && vacuumCleaner.IsPlayerInRange;

        // 고양이가 우선순위
        if (catInRange)
        {
            currentTarget = InteractionTarget.Cat;
            
            // E 힌트 표시 (고양이 위치에)
            if (hintUI != null && catInteractionManager != null)
            {
                hintUI.ShowHint(catInteractionManager.transform, "E");
            }
        }
        else if (vacuumInRange)
        {
            currentTarget = InteractionTarget.VacuumCleaner;
            
            // E 힌트 표시 (청소기 위치에)
            if (hintUI != null && vacuumCleaner != null)
            {
                hintUI.ShowHint(vacuumCleaner.transform, "E");
            }
        }
        else
        {
            currentTarget = InteractionTarget.None;
            
            // 힌트 숨김
            if (hintUI != null)
            {
                hintUI.HideHint();
            }
        }
    }

    /// <summary>
    /// E 키 입력 처리
    /// </summary>
    void HandleInteractionKeyPress()
    {
        if (uiManager == null)
            return;

        // UI가 이미 열려있으면 무시
        if (uiManager.IsUIActive)
            return;

        switch (currentTarget)
        {
            case InteractionTarget.Cat:
                // 고양이 상호작용 UI 표시
                uiManager.ShowCatInteractionUI(transform);
                
                // E 힌트 숨김
                if (hintUI != null)
                {
                    hintUI.HideHintImmediate();
                }
                break;

            case InteractionTarget.VacuumCleaner:
                // 청소기 상호작용 UI 표시
                uiManager.ShowVacuumCleanerUI(transform);
                
                // E 힌트 숨김
                if (hintUI != null)
                {
                    hintUI.HideHintImmediate();
                }
                break;

            case InteractionTarget.None:
                // 상호작용 가능한 대상 없음
                break;
        }
    }
}
