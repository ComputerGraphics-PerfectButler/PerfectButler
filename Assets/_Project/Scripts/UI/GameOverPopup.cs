using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using PerfectButler.GameSystem;

namespace PerfectButler.UI
{
    /// <summary>
    /// 게임 오버 팝업을 관리하는 클래스
    /// </summary>
    public class GameOverPopup : MonoBehaviour
    {
        public static GameOverPopup Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button confirmButton; // 확인 버튼

        [Header("Scene Names")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        [Header("SFX")]
        [SerializeField] private AudioClip gameOverSound; // 게임 오버 소리

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 버튼 이벤트 연결
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            }

            // 시작 시 팝업 숨김
            if (popupPanel != null)
            {
                popupPanel.SetActive(false);
            }

            // 게임 오버 이벤트 구독
            CatStats.OnGameOver += ShowGameOverPopup;
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            CatStats.OnGameOver -= ShowGameOverPopup;

            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            }

            // 시간 정상화 (안전장치)
            Time.timeScale = 1f;
        }

        /// <summary>
        /// 게임 오버 팝업 표시
        /// </summary>
        public void ShowGameOverPopup()
        {
            if (popupPanel == null)
            {
                Debug.LogWarning("GameOverPopup: popupPanel이 null입니다!");
                return;
            }

            // 스탯 UI 숨기기
            if (MainGameUI.Instance != null)
            {
                MainGameUI.Instance.HideUI();
            }

            // 게임 오버 소리 재생
            if (SFXManager.Instance != null && gameOverSound != null)
            {
                SFXManager.Instance.PlaySound(gameOverSound);
            }

            // 메시지 설정
            if (messageText != null)
            {
                messageText.text = "게임 오버!\n고양이의 스탯이 0이 되었습니다.";
            }

            // 팝업 표시
            popupPanel.SetActive(true);
            Time.timeScale = 0f; // 게임 일시정지

            Debug.Log("게임 오버! 메인 메뉴로 돌아갑니다.");
        }

        /// <summary>
        /// 확인 버튼 클릭 - 저장 삭제 후 메인 메뉴로 이동
        /// </summary>
        private void OnConfirmButtonClicked()
        {
            // 시간 정상화
            Time.timeScale = 1f;

            // 저장 데이터 삭제
            CatStats.ClearSaveData();
            Debug.Log("게임 오버: 저장 데이터 삭제 완료");

            // CatStats 인스턴스 파괴 (새 게임 시작 시 새로 생성되도록)
            if (CatStats.Instance != null)
            {
                Destroy(CatStats.Instance.gameObject);
            }

            // BGMManager, SFXManager 등 다른 싱글톤도 파괴
            if (BGMManager.Instance != null)
            {
                Destroy(BGMManager.Instance.gameObject);
            }

            if (SFXManager.Instance != null)
            {
                Destroy(SFXManager.Instance.gameObject);
            }

            // 메인 메뉴로 이동
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
