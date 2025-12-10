using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PerfectButler.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        
        [Header("Main Menu Buttons")]
        [SerializeField] private Button startButton;      // 시작하기
        [SerializeField] private Button settingsButton;   // 설정
        [SerializeField] private Button quitButton;       // 나가기
        
        [Header("Settings Buttons")]
        [SerializeField] private Button backButton;       // 설정창에서 돌아오기
        
        [Header("Settings UI")]
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("SFX")]
        [SerializeField] private AudioClip buttonClickSound;   // 버튼 클릭 소리

        [Header("Scene Names")]
        [SerializeField] private string parkSceneName = "park"; // 동네 탐험 Scene
        [SerializeField] private string roomSceneName = "room";   // 집 Scene
        
        private void Start()
        {
            // 버튼 이벤트 연결
            SetupButtons();
            
            // 초기 패널 설정
            ShowMainMenu();
            
            // 저장된 설정 불러오기
            LoadSettings();
        }
        
        private void SetupButtons()
        {
            // 메인 메뉴 버튼
            if (startButton != null)
            {
                startButton.onClick.AddListener(PlayButtonSound);
                startButton.onClick.AddListener(OnStartGame);
            }
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(PlayButtonSound);
                settingsButton.onClick.AddListener(OnShowSettings);
            }
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(PlayButtonSound);
                quitButton.onClick.AddListener(OnQuitGame);
            }

            // 설정 화면 버튼
            if (backButton != null)
            {
                backButton.onClick.AddListener(PlayButtonSound);
                backButton.onClick.AddListener(OnBackToMainMenu);
            }

            // 설정 슬라이더/토글 이벤트
            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
        }

        // ===== 버튼 클릭 효과음 =====
        private void PlayButtonSound()
        {
            if (SFXManager.Instance != null && buttonClickSound != null)
            {
                SFXManager.Instance.PlaySound(buttonClickSound);
            }
        }
        
        private void ShowMainMenu()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }
        
        // ===== 시작하기 버튼 =====
        private void OnStartGame()
        {
            // 저장된 게임이 있는지 확인
            if (HasSaveData())
            {
                Debug.Log("저장된 게임을 불러옵니다!");
                LoadGameAndStart();
            }
            else
            {
                Debug.Log("새 게임을 시작합니다!");
                StartNewGame();
            }
        }
        
        // 세이브 데이터 존재 여부 확인
        private bool HasSaveData()
        {
            // 레벨 정보가 저장되어 있으면 세이브 데이터가 있다고 판단
            return PlayerPrefs.HasKey("SavedLevel") && PlayerPrefs.HasKey("SavedScene");
        }
        
        // 새 게임 시작
        private void StartNewGame()
        {
            // 기존 세이브 데이터 초기화
            ClearSaveData();
            
            // 첫 Scene(동네 탐험)으로 이동
            SceneManager.LoadScene(parkSceneName);
        }
        
        // 저장된 게임 불러오기
        private void LoadGameAndStart()
        {
            // 저장된 Scene 이름 가져오기
            string savedScene = PlayerPrefs.GetString("SavedScene", parkSceneName);
            
            // 저장된 캐릭터 스탯 불러오기 (실제로는 GameManager나 CatStats에서 처리)
            // 여기서는 어떤 데이터가 저장되어 있는지만 확인
            if (PlayerPrefs.HasKey("SavedLevel"))
            {
                int savedLevel = PlayerPrefs.GetInt("SavedLevel");
                float savedExp = PlayerPrefs.GetFloat("SavedExp", 0f);
                float savedHunger = PlayerPrefs.GetFloat("SavedHunger", 80f);
                float savedCleanliness = PlayerPrefs.GetFloat("SavedCleanliness", 80f);
                float savedFun = PlayerPrefs.GetFloat("SavedFun", 80f);
                float savedHealth = PlayerPrefs.GetFloat("SavedHealth", 80f);
                
                Debug.Log($"불러온 데이터: Lv.{savedLevel}, Exp:{savedExp}, Scene:{savedScene}");
            }
            
            // 저장된 Scene으로 이동
            SceneManager.LoadScene(savedScene);
        }
        
        // 세이브 데이터 초기화
        private void ClearSaveData()
        {
            PlayerPrefs.DeleteKey("SavedLevel");
            PlayerPrefs.DeleteKey("SavedExp");
            PlayerPrefs.DeleteKey("SavedScene");
            PlayerPrefs.DeleteKey("SavedHunger");
            PlayerPrefs.DeleteKey("SavedCleanliness");
            PlayerPrefs.DeleteKey("SavedFun");
            PlayerPrefs.DeleteKey("SavedHealth");
            PlayerPrefs.Save();
        }
        
        // ===== 설정 버튼 =====
        private void OnShowSettings()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }
        
        private void OnBackToMainMenu()
        {
            ShowMainMenu();
        }
        
        // ===== 나가기 버튼 =====
        private void OnQuitGame()
        {
            Debug.Log("게임을 종료합니다!");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        // ===== 설정 관련 =====
        private void LoadSettings()
        {
            // BGM 볼륨 (기본값 70%)
            float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.value = bgmVolume;
            }
                
            // BGMManager에 볼륨 적용
            if (BGMManager.Instance != null)
            {
                BGMManager.Instance.SetVolume(bgmVolume);
            }

            // SFX 볼륨 (기본값 80%)
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = sfxVolume;
            }
        }
        
        private void OnBGMVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("BGMVolume", value);
            PlayerPrefs.Save();
            
            // BGMManager에 볼륨 적용
            if (BGMManager.Instance != null)
            {
                BGMManager.Instance.SetVolume(value);
            }
            
            Debug.Log($"BGM 볼륨: {value * 100:F0}%");
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
            PlayerPrefs.Save();

            // SFXManager에 볼륨 적용
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.SetVolume(value);
            }

            Debug.Log($"SFX 볼륨: {value * 100:F0}%");
        }
        
        // ===== 디버그/테스트용 =====
        [ContextMenu("Clear All Save Data")]
        private void DebugClearSaveData()
        {
            ClearSaveData();
            Debug.Log("모든 세이브 데이터를 삭제했습니다.");
        }
        
        [ContextMenu("Create Test Save Data")]
        private void DebugCreateTestSaveData()
        {
            PlayerPrefs.SetInt("SavedLevel", 2);
            PlayerPrefs.SetFloat("SavedExp", 45f);
            PlayerPrefs.SetString("SavedScene", roomSceneName);
            PlayerPrefs.SetFloat("SavedHunger", 60f);
            PlayerPrefs.SetFloat("SavedCleanliness", 70f);
            PlayerPrefs.SetFloat("SavedFun", 80f);
            PlayerPrefs.SetFloat("SavedHealth", 90f);
            PlayerPrefs.Save();
            Debug.Log("테스트용 세이브 데이터를 생성했습니다.");
        }
    }
}