using UnityEngine;
using UnityEngine.InputSystem;

public class CatInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    
    [Header("UI (Optional)")]
    public GameObject interactionPrompt;
    
    [Header("Manual Setup (옵션)")]
    public PlayerInventory playerInventoryReference;

    private PlayerInventory playerInventory;
    private CatWanderAI nearestCat;

    void Start()
    {
        // PlayerInventory 찾기
        if (playerInventoryReference != null)
        {
            playerInventory = playerInventoryReference;
            Debug.Log("[CatInteraction] PlayerInventory 수동 연결 성공!");
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<PlayerInventory>();
                if (playerInventory != null)
                {
                    Debug.Log("[CatInteraction] Player 태그로 PlayerInventory 찾음!");
                }
            }
        }
        
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory != null)
            {
                Debug.Log("[CatInteraction] FindObjectOfType으로 PlayerInventory 찾음!");
            }
        }
        
        if (playerInventory == null)
        {
            Debug.LogError("[CatInteraction] PlayerInventory를 찾을 수 없습니다!");
        }

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        FindNearestCat();
        UpdateUI();
        
        // ✅ Update에서 키 입력 직접 체크 (New Input System 문제 우회)
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
            
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = cat;
            }
        }

        nearestCat = closest;
    }

    void UpdateUI()
    {
        if (interactionPrompt != null)
        {
            bool canInteract = (nearestCat != null && playerInventory != null && playerInventory.currentItem != CatItem.None);
            interactionPrompt.SetActive(canInteract);
        }
    }

    // ✅ 실제 인터랙션 로직
    void TryInteractWithCat()
    {
        if (nearestCat == null)
        {
            Debug.Log("[CatInteraction] 근처에 고양이가 없습니다.");
            return;
        }
        
        if (playerInventory == null)
        {
            Debug.LogError("[CatInteraction] PlayerInventory가 null입니다!");
            return;
        }
        
        if (playerInventory.currentItem == CatItem.None)
        {
            Debug.Log("[CatInteraction] 인벤토리가 비어있습니다.");
            return;
        }

        Debug.Log($"[상호작용] {nearestCat.catName}에게 {playerInventory.currentItem} 제공 시도");

        bool accepted = nearestCat.TryAcceptItem(playerInventory.currentItem, transform);

        if (accepted)
        {
            playerInventory.ClearItem();
            Debug.Log("✅ 성공! 고양이가 따라옵니다!");
        }
        else
        {
            Debug.Log("❌ 실패! 다른 아이템을 시도해보세요.");
        }
    }

    // ✅ New Input System 이벤트 (혹시 작동하면 사용)
    public void OnInteractWithCat(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryInteractWithCat();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}

// using UnityEngine;
// using UnityEngine.InputSystem;

// public class CatInteraction : MonoBehaviour
// {
//     [Header("Interaction Settings")]
//     public float interactionDistance = 3f;
    
//     [Header("UI (Optional)")]
//     public GameObject interactionPrompt;

//     private PlayerInventory playerInventory;
//     private CatWanderAI nearestCat;

//     void Start()
//     {
//         // ✅ 수정: 이 스크립트가 Player에 붙어있으므로 GetComponent로 직접 가져오기
//         playerInventory = GetComponent<PlayerInventory>();
        
//         // 혹시 다른 오브젝트에 붙어있다면 FindObjectOfType으로 찾기
//         if (playerInventory == null)
//         {
//             playerInventory = FindObjectOfType<PlayerInventory>();
//         }
        
//         if (playerInventory == null)
//         {
//             Debug.LogError("[CatInteraction] PlayerInventory를 찾을 수 없습니다!");
//         }
//         else
//         {
//             Debug.Log($"[CatInteraction] PlayerInventory 찾음! 현재 아이템: {playerInventory.currentItem}");
//         }

//         if (interactionPrompt != null)
//             interactionPrompt.SetActive(false);
//     }

//     void Update()
//     {
//         FindNearestCat();
//         UpdateUI();
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
        
//         // ✅ 디버그: 가장 가까운 고양이 정보 출력
//         if (nearestCat != null)
//         {
//             Debug.Log($"[CatInteraction] 가장 가까운 고양이: {nearestCat.catName}, 거리: {Vector3.Distance(transform.position, nearestCat.transform.position):F2}m");
//         }
//     }

//     void UpdateUI()
//     {
//         if (interactionPrompt != null)
//         {
//             bool canInteract = (nearestCat != null && playerInventory != null && playerInventory.currentItem != CatItem.None);
//             interactionPrompt.SetActive(canInteract);
//         }
//     }

//     // E키 (New Input System)
//     public void OnInteractWithCat(InputAction.CallbackContext context)
//     {
//         Debug.Log($"[CatInteraction] OnInteractWithCat 호출됨! performed: {context.performed}");
        
//         if (!context.performed)
//         {
//             Debug.Log("[CatInteraction] context.performed가 false입니다.");
//             return;
//         }
        
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
//             Debug.Log("[CatInteraction] 인벤토리에 아이템이 없습니다.");
//             return;
//         }

//         Debug.Log($"[상호작용] {nearestCat.catName}에게 {playerInventory.currentItem} 제공");

//         bool accepted = nearestCat.TryAcceptItem(playerInventory.currentItem, transform);

//         if (accepted)
//         {
//             playerInventory.ClearItem();
//             Debug.Log("✅ 성공! 고양이가 따라옵니다!");
//         }
//         else
//         {
//             Debug.Log("❌ 실패! 다른 아이템을 시도해보세요.");
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.cyan;
//         Gizmos.DrawWireSphere(transform.position, interactionDistance);
//     }
// }