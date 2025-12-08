using UnityEngine;

/// <summary>
/// 가구를 관리하고 레벨에 따라 표시하는 클래스
/// 가구 개수 변화를 CatInteractionManager에 알림
/// </summary>
public class RoomDecoManager : MonoBehaviour
{
    public GameObject[] furnitureList; // 인스펙터에서 가구 넣기

    private int previousActiveFurnitureCount = 0;

    void Start()
    {
        // 초기화는 하지 않음 - CatStats에서 레벨 불러온 후 설정
        // 만약 아무것도 설정되지 않았다면 (새 게임) CatStats가 레벨 0으로 설정함
    }

    /// <summary>
    /// 외부(레벨 매니저)에서 이 함수를 호출합니다!
    /// 레벨에 맞춰 가구를 켜고/끕니다.
    /// </summary>
    public void SetFurnitureVisibility(int level)
    {
        Debug.Log($"[RoomDecoManager] SetFurnitureVisibility 호출됨 - 레벨: {level}, 전체 가구 수: {furnitureList.Length}");

        // 레벨에 맞춰 가구를 켜고/끕니다.
        // 예: 레벨 2면 -> 0번, 1번 가구 켜기, 나머지는 끄기
        // 레벨이 0(왕초보)일 때 아무것도 없는 방 (모두 꺼짐)

        int activatedCount = 0;
        int deactivatedCount = 0;

        for (int i = 0; i < furnitureList.Length; i++)
        {
            if (furnitureList[i] != null)
            {
                // i < level이면 켜기, 아니면 끄기
                bool shouldBeActive = i < level;

                if (shouldBeActive)
                {
                    furnitureList[i].SetActive(true);
                    activatedCount++;
                    Debug.Log($"[RoomDecoManager] 가구 {i} ({furnitureList[i].name}) 활성화");
                }
                else
                {
                    furnitureList[i].SetActive(false);
                    deactivatedCount++;
                    Debug.Log($"[RoomDecoManager] 가구 {i} ({furnitureList[i].name}) 비활성화");
                }
            }
            else
            {
                Debug.LogWarning($"[RoomDecoManager] 가구 {i}는 null입니다");
            }
        }

        Debug.Log($"[RoomDecoManager] 가구 설정 완료 - 활성화: {activatedCount}개, 비활성화: {deactivatedCount}개");

        // 가구 개수가 변경되었으면 CatInteractionManager에 알림
        int currentCount = GetActiveFurnitureCount();
        if (currentCount != previousActiveFurnitureCount)
        {
            previousActiveFurnitureCount = currentCount;
            NotifyCatInteractionManager();
        }
    }

    /// <summary>
    /// 현재 활성화된 가구 개수 반환
    /// </summary>
    public int GetActiveFurnitureCount()
    {
        int count = 0;
        foreach (GameObject furniture in furnitureList)
        {
            if (furniture != null && furniture.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// CatInteractionManager에 가구 개수 변경 알림
    /// </summary>
    void NotifyCatInteractionManager()
    {
        CatInteractionManager catManager = CatInteractionManager.Instance;
        if (catManager != null)
        {
            catManager.UpdateActiveFurnitureCount();
            Debug.Log($"가구 개수 변경 알림: {previousActiveFurnitureCount}개");
        }
    }
}