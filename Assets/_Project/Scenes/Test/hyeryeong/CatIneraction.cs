using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CatInteraction : MonoBehaviour
{
    [Header("UI Connection")]
    public GameObject interactionPrompt; // Press E Panel (회색 박스 1)

    // 👇 여기가 바뀜! 박스랑 글씨를 따로 연결해야 함
    public GameObject feedbackPanel;     // Feedback Panel (회색 박스 2 - 배경)
    public TMP_Text feedbackText;        // Feedback Text (글씨)

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

        // 시작할 때 박스들 싹 숨기기 (핵심!)
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        if (feedbackPanel != null) feedbackPanel.SetActive(false); // 박스 자체를 꺼버림
        if (transitionImagePanel != null) transitionImagePanel.SetActive(false); // 전환 이미지도 꺼둠

        // CanvasGroup 알파값 초기화
        if (transitionCanvasGroup != null) transitionCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        FindNearestCat();
        UpdateUI(); 

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

    void TryInteractWithCat()
    {
        if (nearestCat == null) return;
        
        // 1. 아이템 없을 때
        if (playerInventory == null || playerInventory.currentItem == CatItem.None)
        {
            ShowFeedback("You need an item!"); 
            return;
        }

        bool accepted = nearestCat.TryAcceptItem(playerInventory.currentItem, transform);

        if (accepted)
        {
            // 2. 성공했을 때
            playerInventory.ClearItem();
            ShowFeedback($"this cat loves it!");

            // 고양이 색상 정보 저장
            SaveCaughtCatData();

            Invoke("ShowTransitionImage", 4f); // 4초 뒤 전환 이미지 표시
        }
        else
        {
            // 3. 실패했을 때
            ShowFeedback("Cat hates it... Try another."); 
        }
    }

    // 📢 메시지 띄우는 함수
    void ShowFeedback(string message)
    {
        // 박스 켜고, 글씨 쓰고
        if (feedbackPanel != null) feedbackPanel.SetActive(true); 
        if (feedbackText != null) feedbackText.text = message;
        
        CancelInvoke("ClearFeedback");
        Invoke("ClearFeedback", 2f); // 2초 뒤 삭제 예약
    }

    // 🧹 메시지 지우는 함수
    void ClearFeedback()
    {
        // 박스 자체를 꺼버림!
        if (feedbackPanel != null) feedbackPanel.SetActive(false);
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
        if (transitionImagePanel != null)
        {
            transitionImagePanel.SetActive(true);
            StartCoroutine(FadeTransition());
        }
    }

    // 페이드 인 -> 유지 -> 페이드 아웃 -> 씬 전환
    System.Collections.IEnumerator FadeTransition()
    {
        // 1. 페이드 인
        if (transitionCanvasGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                transitionCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
                yield return null;
            }
            transitionCanvasGroup.alpha = 1f;
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
