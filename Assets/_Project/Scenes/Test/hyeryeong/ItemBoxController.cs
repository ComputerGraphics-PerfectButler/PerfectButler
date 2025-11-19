using UnityEngine;
using UnityEngine.InputSystem;

public class ItemBoxController : MonoBehaviour
{
    public GameObject itemSelectPanel; 
    
    private const string PlayerTag = "Player";
    private bool isPlayerInRange = false;
    private bool hasSelectedItem = false; // ✅ 아이템 선택 여부 추적

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isPlayerInRange = true;
            Debug.Log("플레이어가 아이템 상자 범위에 진입했습니다. E키로 상호작용 가능.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isPlayerInRange = false;
            Debug.Log("플레이어가 상자 범위를 벗어났습니다.");

            if (itemSelectPanel != null)
                itemSelectPanel.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isPlayerInRange)
            {
                // ✅ 이미 아이템을 선택했으면 패널을 열지 않음
                if (hasSelectedItem)
                {
                    Debug.Log("[아이템 상자] 이미 아이템을 선택했습니다.");
                    return;
                }
                
                if (itemSelectPanel != null)
                {
                    bool isActive = !itemSelectPanel.activeSelf;
                    itemSelectPanel.SetActive(isActive);
                }
                Debug.Log("[아이템 상자] 패널 토글");
            }
        }
    }

    public void OnItemSelect(int itemIndex)
    {
        // ✅ 이미 선택했으면 무시
        if (hasSelectedItem)
        {
            Debug.Log("[아이템 상자] 이미 아이템을 선택했습니다.");
            return;
        }
        
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AcquireItem(itemIndex);
            hasSelectedItem = true; // ✅ 선택 완료 표시
            Debug.Log($"[아이템 상자] 아이템 선택 완료. 더 이상 변경할 수 없습니다.");
        }
        
        if (itemSelectPanel != null)
            itemSelectPanel.SetActive(false);
    }
    
    // ✅ (선택사항) 리셋 함수 - 필요시 다른 스크립트에서 호출 가능
    public void ResetSelection()
    {
        hasSelectedItem = false;
        Debug.Log("[아이템 상자] 선택 초기화됨. 다시 선택할 수 있습니다.");
    }
}
// using UnityEngine;
// using UnityEngine.InputSystem; // New Input System 사용을 위해 필수

// public class ItemBoxController : MonoBehaviour
// {
//     // Inspector에서 연결할 UI 패널 (버튼 5개가 들어있는 패널)
//     public GameObject itemSelectPanel; 
    
//     private const string PlayerTag = "Player";
//     private bool isPlayerInRange = false;

//     // ==========================================================
//     // 1️⃣ 상호작용 범위 감지 (트리거)
//     // ==========================================================
//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag(PlayerTag))
//         {
//             isPlayerInRange = true;
//             Debug.Log("플레이어가 아이템 상자 범위에 진입했습니다. E키로 상호작용 가능.");
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag(PlayerTag))
//         {
//             isPlayerInRange = false;
//             Debug.Log("플레이어가 상자 범위를 벗어났습니다.");

//             // 범위 벗어나면 패널 닫기
//             if (itemSelectPanel != null)
//                 itemSelectPanel.SetActive(false);
//         }
//     }

//     // ==========================================================
//     // 2️⃣ E키 입력 처리 (New Input System - Invoke Unity Events 모드)
//     // ==========================================================
//     // Behavior가 'Invoke Unity Events'이므로,
//     // 매개변수는 반드시 'InputAction.CallbackContext' 여야 합니다.
//     public void OnInteract(InputAction.CallbackContext context)
//     {
//         // context.performed는 키가 눌린 순간을 감지합니다.
//         if (context.performed)
//         {
//             // 플레이어가 범위 안에 있을 때만 동작
//             if (isPlayerInRange)
//             {
//                 if (itemSelectPanel != null)
//                 {
//                     // 패널 열기/닫기 토글
//                     bool isActive = !itemSelectPanel.activeSelf;
//                     itemSelectPanel.SetActive(isActive);
//                 }
//                 Debug.Log("[New Input System] 상호작용 키 감지 — 패널 토글.");
//             }
//         }
//     }

//     // ==========================================================
//     // 3️⃣ 버튼 클릭 처리 (아이템 선택)
//     // ==========================================================
//     public void OnItemSelect(int itemIndex)
//     {
//         // PlayerInventory 스크립트를 찾아 아이템 획득 함수 호출
//         PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
//         if (inventory != null)
//         {
//             inventory.AcquireItem(itemIndex);
//         }
        
//         // 선택 후 패널 닫기
//         if (itemSelectPanel != null)
//             itemSelectPanel.SetActive(false);
//     }
// }