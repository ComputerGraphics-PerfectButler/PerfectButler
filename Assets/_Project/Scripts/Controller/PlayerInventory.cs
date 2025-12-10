using UnityEngine;

public enum CatItem
{
    None,           // 0
    Toy1,           // 1 - 장난감1
    Toy2,           // 2 - 장난감2
    ToyMouse,       // 3 - 쥐돌이 장난감
    Churu,          // 4 - 츄르
    Scratcher       // 5 - 스크래쳐
}

public class PlayerInventory : MonoBehaviour
{
    public CatItem currentItem = CatItem.None;

    [Header("SFX")]
    [SerializeField] private AudioClip itemPickupSound; // 아이템 획득 소리

    void Start()
    {
        Debug.Log($"[PlayerInventory] 초기화됨. 시작 아이템: {currentItem}");
    }

    public void AcquireItem(int itemIndex)
    {
        currentItem = (CatItem)itemIndex;
        Debug.Log($"★★★ [인벤토리]: {currentItem} 획득! (인덱스: {itemIndex}) 이제 고양이에게 사용할 수 있습니다. ★★★");

        // 아이템 획득 소리 재생
        if (SFXManager.Instance != null && itemPickupSound != null)
        {
            SFXManager.Instance.PlaySound(itemPickupSound);
        }
    }

    public void ClearItem()
    {
        Debug.Log($"[인벤토리]: {currentItem}을(를) 사용했습니다.");
        currentItem = CatItem.None;
        Debug.Log("[인벤토리]: 인벤토리가 비었습니다.");
    }
}