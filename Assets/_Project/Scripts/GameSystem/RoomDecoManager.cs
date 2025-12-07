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
        // 게임 시작 시 일단 모두 숨김 (초기화)
        foreach (GameObject prop in furnitureList)
        {
            if(prop != null) prop.SetActive(false);
        }

        previousActiveFurnitureCount = 0;
    }

    /// <summary>
    /// 외부(레벨 매니저)에서 이 함수를 호출합니다!
    /// 레벨에 맞춰 가구를 켭니다.
    /// </summary>
    public void SetFurnitureVisibility(int level)
    {
        // 레벨에 맞춰 가구를 켭니다.
        // 예: 레벨 2면 -> 0번, 1번 가구 켜기 (인덱스는 0부터 시작하므로 < level)
        
        for (int i = 0; i < furnitureList.Length; i++)
        {
            // 현재 레벨이 1(초보 집사)라면 index 0번 가구 하나 보여주기
            // 레벨이 0(왕초보)일 때 아무것도 없는 방
            if (i < level) 
            {
                if(furnitureList[i] != null) 
                {
                    furnitureList[i].SetActive(true);
                }
            }
        }

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