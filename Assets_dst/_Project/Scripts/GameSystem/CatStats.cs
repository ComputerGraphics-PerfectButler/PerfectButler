using UnityEngine;
using System;
using System.Collections.Generic;

namespace PerfectButler.GameSystem
{
    public class CatStats : MonoBehaviour
    {
        [Header("Cat Stats")]
        [SerializeField, Range(0, 100)] private float hunger = 80f;
        [SerializeField, Range(0, 100)] private float cleanliness = 80f;
        [SerializeField, Range(0, 100)] private float fun = 80f;
        [SerializeField, Range(0, 100)] private float health = 80f;
        
        [Header("Level System")]
        [SerializeField, Range(0, 100)] private float experience = 0f;
        [SerializeField] private int currentLevel = 0; // 0-4 (왕초보~완벽한집사)
        
        [Header("Stat Decrease Settings")]
        [SerializeField] private float baseDecreaseRate = 0.5f; // 기본 감소량 (초당)
        
        [Header("Cooltime System")]
        [SerializeField] private bool showCooltime = true; // Inspector에서 쿨타임 표시 여부
        
        // 쿨타임 추적용 Dictionary
        private Dictionary<StatType, float> lastActionTime = new Dictionary<StatType, float>();
        
        // 프로퍼티로 외부 접근
        public float Hunger => hunger;
        public float Cleanliness => cleanliness;
        public float Fun => fun;
        public float Health => health;
        public float Experience => experience;
        public int CurrentLevel => currentLevel;
        public string CurrentLevelName => LevelData.LevelNames[currentLevel];
        
        // 이벤트 시스템
        public static event Action<StatType, float> OnStatChanged;
        public static event Action<int, float, string> OnLevelChanged; // level, exp, levelName
        public static event Action OnGameOver;
        public static event Action OnGameComplete;
        
        private void Start()
        {
            // 스탯 자동 감소만 시작 (경험치 자동증가 제거)
            InvokeRepeating(nameof(DecreaseStats), 1f, 1f);
            
            // 쿨타임 Dictionary 초기화
            InitializeCooltime();
        }
        
        private void InitializeCooltime()
        {
            // 모든 액션 타입을 Dictionary에 초기화
            foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
            {
                lastActionTime[statType] = -1000f; // 시작 시 모든 액션 사용 가능하도록
            }
        }
        
        private void DecreaseStats()
        {
            // 모든 스탯 감소
            hunger = Mathf.Max(0, hunger - baseDecreaseRate);
            cleanliness = Mathf.Max(0, cleanliness - baseDecreaseRate);
            fun = Mathf.Max(0, fun - baseDecreaseRate);
            health = Mathf.Max(0, health - baseDecreaseRate);
            
            // 이벤트 발생
            OnStatChanged?.Invoke(StatType.Hunger, hunger);
            OnStatChanged?.Invoke(StatType.Cleanliness, cleanliness);
            OnStatChanged?.Invoke(StatType.Fun, fun);
            OnStatChanged?.Invoke(StatType.Health, health);
            
            // 게임 오버 체크
            CheckGameOver();
        }
        
        // 쿨타임 관련 메서드들
        public bool CanPerformAction(StatType statType)
        {
            if (!lastActionTime.ContainsKey(statType))
                return true;
                
            float cooltime = GetCooltimeForAction(statType);
            return Time.time - lastActionTime[statType] >= cooltime;
        }
        
        public float GetRemainingCooltime(StatType statType)
        {
            if (!lastActionTime.ContainsKey(statType))
                return 0f;
                
            float cooltime = GetCooltimeForAction(statType);
            float elapsed = Time.time - lastActionTime[statType];
            return Mathf.Max(0f, cooltime - elapsed);
        }
        
        private float GetCooltimeForAction(StatType statType)
        {
            return statType switch
            {
                StatType.Hunger => ActionCooltime.FEED_CAT,
                StatType.Cleanliness => ActionCooltime.CLEAN_DUST,
                StatType.Fun => ActionCooltime.PLAY_MINIGAME,
                StatType.Health => ActionCooltime.HOSPITAL_VISIT,
                _ => 0f
            };
        }
        
        // 쿨타임을 고려한 액션 실행 메서드
        public bool TryPerformAction(StatType statType, float statIncrease, float expReward, string actionName)
        {
            if (!CanPerformAction(statType))
            {
                float remaining = GetRemainingCooltime(statType);
                Debug.Log($"{actionName} 쿨타임 중... {remaining:F1}초 남음");
                return false;
            }
            
            // 액션 수행
            ModifyStat(statType, statIncrease);
            GainExperienceFromAction(expReward, actionName);
            
            // 쿨타임 기록
            lastActionTime[statType] = Time.time;
            
            return true;
        }
        
        public void ModifyStat(StatType statType, float amount)
        {
            switch (statType)
            {
                case StatType.Hunger:
                    hunger = Mathf.Clamp(hunger + amount, 0, 100);
                    break;
                case StatType.Cleanliness:
                    cleanliness = Mathf.Clamp(cleanliness + amount, 0, 100);
                    break;
                case StatType.Fun:
                    fun = Mathf.Clamp(fun + amount, 0, 100);
                    break;
                case StatType.Health:
                    health = Mathf.Clamp(health + amount, 0, 100);
                    break;
            }
            
            OnStatChanged?.Invoke(statType, GetStatValue(statType));
        }
        
        // 행동별 경험치 획득 (자동 증가 제거)
        public void GainExperienceFromAction(float expAmount, string actionName = "")
        {
            ModifyExperience(expAmount);
            if (!string.IsNullOrEmpty(actionName))
            {
                Debug.Log($"{actionName} 완료! 경험치 +{expAmount}");
            }
        }
        
        private void ModifyExperience(float amount)
        {
            experience += amount;
            
            // 레벨업 체크
            if (experience >= LevelData.EXP_PER_LEVEL)
            {
                if (currentLevel < LevelData.MAX_LEVEL)
                {
                    currentLevel++;
                    experience = 0f;
                    
                    Debug.Log($"레벨업! 현재 레벨: {CurrentLevelName}");
                    OnLevelChanged?.Invoke(currentLevel, experience, CurrentLevelName);
                }
                else
                {
                    // 최대 레벨에서 경험치 100 달성 시 게임 완료
                    if (experience >= LevelData.EXP_PER_LEVEL)
                    {
                        OnGameComplete?.Invoke();
                        Debug.Log("게임 완료! 완벽한 집사가 되었습니다!");
                    }
                }
            }
        }
        
        public float GetStatValue(StatType statType)
        {
            return statType switch
            {
                StatType.Hunger => hunger,
                StatType.Cleanliness => cleanliness,
                StatType.Fun => fun,
                StatType.Health => health,
                _ => 0f
            };
        }
        
        private void CheckGameOver()
        {
            // 하나라도 0이 되면 게임 오버
            if (hunger <= 0 || cleanliness <= 0 || fun <= 0 || health <= 0)
            {
                Debug.Log("게임 오버! 스탯이 0에 도달했습니다.");
                OnGameOver?.Invoke();
            }
        }
        
        // 테스트용 메서드들 (쿨타임 적용)
        [ContextMenu("Test Feed Cat")]
        public void TestFeedCat()
        {
            TryPerformAction(StatType.Hunger, 25f, ActionExpReward.FEED_CAT, "밥주기");
        }
        
        [ContextMenu("Test Clean")]
        public void TestClean()
        {
            TryPerformAction(StatType.Cleanliness, 10f, ActionExpReward.CLEAN_DUST, "청소하기");
        }
        
        [ContextMenu("Test Play")]
        public void TestPlay()
        {
            TryPerformAction(StatType.Fun, 15f, ActionExpReward.PLAY_MINIGAME, "놀아주기");
        }
        
        [ContextMenu("Test Hospital")]
        public void TestHospital()
        {
            TryPerformAction(StatType.Health, 100f, ActionExpReward.HOSPITAL_VISIT, "병원 보내기");
        }
        
        // 쿨타임 상태 확인용 테스트 메서드
        [ContextMenu("Check All Cooltime")]
        public void CheckAllCooltime()
        {
            foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
            {
                float remaining = GetRemainingCooltime(statType);
                string status = CanPerformAction(statType) ? "사용가능" : $"{remaining:F1}초 남음";
                Debug.Log($"{statType}: {status}");
            }
        }
    }
}