using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PerfectButler.GameSystem;

/// <summary>
/// 플레이어 머리 위에 표시되는 상호작용 UI를 관리하는 클래스
/// </summary>
public class InteractionUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiPanel; // UI 패널
    public Transform uiWorldPosition; // UI가 표시될 월드 좌표 (플레이어 머리 위)
    public float uiHeightOffset = 5f; // 플레이어 머리 위 높이

    [Header("Cat Interaction Buttons")]
    public GameObject catInteractionPanel;
    public Button feedButton;      // 밥주기 버튼
    public Button playButton;      // 놀아주기 버튼
    public Button hospitalButton;  // 병원보내기 버튼

    [Header("Vacuum Cleaner UI")]
    public GameObject vacuumCleanerPanel;
    public Button cleanButton;     // 청소하기 버튼

    private bool isUIActive = false;
    private Transform playerTransform;

    public bool IsUIActive => isUIActive;

    void Start()
    {
        // UI 초기화
        if (uiPanel != null)
            uiPanel.SetActive(false);

        if (catInteractionPanel != null)
            catInteractionPanel.SetActive(false);

        if (vacuumCleanerPanel != null)
            vacuumCleanerPanel.SetActive(false);

        // 버튼 이벤트 리스너 등록
        SetupButtonListeners();
    }

    /// <summary>
    /// 버튼 이벤트 리스너 설정
    /// </summary>
    void SetupButtonListeners()
    {
        // 고양이 상호작용 버튼
        if (feedButton != null)
            feedButton.onClick.AddListener(OnFeedButtonClicked);

        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        if (hospitalButton != null)
            hospitalButton.onClick.AddListener(OnHospitalButtonClicked);

        // 청소기 버튼
        if (cleanButton != null)
            cleanButton.onClick.AddListener(OnCleanButtonClicked);
    }

    void OnDestroy()
    {
        // 버튼 이벤트 리스너 제거
        if (feedButton != null)
            feedButton.onClick.RemoveListener(OnFeedButtonClicked);

        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButtonClicked);

        if (hospitalButton != null)
            hospitalButton.onClick.RemoveListener(OnHospitalButtonClicked);

        if (cleanButton != null)
            cleanButton.onClick.RemoveListener(OnCleanButtonClicked);
    }

    void Update()
    {
        // UI가 활성화되어 있으면 플레이어를 따라다님
        if (isUIActive && playerTransform != null && uiPanel != null)
        {
            Vector3 targetPosition = playerTransform.position + Vector3.up * uiHeightOffset;
            uiPanel.transform.position = targetPosition;

            // UI가 카메라를 바라보도록
            if (Camera.main != null)
            {
                uiPanel.transform.LookAt(Camera.main.transform);
                uiPanel.transform.Rotate(0, 180, 0); // 반대로 보이지 않도록
            }
        }
    }

    /// <summary>
    /// 고양이 상호작용 UI 표시
    /// </summary>
    public void ShowCatInteractionUI(Transform player)
    {
        playerTransform = player;
        isUIActive = true;

        if (uiPanel != null)
            uiPanel.SetActive(true);

        if (catInteractionPanel != null)
            catInteractionPanel.SetActive(true);

        if (vacuumCleanerPanel != null)
            vacuumCleanerPanel.SetActive(false);
    }

    /// <summary>
    /// 청소기 상호작용 UI 표시
    /// </summary>
    public void ShowVacuumCleanerUI(Transform player)
    {
        playerTransform = player;
        isUIActive = true;

        if (uiPanel != null)
            uiPanel.SetActive(true);

        if (catInteractionPanel != null)
            catInteractionPanel.SetActive(false);

        if (vacuumCleanerPanel != null)
            vacuumCleanerPanel.SetActive(true);
    }

    /// <summary>
    /// UI 숨기기
    /// </summary>
    public void HideUI()
    {
        isUIActive = false;

        if (uiPanel != null)
            uiPanel.SetActive(false);

        if (catInteractionPanel != null)
            catInteractionPanel.SetActive(false);

        if (vacuumCleanerPanel != null)
            vacuumCleanerPanel.SetActive(false);

        playerTransform = null;
        
        // UI가 닫힌 후 힌트 다시 표시 (플레이어가 여전히 범위 내에 있다면)
        PlayerInteractionController playerController = FindObjectOfType<PlayerInteractionController>();
        if (playerController != null)
        {
            // UpdateInteractionTarget이 다음 프레임에 호출되어 힌트를 다시 표시함
        }
    }

    // ========== 버튼 클릭 이벤트 핸들러 ==========

    /// <summary>
    /// 밥주기 버튼 클릭
    /// </summary>
    void OnFeedButtonClicked()
    {
        CatInteractionManager catManager = CatInteractionManager.Instance;
        if (catManager != null)
        {
            catManager.OnFeedAction();
            HideUI();
        }
        else
        {
            Debug.LogWarning("CatInteractionManager를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 놀아주기 버튼 클릭
    /// </summary>
    void OnPlayButtonClicked()
    {
        CatInteractionManager catManager = CatInteractionManager.Instance;
        if (catManager != null)
        {
            catManager.OnPlayAction();
            HideUI();
        }
        else
        {
            Debug.LogWarning("CatInteractionManager를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 병원보내기 버튼 클릭
    /// </summary>
    void OnHospitalButtonClicked()
    {
        CatInteractionManager catManager = CatInteractionManager.Instance;
        if (catManager != null)
        {
            catManager.OnHospitalAction();
            HideUI();
        }
        else
        {
            Debug.LogWarning("CatInteractionManager를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 청소하기 버튼 클릭
    /// </summary>
    void OnCleanButtonClicked()
    {
        VacuumCleanerInteraction vacuum = FindObjectOfType<VacuumCleanerInteraction>();
        if (vacuum != null)
        {
            vacuum.PerformCleanAction();
            HideUI();
        }
        else
        {
            Debug.LogWarning("VacuumCleanerInteraction을 찾을 수 없습니다!");
        }
    }
}
