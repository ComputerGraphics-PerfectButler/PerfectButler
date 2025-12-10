using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PerfectButler.GameSystem;

namespace PerfectButler.UI
{
    public class MainGameUI : MonoBehaviour
    {
        public static MainGameUI Instance { get; private set; }

        [Header("UI Panel")]
        [SerializeField] private GameObject uiPanel; // 전체 UI 패널 (게임 오버 시 숨길 패널)

        [Header("Stat Bars")]
        [SerializeField] private Slider hungerBar;
        [SerializeField] private Slider cleanlinessBar;
        [SerializeField] private Slider funBar;
        [SerializeField] private Slider healthBar;

        [Header("Level System")]
        [SerializeField] private Image expCircle; // 원형 경험치 바
        [SerializeField] private TextMeshProUGUI levelText; // "Lv.1 초보 집사"
        [SerializeField] private TextMeshProUGUI expText; // 경험치 숫자 표시 (예: "45/100")
        
        [Header("Action Buttons")]
        [SerializeField] private Button feedButton;
        [SerializeField] private Button cleanButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button hospitalButton;

        [Header("Save System")]
        [SerializeField] private Button saveButton;

        [Header("Mini Game Popup")]
        [SerializeField] private MiniGameSelectionPopup miniGamePopup;

        private CatStats catStats;

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
            // CatStats 참조 찾기
            catStats = FindObjectOfType<CatStats>();
            if (catStats == null)
            {
                Debug.LogError("CatStats를 찾을 수 없습니다!");
                return;
            }

            // initialLevel = catStats.CurrentLevel;

            // 이벤트 구독
            CatStats.OnStatChanged += UpdateStatBar;
            CatStats.OnLevelChanged += UpdateLevelUI;
            CatStats.OnExperienceChanged += UpdateExperienceUI;

            // 버튼 이벤트 연결
            SetupButtons();

            // 초기 UI 업데이트
            UpdateAllUI();

            // 쿨타임 업데이트 시작 (후순위)
            // InvokeRepeating(nameof(UpdateCooltime), 0.1f, 0.1f);
        }
        
        private void SetupButtons()
        {
            feedButton.onClick.AddListener(() => catStats.TryPerformAction(StatType.Hunger, 25f, ActionExpReward.FEED_CAT, "밥주기"));
            cleanButton.onClick.AddListener(() => catStats.TryPerformAction(StatType.Cleanliness, 10f, ActionExpReward.CLEAN_DUST, "청소하기"));
            // playButton.onClick.AddListener(() => catStats.TryPerformAction(StatType.Fun, 15f, ActionExpReward.PLAY_MINIGAME, "놀아주기"));
            hospitalButton.onClick.AddListener(() => catStats.TryPerformAction(StatType.Health, 100f, ActionExpReward.HOSPITAL_VISIT, "병원가기"));

            // 놀아주기는 먼저 쿨타임 체크 후 팝업 열기
            playButton.onClick.AddListener(OnPlayButtonClicked);

            // 저장 버튼
            if (saveButton != null)
            {
                saveButton.onClick.AddListener(OnSaveButtonClicked);
            }
        }
        
        /// 놀아주기 버튼 클릭 시 - 쿨타임 체크 후 미니게임 선택 팝업 열기
        private void OnPlayButtonClicked()
        {
            // 쿨타임 체크
            if (!catStats.CanPerformAction(StatType.Fun))
            {
                float remaining = catStats.GetRemainingCooltime(StatType.Fun);
                Debug.Log($"놀아주기 쿨타임 중... {remaining:F1}초 남음");
                // TODO: 쿨타임 알림 UI 표시
                return;
            }

            // 쿨타임이 없다면 미니게임 선택 팝업 열기
            if (miniGamePopup != null)
            {
                miniGamePopup.ShowPopup();
            }
            else
            {
                Debug.LogError("MiniGameSelectionPopup이 연결되지 않았습니다!");
            }
        }

        /// 저장 버튼 클릭 시 - 저장 후 게임 종료
        private void OnSaveButtonClicked()
        {
            if (catStats != null)
            {
                catStats.SaveGameData();
                Debug.Log("게임 저장 완료! 게임을 종료합니다.");
                // TODO: 저장 완료 알림 UI 표시 (선택적)

                // 게임 종료
#if UNITY_EDITOR
                // 에디터에서는 플레이 모드 종료
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // 빌드된 게임에서는 애플리케이션 종료
                Application.Quit();
#endif
            }
            else
            {
                Debug.LogError("CatStats를 찾을 수 없습니다!");
            }
        }

        
        private void UpdateStatBar(StatType statType, float value)
        {
            float normalizedValue = value / 100f;
            
            switch (statType)
            {
                case StatType.Hunger:
                    hungerBar.value = normalizedValue;
                    break;
                case StatType.Cleanliness:
                    cleanlinessBar.value = normalizedValue;
                    break;
                case StatType.Fun:
                    funBar.value = normalizedValue;
                    break;
                case StatType.Health:
                    healthBar.value = normalizedValue;
                    break;
            }
        }
        
        private void UpdateLevelUI(int level, float exp, string levelName)
        {
            // 레벨 텍스트 업데이트
            levelText.text = $"Lv.{level + 1} {levelName}";

            // 경험치 원형 바 업데이트
            float expPercentage = exp / LevelData.EXP_PER_LEVEL;
            expCircle.fillAmount = expPercentage;
            expText.text = $"{(int)exp}";
        }

        // 경험치 UI만 업데이트 (경험치 변경 시마다 호출)
        private void UpdateExperienceUI(float currentExp, float maxExp)
        {
            // 경험치 원형 바 업데이트
            float expPercentage = currentExp / maxExp;
            if (expCircle != null)
            {
                expCircle.fillAmount = expPercentage;
                Debug.Log($"ExpCircle 업데이트: {currentExp}/{maxExp} = {expPercentage:F2}");
            }
            else
                Debug.LogWarning("expCircle이 할당되지 않았습니다.");
            
            // 경험치 숫자 텍스트 (0~100)
            if (expText != null)
                expText.text = $"{currentExp:F0}";
            else
                Debug.LogWarning("expText가 할당되지 않았습니다.");
        }
        
        
        private void UpdateAllUI()
        {
            // 모든 스탯 바 초기 업데이트
            UpdateStatBar(StatType.Hunger, catStats.Hunger);
            UpdateStatBar(StatType.Cleanliness, catStats.Cleanliness);
            UpdateStatBar(StatType.Fun, catStats.Fun);
            UpdateStatBar(StatType.Health, catStats.Health);
            
            // 레벨 UI 초기 업데이트
            UpdateLevelUI(catStats.CurrentLevel, catStats.Experience, catStats.CurrentLevelName);
        }
        
        private void OnDestroy()
        {
            // 이벤트 구독 해제
            CatStats.OnStatChanged -= UpdateStatBar;
            CatStats.OnLevelChanged -= UpdateLevelUI;
            CatStats.OnExperienceChanged -= UpdateExperienceUI;
        }

        /// <summary>
        /// UI 패널 숨기기 (게임 오버/클리어 시 호출)
        /// </summary>
        public void HideUI()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
        }

        /// <summary>
        /// UI 패널 표시
        /// </summary>
        public void ShowUI()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(true);
            }
        }
    }
}