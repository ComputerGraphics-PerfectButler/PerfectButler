using UnityEngine;
using UnityEngine.InputSystem; // New Input System 사용을 위해 필수

public class ItemBoxController : MonoBehaviour
{
    // Inspector에서 연결할 UI 패널 (버튼 5개가 들어있는 패널)
    public GameObject itemSelectPanel; 
    
    private const string PlayerTag = "Player";
    private bool isPlayerInRange = false;

    // ==========================================================
    // 1️⃣ 상호작용 범위 감지 (트리거)
    // ==========================================================
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

            // 범위 벗어나면 패널 닫기
            if (itemSelectPanel != null)
                itemSelectPanel.SetActive(false);
        }
    }

    // ==========================================================
    // 2️⃣ E키 입력 처리 (New Input System - Invoke Unity Events 모드)
    // ==========================================================
    // Behavior가 'Invoke Unity Events'이므로,
    // 매개변수는 반드시 'InputAction.CallbackContext' 여야 합니다.
    public void OnInteract(InputAction.CallbackContext context)
    {
        // context.performed는 키가 눌린 순간을 감지합니다.
        if (context.performed)
        {
            // 플레이어가 범위 안에 있을 때만 동작
            if (isPlayerInRange)
            {
                if (itemSelectPanel != null)
                {
                    // 패널 열기/닫기 토글
                    bool isActive = !itemSelectPanel.activeSelf;
                    itemSelectPanel.SetActive(isActive);
                }
                Debug.Log("[New Input System] 상호작용 키 감지 — 패널 토글.");
            }
        }
    }

    // ==========================================================
    // 3️⃣ 버튼 클릭 처리 (아이템 선택)
    // ==========================================================
    public void OnItemSelect(int itemIndex)
    {
        // PlayerInventory 스크립트를 찾아 아이템 획득 함수 호출
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AcquireItem(itemIndex);
        }
        
        // 선택 후 패널 닫기
        if (itemSelectPanel != null)
            itemSelectPanel.SetActive(false);
    }
}