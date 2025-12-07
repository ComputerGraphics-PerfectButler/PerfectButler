using UnityEngine;

// 가구를 껐다 켰다 하는 역할
public class RoomDecoManager : MonoBehaviour
{
    public GameObject[] furnitureList; // 인스펙터에서 가구 넣기

    void Start()
    {
        // 게임 시작 시 일단 모두 숨김 (초기화)
        foreach (GameObject prop in furnitureList)
        {
            if(prop != null) prop.SetActive(false);
        }
    }

    // ★ 외부(레벨 매니저)에서 이 함수를 호출할 겁니다!
    public void SetFurnitureVisibility(int level)
    {
        // 레벨에 맞춰 가구를 켭니다.
        // 예: 레벨 2면 -> 0번, 1번 가구 켜기 (인덱스는 0부터 시작하므로 < level)
        // 작성하신 레벨은 0부터 시작하므로 (level + 1)개 보여주거나, 기획에 맞게 조정
        
        for (int i = 0; i < furnitureList.Length; i++)
        {
            // 예: 현재 레벨이 1(초보 집사)라면 index 0번 가구 하나 보여주기
            // 레벨이 0(왕초보)일 때 아무것도 없는 방이라면 i < level
            // 레벨 0부터 가구 하나 주고 싶다면 i <= level
            if (i < level) 
            {
                if(furnitureList[i] != null) furnitureList[i].SetActive(true);
            }
        }
    }
}
