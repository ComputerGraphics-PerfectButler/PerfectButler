using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [Header("연결할 것들")]
    public GameObject uiObject;       // 화면의 "Press E..." 이미지 (PromptPanel)
    public GameObject questionMark;   // [추가됨] 박스 위의 뱅글뱅글 물음표

    private bool isNear = false;
    private bool isDone = false;      // 아이템을 이미 먹었는지 체크

    void Start()
    {
        // 시작할 때 UI 끄기
        if (uiObject != null) uiObject.SetActive(false);
    }

    void Update()
    {
        // 근처에 있고 + 아직 안 먹었고 + E키를 눌렀을 때
        if (isNear && !isDone && Input.GetKeyDown(KeyCode.E))
        {
            DoAction();
        }
    }

    void DoAction()
    {
        Debug.Log("아이템 획득!");
        
        // 1. 상태를 '완료'로 변경 (이제 E 눌러도 반응 안 함)
        isDone = true;

        // 2. 물음표 없애기 (핵심!)
        if (questionMark != null)
        {
            questionMark.SetActive(false);
        }

        // 3. 화면에 떠 있던 "Press E" UI도 즉시 끄기
        if (uiObject != null)
        {
            uiObject.SetActive(false);
        }
    }

    // 다가오면 UI 켜기
    void OnTriggerEnter(Collider other)
    {
        // 플레이어고 + 아직 아이템을 안 먹었을 때만 UI 보여줌
        if (other.CompareTag("Player") && !isDone)
        {
            isNear = true;
            uiObject.SetActive(true);
        }
    }

    // 멀어지면 UI 끄기
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            uiObject.SetActive(false);
        }
    }
}