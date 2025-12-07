using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro; 

public class CatInteraction : MonoBehaviour
{
    [Header("UI Connection")]
    public GameObject interactionPrompt; // Press E Panel (회색 박스 1)
    
    // 👇 여기가 바뀜! 박스랑 글씨를 따로 연결해야 함
    public GameObject feedbackPanel;     // Feedback Panel (회색 박스 2 - 배경)
    public TMP_Text feedbackText;        // Feedback Text (글씨)

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
            Invoke("OnCatCaught", 1.5f); 
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

    public void OnCatCaught()
    {
        SceneManager.LoadScene("room");
    }
}
