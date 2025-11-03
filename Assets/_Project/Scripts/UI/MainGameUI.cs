using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PerfectButler.GameSystem;

namespace PerfectButler.UI
{
    public class MainGameUI : MonoBehaviour
    {
        [Header("Stat Bars")]
        [SerializeField] private Slider hungerBar;
        [SerializeField] private Slider cleanlinessBar;
        [SerializeField] private Slider funBar;
        [SerializeField] private Slider healthBar;
        
        [Header("Level System")]
        [SerializeField] private Image expCircle; // 원형 경험치 바
        [SerializeField] private TextMeshProUGUI levelText; // "Lv.1 초보 집사"
        [SerializeField] private TextMeshProUGUI expStatusText; // "거의키" 같은 상태
        
        [Header("Quest System")]
        [SerializeField] private Toggle feedCatQuest; // 고양이 밥 주기 5회
        [SerializeField] private Toggle playCatQuest; // 고양이 놀아주기 10회
        [SerializeField] private Toggle cleanHouseQuest; // 집 깨끗하게 정리하기
        [SerializeField] private Toggle levelUpQuest; // 레벨업하기
        
        [Header("Action Buttons")]
        [SerializeField] private Button feedButton;
        [SerializeField] private Button cleanButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button hospitalButton;
        
        [Header("Cooltime Display")]
        [SerializeField] private TextMeshProUGUI feedCooltime;
        [SerializeField] private TextMeshProUGUI cleanCooltime;
        [SerializeField] private TextMeshProUGUI playCooltime;
        [SerializeField] private TextMeshProUGUI hospitalCooltime;
        
        // 퀘스트 진행상황 추적
        private int feedCount = 0;
        private int playCount = 0;
        private bool isHouseCleaned = false;
        private int initialLevel = 0;
        
        private CatStats catStats;
        
        private void Start()
        {
            // CatStats 참조 찾기
            catStats = FindObjectOfType<CatStats>();
            if (catStats == null)
            {
                Debug.LogError("CatStats를 찾을 수 없습니다!");
                return;
            }
            
            initialLevel = catStats.CurrentLevel;
            
            // 이벤트 구독
            CatStats.OnStatChanged += UpdateStatBar;
            CatStats.OnLevelChanged += UpdateLevelUI;
            
            // 버튼 이벤트 연결
            SetupButtons();
            
            // 초기 UI 업데이트
            UpdateAllUI();
            
            // 쿨타임 업데이트 시작
            InvokeRepeating(nameof(UpdateCooltime), 0.1f, 0.1f);
        }
        
        private void SetupButtons()
        {
            feedButton.onClick.AddListener(() => TryAction(StatType.Hunger, 25f, ActionExpReward.FEED_CAT, "밥주기"));
            cleanButton.onClick.AddListener(() => TryAction(StatType.Cleanliness, 10f, ActionExpReward.CLEAN_DUST, "청소하기"));
            playButton.onClick.AddListener(() => TryAction(StatType.Fun, 15f, ActionExpReward.PLAY_MINIGAME, "놀아주기"));
            hospitalButton.onClick.AddListener(() => TryAction(StatType.Health, 100f, ActionExpReward.HOSPITAL_VISIT, "병원가기"));
        }
        
        private void TryAction(StatType statType, float statIncrease, float expReward, string actionName)
        {
            if (catStats.TryPerformAction(statType, statIncrease, expReward, actionName))
            {
                // 퀘스트 진행상황 업데이트
                UpdateQuestProgress(statType);
            }
        }
        
        private void UpdateQuestProgress(StatType statType)
        {
            switch (statType)
            {
                case StatType.Hunger:
                    feedCount++;
                    feedCatQuest.isOn = feedCount >= 5;
                    break;
                    
                case StatType.Fun:
                    playCount++;
                    playCatQuest.isOn = playCount >= 10;
                    break;
                    
                case StatType.Cleanliness:
                    // 청결도가 90 이상이면 집이 깨끗하다고 판단
                    if (catStats.Cleanliness >= 90f)
                    {
                        isHouseCleaned = true;
                        cleanHouseQuest.isOn = true;
                    }
                    break;
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
            
            // 경험치 상태 텍스트
            if (expPercentage < 0.3f)
                expStatusText.text = "초보";
            else if (expPercentage < 0.7f)
                expStatusText.text = "성장중";
            else
                expStatusText.text = "거의다";
            
            // 레벨업 퀘스트 체크
            if (catStats.CurrentLevel > initialLevel)
            {
                levelUpQuest.isOn = true;
            }
        }
        
        private void UpdateCooltime()
        {
            UpdateCooltimeText(StatType.Hunger, feedCooltime);
            UpdateCooltimeText(StatType.Cleanliness, cleanCooltime);
            UpdateCooltimeText(StatType.Fun, playCooltime);
            UpdateCooltimeText(StatType.Health, hospitalCooltime);
        }
        
        private void UpdateCooltimeText(StatType statType, TextMeshProUGUI cooltimeText)
        {
            if (catStats.CanPerformAction(statType))
            {
                cooltimeText.text = "사용가능";
                cooltimeText.color = Color.green;
            }
            else
            {
                float remaining = catStats.GetRemainingCooltime(statType);
                cooltimeText.text = $"{remaining:F0}초";
                cooltimeText.color = Color.red;
            }
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
        }
    }
}