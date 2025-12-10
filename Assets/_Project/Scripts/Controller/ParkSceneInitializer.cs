using UnityEngine;
using PerfectButler.UI;

/// <summary>
/// Park 씬 시작 시 스토리 대사를 자동으로 실행하고 플레이어 이동을 제어하는 스크립트
/// </summary>
public class ParkSceneInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StoryDialogueManager storyDialogueManager;  // 스토리 대사 매니저
    [SerializeField] private ParkPlayerMovement playerMovement;          // 플레이어 이동 컨트롤러

    [Header("Settings")]
    [SerializeField] private bool autoStartDialogue = true;              // 씬 시작 시 자동으로 대사 시작 여부

    private void Start()
    {
        // 스토리 대사 매니저 자동 찾기 (참조가 없는 경우)
        if (storyDialogueManager == null)
        {
            storyDialogueManager = FindObjectOfType<StoryDialogueManager>();
        }

        // 플레이어 이동 컨트롤러 자동 찾기 (참조가 없는 경우)
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<ParkPlayerMovement>();
        }

        // 스토리 대사 매니저가 있으면 이벤트 구독
        if (storyDialogueManager != null)
        {
            // 이벤트 구독을 먼저 하고
            storyDialogueManager.OnDialogueComplete += OnDialogueComplete;

            // 자동 시작 설정이 켜져 있으면 대사 시작 (다음 프레임에 실행)
            if (autoStartDialogue)
            {
                // Invoke를 사용하여 약간의 딜레이를 줌
                Invoke(nameof(StartStoryDialogue), 0.1f);
            }
        }
        else
        {
            Debug.LogWarning("StoryDialogueManager를 찾을 수 없습니다!");
            // 대사 매니저가 없으면 플레이어 이동 허용
            if (playerMovement != null)
            {
                playerMovement.SetCanMove(true);
            }
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (storyDialogueManager != null)
        {
            storyDialogueManager.OnDialogueComplete -= OnDialogueComplete;
        }
    }

    /// <summary>
    /// 스토리 대사 시작
    /// </summary>
    private void StartStoryDialogue()
    {
        // 대사 매니저가 없으면 플레이어 이동 허용하고 리턴
        if (storyDialogueManager == null)
        {
            Debug.LogWarning("StoryDialogueManager가 없어서 대사를 시작할 수 없습니다. 플레이어 이동을 허용합니다.");
            if (playerMovement != null)
            {
                playerMovement.SetCanMove(true);
            }
            return;
        }

        // 플레이어 이동 제한
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
            Debug.Log("플레이어 이동 제한됨 (대사 진행 중)");
        }

        // 대사 시작
        storyDialogueManager.StartDialogue();
        Debug.Log("스토리 대사 시작!");
    }

    /// <summary>
    /// 대사 종료 시 호출되는 콜백
    /// </summary>
    private void OnDialogueComplete()
    {
        // 플레이어 이동 허용
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
            Debug.Log("플레이어 이동 허용됨 (대사 종료)");
        }
    }

    /// <summary>
    /// 대사를 수동으로 시작 (디버그용 또는 특정 조건에서 사용)
    /// </summary>
    [ContextMenu("Manual Start Dialogue")]
    public void ManualStartDialogue()
    {
        StartStoryDialogue();
    }
}
