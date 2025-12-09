using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CatInteraction : MonoBehaviour
{
    [Header("UI Connection")]
    public GameObject interactionPrompt; // Press E to Interact 이미지 오브젝트

    [Header("Feedback Images")]
    public GameObject feedbackNeedItem;  // "You need an item!" 이미지
    public GameObject feedbackLovesIt;   // "This cat loves it!" 이미지
    public GameObject feedbackHatesIt;   // "Cat hates it... Try another." 이미지
    public bool followCat = true;        // 고양이 위에 표시할지 여부
    public Vector3 feedbackOffset = new Vector3(0, 3, 0); // 고양이 위 오프셋

    [Header("Transition Image")]
    public GameObject transitionImagePanel; // cat_interaction.png를 표시할 패널
    public CanvasGroup transitionCanvasGroup; // 페이드 효과용

    [Header("Transition Settings")]
    public float fadeInDuration = 0.5f;     // 페이드 인 시간
    public float imageDuration = 2.5f;      // 이미지 유지 시간
    public float fadeOutDuration = 0.5f;    // 페이드 아웃 시간

    [Header("Settings")]
    public float interactionDistance = 3f;
    public PlayerInventory playerInventoryReference;

    private PlayerInventory playerInventory;
    private CatWanderAI nearestCat;

    void Start()
    {
        // 인벤토리 찾기
        if (playerInventoryReference != null) playerInventory = playerInventoryReference;
        else playerInventory = FindObjectOfType<PlayerInventory>();

        // 시작할 때 UI 싹 숨기기 (핵심!)
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        if (feedbackNeedItem != null) feedbackNeedItem.SetActive(false);
        if (feedbackLovesIt != null) feedbackLovesIt.SetActive(false);
        if (feedbackHatesIt != null) feedbackHatesIt.SetActive(false);
        if (transitionImagePanel != null) transitionImagePanel.SetActive(false); // 전환 이미지도 꺼둠

        // CanvasGroup 알파값 초기화
        if (transitionCanvasGroup != null) transitionCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        FindNearestCat();
        UpdateUI();
        UpdateFeedbackPosition(); // 피드백 위치 업데이트

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteractWithCat();
        }
    }

    void FindNearestCat()
    {
        CatWanderAI[] allCats = FindObjectsOfType<CatWanderAI>();
        CatWanderAI closest = null;
        float closestDist = interactionDistance;

        foreach (CatWanderAI cat in allCats)
        {
            float dist = Vector3.Distance(transform.position, cat.transform.position);
            if (dist < closestDist) { closestDist = dist; closest = cat; }
        }
        nearestCat = closest;
    }

    void UpdateUI()
    {
        if (interactionPrompt != null)
        {
            bool canInteract = (nearestCat != null);
            interactionPrompt.SetActive(canInteract);
        }
    }

    void UpdateFeedbackPosition()
    {
        if (!followCat || nearestCat == null) return;

        // 현재 활성화된 피드백 이미지 찾기
        GameObject activeFeedback = null;
        if (feedbackNeedItem != null && feedbackNeedItem.activeSelf) activeFeedback = feedbackNeedItem;
        else if (feedbackLovesIt != null && feedbackLovesIt.activeSelf) activeFeedback = feedbackLovesIt;
        else if (feedbackHatesIt != null && feedbackHatesIt.activeSelf) activeFeedback = feedbackHatesIt;

        if (activeFeedback != null)
        {
            // 고양이의 월드 위치를 스크린 위치로 변환
            Vector3 catWorldPos = nearestCat.transform.position + feedbackOffset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(catWorldPos);

            // RectTransform 위치 업데이트
            RectTransform rectTransform = activeFeedback.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = screenPos;
            }
        }
    }

    void TryInteractWithCat()
    {
        if (nearestCat == null) return;
        
        // 1. 아이템 없을 때
        if (playerInventory == null || playerInventory.currentItem == CatItem.None)
        {
            ShowFeedback(feedbackNeedItem);
            return;
        }

        bool accepted = nearestCat.TryAcceptItem(playerInventory.currentItem, transform);

        if (accepted)
        {
            // 2. 성공했을 때
            playerInventory.ClearItem();
            ShowFeedback(feedbackLovesIt);

            // 고양이 색상 정보 저장
            SaveCaughtCatData();

            Invoke("ShowTransitionImage", 4f); // 4초 뒤 전환 이미지 표시
        }
        else
        {
            // 3. 실패했을 때
            ShowFeedback(feedbackHatesIt);
        }
    }

    // 📢 피드백 이미지 띄우는 함수
    void ShowFeedback(GameObject feedbackImage)
    {
        // 먼저 모든 피드백 이미지 끄기
        if (feedbackNeedItem != null) feedbackNeedItem.SetActive(false);
        if (feedbackLovesIt != null) feedbackLovesIt.SetActive(false);
        if (feedbackHatesIt != null) feedbackHatesIt.SetActive(false);

        // 선택된 이미지만 켜기
        if (feedbackImage != null) feedbackImage.SetActive(true);

        CancelInvoke("ClearFeedback");
        Invoke("ClearFeedback", 2f); // 2초 뒤 삭제 예약
    }

    // 🧹 피드백 이미지 지우는 함수
    void ClearFeedback()
    {
        if (feedbackNeedItem != null) feedbackNeedItem.SetActive(false);
        if (feedbackLovesIt != null) feedbackLovesIt.SetActive(false);
        if (feedbackHatesIt != null) feedbackHatesIt.SetActive(false);
    }

    // 💾 냥줍한 고양이 데이터 저장
    void SaveCaughtCatData()
    {
        if (nearestCat != null)
        {
            // Material 이름 저장
            if (nearestCat.catMaterial != null)
            {
                PlayerPrefs.SetString("CaughtCat_MaterialName", nearestCat.catMaterial.name);
                Debug.Log($"고양이 저장: {nearestCat.catName}, Material: {nearestCat.catMaterial.name}");
            }

            // 고양이 이름 저장
            PlayerPrefs.SetString("CaughtCat_Name", nearestCat.catName);

            PlayerPrefs.Save();
        }
    }

    // 🖼️ 전환 이미지 표시
    void ShowTransitionImage()
    {
        Debug.Log("ShowTransitionImage 호출됨!");
        if (transitionImagePanel != null)
        {
            Debug.Log("Panel 활성화!");
            transitionImagePanel.SetActive(true);
            StartCoroutine(FadeTransition());
        }
        else
        {
            Debug.LogError("transitionImagePanel이 연결되지 않았습니다!");
        }
    }

    // 페이드 인 -> 유지 -> 페이드 아웃 -> 씬 전환
    System.Collections.IEnumerator FadeTransition()
    {
        // 1. 페이드 인
        if (transitionCanvasGroup != null)
        {
            Debug.Log("페이드 인 시작! Alpha: " + transitionCanvasGroup.alpha);
            float elapsedTime = 0f;
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                transitionCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
                yield return null;
            }
            transitionCanvasGroup.alpha = 1f;
            Debug.Log("페이드 인 완료! Alpha: " + transitionCanvasGroup.alpha);
        }
        else
        {
            Debug.LogError("transitionCanvasGroup이 연결되지 않았습니다!");
        }

        // 2. 이미지 유지
        yield return new WaitForSeconds(imageDuration);

        // 3. 페이드 아웃
        if (transitionCanvasGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                transitionCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeOutDuration);
                yield return null;
            }
            transitionCanvasGroup.alpha = 0f;
        }

        // 4. 씬 전환
        OnCatCaught();
    }

    public void OnCatCaught()
    {
        SceneManager.LoadScene("room");
    }
}
