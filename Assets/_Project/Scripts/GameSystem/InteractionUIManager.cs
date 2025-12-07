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
    public float uiHeightOffset = 2.5f; // 플레이어 머리 위 높이

    [Header("Cat Interaction UI")]
    public GameObject catInteractionPanel;
    public TextMeshProUGUI[] catOptionTexts; // 밥주기, 놀아주기, 병원보내기

    [Header("Vacuum Cleaner UI")]
    public GameObject vacuumCleanerPanel;
    public TextMeshProUGUI vacuumCleanerText;

    [Header("UI Settings")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    // 고양이 상호작용 옵션
    private enum CatInteractionOption
    {
        Feed = 0,      // 밥주기
        Play = 1,      // 놀아주기
        Hospital = 2   // 병원보내기
    }

    private CatInteractionOption currentSelection = CatInteractionOption.Feed;
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

        // 옵션 텍스트 초기화
        if (catOptionTexts != null && catOptionTexts.Length > 0)
        {
            UpdateCatOptionTexts();
        }
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
        currentSelection = CatInteractionOption.Feed;

        if (uiPanel != null)
            uiPanel.SetActive(true);

        if (catInteractionPanel != null)
            catInteractionPanel.SetActive(true);

        if (vacuumCleanerPanel != null)
            vacuumCleanerPanel.SetActive(false);

        UpdateCatOptionTexts();
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

        if (vacuumCleanerText != null)
        {
            vacuumCleanerText.text = "청소하기";
            vacuumCleanerText.color = selectedColor;
        }
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

    /// <summary>
    /// 위로 이동 (고양이 메뉴에서만)
    /// </summary>
    public void NavigateUp()
    {
        if (!catInteractionPanel.activeSelf)
            return;

        // 순환: 밥주기 -> 병원보내기 -> 놀아주기 -> 밥주기
        currentSelection = (CatInteractionOption)(((int)currentSelection - 1 + 3) % 3);
        UpdateCatOptionTexts();
    }

    /// <summary>
    /// 아래로 이동 (고양이 메뉴에서만)
    /// </summary>
    public void NavigateDown()
    {
        if (!catInteractionPanel.activeSelf)
            return;

        // 순환: 밥주기 -> 놀아주기 -> 병원보내기 -> 밥주기
        currentSelection = (CatInteractionOption)(((int)currentSelection + 1) % 3);
        UpdateCatOptionTexts();
    }

    /// <summary>
    /// 선택 확정
    /// </summary>
    public void ConfirmSelection()
    {
        if (catInteractionPanel.activeSelf)
        {
            // 고양이 상호작용 처리
            ConfirmCatInteraction();
        }
        else if (vacuumCleanerPanel.activeSelf)
        {
            // 청소기 상호작용 처리
            ConfirmVacuumCleanerInteraction();
        }

        // UI 숨기기
        HideUI();
    }

    /// <summary>
    /// 고양이 상호작용 확정 처리
    /// </summary>
    void ConfirmCatInteraction()
    {
        CatInteractionManager catManager = CatInteractionManager.Instance;
        if (catManager == null)
        {
            Debug.LogWarning("CatInteractionManager를 찾을 수 없습니다!");
            return;
        }

        switch (currentSelection)
        {
            case CatInteractionOption.Feed:
                catManager.OnFeedAction();
                break;

            case CatInteractionOption.Play:
                catManager.OnPlayAction();
                break;

            case CatInteractionOption.Hospital:
                catManager.OnHospitalAction();
                break;
        }
    }

    /// <summary>
    /// 청소기 상호작용 확정 처리
    /// </summary>
    void ConfirmVacuumCleanerInteraction()
    {
        VacuumCleanerInteraction vacuum = FindObjectOfType<VacuumCleanerInteraction>();
        if (vacuum != null)
        {
            vacuum.PerformCleanAction();
        }
    }

    /// <summary>
    /// 고양이 옵션 텍스트 업데이트 (선택된 항목 강조)
    /// </summary>
    void UpdateCatOptionTexts()
    {
        if (catOptionTexts == null || catOptionTexts.Length < 3)
            return;

        string[] optionNames = { "밥주기", "놀아주기", "병원보내기" };

        for (int i = 0; i < 3; i++)
        {
            if (catOptionTexts[i] != null)
            {
                catOptionTexts[i].text = optionNames[i];
                catOptionTexts[i].color = (i == (int)currentSelection) ? selectedColor : normalColor;
            }
        }
    }
}
