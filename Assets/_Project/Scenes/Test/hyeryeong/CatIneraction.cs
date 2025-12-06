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


// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.SceneManagement; // 씬 이동

// public class CatInteraction : MonoBehaviour
// {
//     [Header("Interaction Settings")]
//     public float interactionDistance = 3f;
    
//     [Header("UI (Optional)")]
//     public GameObject interactionPrompt;
    
//     [Header("Manual Setup (옵션)")]
//     public PlayerInventory playerInventoryReference;

//     private PlayerInventory playerInventory;
//     private CatWanderAI nearestCat;

//     void Start()
//     {
//         // PlayerInventory 찾기
//         if (playerInventoryReference != null)
//         {
//             playerInventory = playerInventoryReference;
//             Debug.Log("[CatInteraction] PlayerInventory 수동 연결 성공!");
//         }
//         else
//         {
//             GameObject player = GameObject.FindGameObjectWithTag("Player");
//             if (player != null)
//             {
//                 playerInventory = player.GetComponent<PlayerInventory>();
//                 if (playerInventory != null)
//                 {
//                     Debug.Log("[CatInteraction] Player 태그로 PlayerInventory 찾음!");
//                 }
//             }
//         }
        
//         if (playerInventory == null)
//         {
//             playerInventory = FindObjectOfType<PlayerInventory>();
//             if (playerInventory != null)
//             {
//                 Debug.Log("[CatInteraction] FindObjectOfType으로 PlayerInventory 찾음!");
//             }
//         }
        
//         if (playerInventory == null)
//         {
//             Debug.LogError("[CatInteraction] PlayerInventory를 찾을 수 없습니다!");
//         }

//         if (interactionPrompt != null)
//             interactionPrompt.SetActive(false);
//     }

//     void Update()
//     {
//         FindNearestCat();
//         UpdateUI();
        
//         // ✅ Update에서 키 입력 직접 체크 (New Input System 문제 우회)
//         if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
//         {
//             TryInteractWithCat();
//         }
//     }

//     void FindNearestCat()
//     {
//         CatWanderAI[] allCats = FindObjectsOfType<CatWanderAI>();
//         CatWanderAI closest = null;
//         float closestDist = interactionDistance;

//         foreach (CatWanderAI cat in allCats)
//         {
//             float dist = Vector3.Distance(transform.position, cat.transform.position);
            
//             if (dist < closestDist)
//             {
//                 closestDist = dist;
//                 closest = cat;
//             }
//         }

//         nearestCat = closest;
//     }

//     void UpdateUI()
//     {
//         if (interactionPrompt != null)
//         {
//             bool canInteract = (nearestCat != null);
//             interactionPrompt.SetActive(canInteract);
//         }
//     }

//     // ✅ 실제 인터랙션 로직
//     void TryInteractWithCat()
//     {
//         if (nearestCat == null)
//         {
//             Debug.Log("[CatInteraction] 근처에 고양이가 없습니다.");
//             return;
//         }
        
//         if (playerInventory == null)
//         {
//             Debug.LogError("[CatInteraction] PlayerInventory가 null입니다!");
//             return;
//         }
        
//         if (playerInventory.currentItem == CatItem.None)
//         {
//             Debug.Log("[CatInteraction] 인벤토리가 비어있습니다.");
//             return;
//         }

//         Debug.Log($"[상호작용] {nearestCat.catName}에게 {playerInventory.currentItem} 제공 시도");

//         bool accepted = nearestCat.TryAcceptItem(playerInventory.currentItem, transform);

//         if (accepted)
//         {
//             playerInventory.ClearItem();
//             Debug.Log("✅ 성공! 고양이가 따라옵니다!");


//             OnCatCaught(); // 고양이 획득 후 방으로 이동
//         }
//         else
//         {
//             Debug.Log("❌ 실패! 다른 아이템을 시도해보세요.");
//         }
//     }

//     // ✅ New Input System 이벤트 (혹시 작동하면 사용)
//     public void OnInteractWithCat(InputAction.CallbackContext context)
//     {
//         if (context.performed)
//         {
//             TryInteractWithCat();
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.cyan;
//         Gizmos.DrawWireSphere(transform.position, interactionDistance);
//     }

//     // 고양이와 상호작용(냥줍) 성공 시 호출될 함수
//     public void OnCatCaught()
//     {
//         Debug.Log("고양이 획득! 방으로 이동합니다.");
//         SceneManager.LoadScene("room"); 
//     }
// }
