using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace PerfectButler.UI
{
    public class MiniGameSelectionPopup : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private Button fishingRodButton;
        [SerializeField] private Button whackAMoleButton;
        [SerializeField] private Button closeButton;
        
        [Header("Scene Names")]
        [SerializeField] private string fishingRodSceneName = "MiniGame_FishingRod";
        [SerializeField] private string whackAMoleSceneName = "MiniGame_WhackAMole";
        
        private void Start()
        {
            // 버튼 이벤트 연결
            fishingRodButton?.onClick.AddListener(OnFishingRodButtonClicked);
            whackAMoleButton?.onClick.AddListener(OnWhackAMoleButtonClicked);
            closeButton?.onClick.AddListener(ClosePopup);
            
            // 시작 시 팝업 숨김
            popupPanel?.SetActive(false);
        }
        
        /// <summary>
        /// 팝업을 표시합니다
        /// </summary>
        public void ShowPopup()
        {
            popupPanel?.SetActive(true);
            Time.timeScale = 0f; // 게임 일시정지
            Debug.Log("미니게임 선택 팝업 열림");
        }
        
        /// <summary>
        /// 팝업을 닫습니다
        /// </summary>
        public void ClosePopup()
        {
            popupPanel?.SetActive(false);
            Time.timeScale = 1f; // 게임 재개
            Debug.Log("미니게임 선택 팝업 닫힘");
        }
        
        /// <summary>
        /// 낚싯대 흔들기 게임 시작
        /// </summary>
        private void OnFishingRodButtonClicked()
        {
            Debug.Log("낚싯대 흔들기 게임 시작");
            Time.timeScale = 1f; // 씬 전환 전에 시간 정상화
            
            // 현재 씬 이름 저장 (돌아올 때 사용)
            PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
            
            // 미니게임 씬으로 전환
            SceneManager.LoadScene(fishingRodSceneName);
        }
        
        /// <summary>
        /// 두더지잡기 게임 시작
        /// </summary>
        private void OnWhackAMoleButtonClicked()
        {
            Debug.Log("두더지잡기 게임 시작");
            Time.timeScale = 1f; // 씬 전환 전에 시간 정상화
            
            // 현재 씬 이름 저장 (돌아올 때 사용)
            PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
            
            // 미니게임 씬으로 전환
            SceneManager.LoadScene(whackAMoleSceneName);
        }
        
        private void OnDestroy()
        {
            // 이벤트 구독 해제
            fishingRodButton?.onClick.RemoveListener(OnFishingRodButtonClicked);
            whackAMoleButton?.onClick.RemoveListener(OnWhackAMoleButtonClicked);
            closeButton?.onClick.RemoveListener(ClosePopup);
            
            // 시간 정상화 (안전장치)
            Time.timeScale = 1f;
        }
    }
}