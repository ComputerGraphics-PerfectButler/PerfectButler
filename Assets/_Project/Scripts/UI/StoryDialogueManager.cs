using UnityEngine;
using UnityEngine.UI;
using System;

namespace PerfectButler.UI
{
    /// <summary>
    /// Park 씬 시작 시 스토리 대사 이미지를 순차적으로 표시하는 매니저
    /// </summary>
    public class StoryDialogueManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;        // 대사 패널 전체
        [SerializeField] private Image dialogueImage;             // 대사 이미지를 표시할 Image
        [SerializeField] private Button nextButton;               // 다음 버튼

        [Header("Dialogue Images")]
        [SerializeField] private Sprite[] dialogueSprites;        // 대사 이미지 배열 (2개)

        [Header("SFX")]
        [SerializeField] private AudioClip nextButtonSound;       // 다음 버튼 소리

        private int currentDialogueIndex = 0;                     // 현재 대사 인덱스

        // 대사 시스템이 활성화되어 있는지 여부
        public bool IsDialogueActive { get; private set; }

        // 대사 시스템 종료 시 호출될 이벤트
        public event Action OnDialogueComplete;

        private void Start()
        {
            // 버튼 이벤트 연결
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnNextButtonClicked);
            }

            // 초기에는 대사 패널 비활성화
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            IsDialogueActive = false;
        }

        /// <summary>
        /// 스토리 대사 시스템 시작
        /// </summary>
        public void StartDialogue()
        {
            if (dialogueSprites == null || dialogueSprites.Length == 0)
            {
                Debug.LogError("대사 이미지가 설정되지 않았습니다! 대사 시스템을 종료합니다.");
                // 대사 이미지가 없으면 바로 종료 이벤트 발생
                OnDialogueComplete?.Invoke();
                return;
            }

            // 초기화
            currentDialogueIndex = 0;
            IsDialogueActive = true;

            // 대사 패널 활성화
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            // 첫 번째 대사 이미지 표시
            ShowCurrentDialogue();
        }

        /// <summary>
        /// 현재 인덱스의 대사 이미지 표시
        /// </summary>
        private void ShowCurrentDialogue()
        {
            if (dialogueImage != null && currentDialogueIndex < dialogueSprites.Length)
            {
                dialogueImage.sprite = dialogueSprites[currentDialogueIndex];
                Debug.Log($"대사 {currentDialogueIndex + 1}/{dialogueSprites.Length} 표시");
            }
        }

        /// <summary>
        /// 다음 버튼 클릭 시 호출
        /// </summary>
        private void OnNextButtonClicked()
        {
            // 효과음 재생
            PlayNextButtonSound();

            // 다음 대사로 이동
            currentDialogueIndex++;

            // 모든 대사를 다 봤는지 확인
            if (currentDialogueIndex >= dialogueSprites.Length)
            {
                // 대사 종료
                EndDialogue();
            }
            else
            {
                // 다음 대사 표시
                ShowCurrentDialogue();
            }
        }

        /// <summary>
        /// 대사 시스템 종료
        /// </summary>
        private void EndDialogue()
        {
            IsDialogueActive = false;

            // 대사 패널 비활성화
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            Debug.Log("스토리 대사 종료!");

            // 대사 종료 이벤트 발생
            OnDialogueComplete?.Invoke();
        }

        /// <summary>
        /// 다음 버튼 효과음 재생
        /// </summary>
        private void PlayNextButtonSound()
        {
            if (SFXManager.Instance != null && nextButtonSound != null)
            {
                SFXManager.Instance.PlaySound(nextButtonSound);
            }
        }

        /// <summary>
        /// 대사 시스템을 강제로 종료 (디버그용)
        /// </summary>
        [ContextMenu("Force End Dialogue")]
        public void ForceEndDialogue()
        {
            EndDialogue();
        }
    }
}
