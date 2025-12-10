using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [Header("연결할 것들")]
    public GameObject uiObject;       // 화면의 "Press E..." 이미지 (PromptPanel)
    public GameObject questionMark;   // [추가됨] 박스 위의 뱅글뱅글 물음표

    [Header("UI 위치 설정")]
    public Vector3 uiOffset = new Vector3(0, 2f, 0); // 플레이어 머리 위 오프셋 (기본: 2유닛 위)

    private bool isNear = false;
    private bool isDone = false;      // 아이템을 이미 먹었는지 체크
    private Transform playerTransform; // 플레이어 Transform 참조
    private RectTransform uiRectTransform; // UI의 RectTransform

    void Start()
    {
        // 시작할 때 UI 끄기
        if (uiObject != null)
        {
            uiObject.SetActive(false);
            uiRectTransform = uiObject.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        // 근처에 있고 + 아직 안 먹었고 + E키를 눌렀을 때
        if (isNear && !isDone && Input.GetKeyDown(KeyCode.E))
        {
            DoAction();
        }

        // UI가 활성화되어 있고 플레이어가 있으면 UI 위치 업데이트
        if (uiObject != null && uiObject.activeSelf && playerTransform != null)
        {
            UpdateUIPosition();
        }
    }

    /// <summary>
    /// UI를 플레이어 머리 위로 따라다니게 함
    /// </summary>
    void UpdateUIPosition()
    {
        if (uiRectTransform == null || playerTransform == null) return;

        // 플레이어 머리 위의 월드 좌표
        Vector3 worldPosition = playerTransform.position + uiOffset;

        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // UI 위치 설정
        uiRectTransform.position = screenPosition;
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
            playerTransform = other.transform; // 플레이어 Transform 저장
            uiObject.SetActive(true);
        }
    }

    // 멀어지면 UI 끄기
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            playerTransform = null; // 플레이어 참조 해제
            uiObject.SetActive(false);
        }
    }
}