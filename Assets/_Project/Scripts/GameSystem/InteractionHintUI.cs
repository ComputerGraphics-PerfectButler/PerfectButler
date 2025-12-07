using UnityEngine;
using TMPro;

/// <summary>
/// 상호작용 가능한 오브젝트 근처에서 "E" 힌트를 표시하는 클래스
/// (배틀그라운드 아이템 상호작용 UI 스타일)
/// </summary>
public class InteractionHintUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hintPanel; // E 힌트 패널
    public TextMeshProUGUI hintText; // "E" 텍스트
    public Transform uiWorldPosition; // UI가 표시될 월드 좌표
    
    [Header("UI Settings")]
    public float uiHeightOffset = 2.0f; // 오브젝트 위 높이
    public Color normalColor = Color.white;
    public float fadeSpeed = 5f; // 페이드 인/아웃 속도
    
    private Transform targetTransform; // 현재 타겟 오브젝트
    private bool isVisible = false;
    private CanvasGroup canvasGroup;

    void Start()
    {
        // CanvasGroup 가져오기 (페이드 효과용)
        if (hintPanel != null)
        {
            canvasGroup = hintPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = hintPanel.AddComponent<CanvasGroup>();
            }
            
            // 시작 시 숨김
            canvasGroup.alpha = 0f;
            hintPanel.SetActive(false);
        }

        // 기본 텍스트 설정
        if (hintText != null)
        {
            hintText.text = "E";
            hintText.color = normalColor;
        }
    }

    void Update()
    {
        // UI가 보이는 상태이고 타겟이 있으면 따라다님
        if (isVisible && targetTransform != null && hintPanel != null)
        {
            Vector3 targetPosition = targetTransform.position + Vector3.up * uiHeightOffset;
            hintPanel.transform.position = targetPosition;

            // UI가 카메라를 바라보도록
            if (Camera.main != null)
            {
                hintPanel.transform.LookAt(Camera.main.transform);
                hintPanel.transform.Rotate(0, 180, 0);
            }
        }

        // 페이드 효과
        if (canvasGroup != null)
        {
            float targetAlpha = isVisible ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }

    /// <summary>
    /// 힌트 UI 표시
    /// </summary>
    public void ShowHint(Transform target, string hintKey = "E")
    {
        if (hintPanel == null) return;

        targetTransform = target;
        isVisible = true;

        if (!hintPanel.activeSelf)
        {
            hintPanel.SetActive(true);
        }

        if (hintText != null)
        {
            hintText.text = hintKey;
        }
    }

    /// <summary>
    /// 힌트 UI 숨김
    /// </summary>
    public void HideHint()
    {
        isVisible = false;
        targetTransform = null;

        // 완전히 페이드 아웃된 후 비활성화
        if (canvasGroup != null && canvasGroup.alpha <= 0.01f)
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 즉시 숨김 (페이드 없이)
    /// </summary>
    public void HideHintImmediate()
    {
        isVisible = false;
        targetTransform = null;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }
}
