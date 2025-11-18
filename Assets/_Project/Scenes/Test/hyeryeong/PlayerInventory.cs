using UnityEngine;

public enum CatItem
{
    None, // 0
    Snack, // 1
    LaserPointer, // 2
    ToyMouse, // 3
    Brush // 4
}

public class PlayerInventory : MonoBehaviour
{
    public CatItem currentItem = CatItem.None;

    public void AcquireItem(int itemIndex)
    {
        // 정수(int)로 받은 인덱스를 CatItem Enum으로 변환합니다.
        currentItem = (CatItem)itemIndex;
        Debug.Log($"[인벤토리]: {currentItem} 획득! 이제 고양이에게 사용할 수 있습니다.");
    }

    public void ClearItem()
    {
        currentItem = CatItem.None;
        Debug.Log("[인벤토리]: 아이템을 사용했습니다. 인벤토리가 비었습니다.");
    }
}