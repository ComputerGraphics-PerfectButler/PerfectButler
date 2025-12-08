using UnityEngine;
using System;
using System.Collections.Generic;

namespace PerfectButler.GameSystem
{
    public class CatStats : MonoBehaviour
    {
        public static CatStats Instance { get; private set; }

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

        private void Awake()
        {
            // 싱글톤 패턴 적용
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
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
        public static event Action<float, float> OnExperienceChanged; // currentExp, maxExp (새로 추가!)
        public static event Action OnGameOver;
        public static event Action OnGameComplete;
        
        private void Start()
        {
            // 쿨타임 Dictionary 초기화 (먼저 초기화)
            InitializeCooltime();

            // 저장된 데이터가 있으면 불러오기
            if (HasSaveData())
            {
                Debug.Log("[CatStats] 저장된 데이터 발견, 불러오기 시작");
                LoadGameData();
            }
            else
            {
                Debug.Log("[CatStats] 저장된 데이터 없음, 새 게임 시작");
                // 새 게임 시작 - 한 프레임 기다린 후 가구 설정
                StartCoroutine(InitializeFurnitureAfterFrame());
            }

            // 스탯 자동 감소만 시작 (경험치 자동증가 제거)
            InvokeRepeating(nameof(DecreaseStats), 1f, 1f);
        }


        /// 한 프레임 기다린 후 가구 초기화 (새 게임 시작 시)
        private System.Collections.IEnumerator InitializeFurnitureAfterFrame()
        {
            yield return null; // 한 프레임 대기
            Debug.Log("[CatStats] 한 프레임 대기 후 가구 초기화 시작");
            UpdateFurnitureVisibility();
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

        // 쿨타임 체크 없이 강제로 액션 수행 (미니게임 보상 등에 사용)
        public void PerformActionWithoutCooldownCheck(StatType statType, float statIncrease, float expReward, string actionName)
        {
            // 액션 수행
            ModifyStat(statType, statIncrease);
            GainExperienceFromAction(expReward, actionName);

            // 쿨타임 기록 (다음 액션을 위해)
            lastActionTime[statType] = Time.time;
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

            // 경험치 변경 이벤트 발생 (UI 업데이트용)
            OnExperienceChanged?.Invoke(experience, LevelData.EXP_PER_LEVEL);

            // 레벨업 체크
            if (experience >= LevelData.EXP_PER_LEVEL)
            {
                if (currentLevel < LevelData.MAX_LEVEL)
                {
                    currentLevel++;
                    experience = 0f;

                    Debug.Log($"레벨업! 현재 레벨: {CurrentLevelName}");
                    OnLevelChanged?.Invoke(currentLevel, experience, CurrentLevelName);

                    // 레벨업 후 경험치 다시 전송
                    OnExperienceChanged?.Invoke(experience, LevelData.EXP_PER_LEVEL);

                    UpdateFurnitureVisibility();
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

        // ===== 저장/불러오기 시스템 =====
        /// 현재 게임 상태를 저장합니다
        public void SaveGameData()
        {
            // 현재 Scene 이름 저장
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("SavedScene", currentScene);
            
            // 레벨 & 경험치
            PlayerPrefs.SetInt("SavedLevel", currentLevel);
            PlayerPrefs.SetFloat("SavedExp", experience);
            
            // 4가지 스탯
            PlayerPrefs.SetFloat("SavedHunger", hunger);
            PlayerPrefs.SetFloat("SavedCleanliness", cleanliness);
            PlayerPrefs.SetFloat("SavedFun", fun);
            PlayerPrefs.SetFloat("SavedHealth", health);
            
            // 저장 시간 기록
            PlayerPrefs.SetString("SavedTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            PlayerPrefs.Save();
            
            Debug.Log($"게임 저장 완료! Scene: {currentScene}, Lv.{CurrentLevelName}, Exp: {experience:F0}");
        }

        /// 저장된 게임 상태를 불러옵니다
        public void LoadGameData()
        {
            if (!HasSaveData())
            {
                Debug.LogWarning("[CatStats] 저장된 데이터가 없습니다!");
                return;
            }

            Debug.Log("[CatStats] 저장 데이터 로드 시작...");

            // 레벨 & 경험치 복구
            currentLevel = PlayerPrefs.GetInt("SavedLevel", 0);
            experience = PlayerPrefs.GetFloat("SavedExp", 0f);

            Debug.Log($"[CatStats] 레벨 복구: {currentLevel}, 경험치: {experience}");

            // 4가지 스탯 복구
            hunger = PlayerPrefs.GetFloat("SavedHunger", 80f);
            cleanliness = PlayerPrefs.GetFloat("SavedCleanliness", 80f);
            fun = PlayerPrefs.GetFloat("SavedFun", 80f);
            health = PlayerPrefs.GetFloat("SavedHealth", 80f);

            string savedTime = PlayerPrefs.GetString("SavedTime", "Unknown");
            Debug.Log($"[CatStats] 게임 불러오기 완료! Lv.{CurrentLevelName}, Exp: {experience:F0} (저장시간: {savedTime})");

            // UI 업데이트를 위해 이벤트 발생
            OnLevelChanged?.Invoke(currentLevel, experience, CurrentLevelName);
            OnExperienceChanged?.Invoke(experience, LevelData.EXP_PER_LEVEL);
            OnStatChanged?.Invoke(StatType.Hunger, hunger);
            OnStatChanged?.Invoke(StatType.Cleanliness, cleanliness);
            OnStatChanged?.Invoke(StatType.Fun, fun);
            OnStatChanged?.Invoke(StatType.Health, health);

            Debug.Log("[CatStats] 이벤트 발생 완료, 가구 업데이트 예약");
            // 한 프레임 기다린 후 가구 업데이트 (RoomDecoManager가 초기화될 시간을 줌)
            StartCoroutine(UpdateFurnitureAfterFrame());
        }

        /// <summary>
        /// 한 프레임 기다린 후 가구 업데이트 (데이터 로드 시)
        /// </summary>
        private System.Collections.IEnumerator UpdateFurnitureAfterFrame()
        {
            yield return null; // 한 프레임 대기
            Debug.Log("[CatStats] 한 프레임 대기 후 가구 업데이트 시작");
            UpdateFurnitureVisibility();
        }

        /// 저장된 게임 데이터가 있는지 확인
        public static bool HasSaveData()
        {
            return PlayerPrefs.HasKey("SavedLevel") && PlayerPrefs.HasKey("SavedScene");
        }
        /// 모든 저장 데이터 삭제
        public static void ClearSaveData()
        {
            PlayerPrefs.DeleteKey("SavedLevel");
            PlayerPrefs.DeleteKey("SavedExp");
            PlayerPrefs.DeleteKey("SavedScene");
            PlayerPrefs.DeleteKey("SavedHunger");
            PlayerPrefs.DeleteKey("SavedCleanliness");
            PlayerPrefs.DeleteKey("SavedFun");
            PlayerPrefs.DeleteKey("SavedHealth");
            PlayerPrefs.DeleteKey("SavedTime");
            PlayerPrefs.Save();
            Debug.Log("모든 저장 데이터를 삭제했습니다.");
        }

        // ===== 자동 저장 관련 =====
        private float autoSaveInterval = 60f; // 60초마다 자동 저장
        private float autoSaveTimer = 0f;

        // Start() 메서드에 추가:
        // InvokeRepeating(nameof(AutoSave), autoSaveInterval, autoSaveInterval);

        private void AutoSave()
        {
            SaveGameData();
            Debug.Log("자동 저장 완료!");
        }

        // ===== 테스트용 메서드 =====
        [ContextMenu("Save Game")]
        public void TestSaveGame()
        {
            SaveGameData();
        }

        [ContextMenu("Load Game")]
        public void TestLoadGame()
        {
            LoadGameData();
        }

        // [추가] 가구 매니저를 찾아서 가구를 갱신하라고 명령하는 함수
        private void UpdateFurnitureVisibility()
        {
            Debug.Log($"[CatStats] UpdateFurnitureVisibility 호출됨 - 현재 레벨: {currentLevel}");

            RoomDecoManager deco = FindObjectOfType<RoomDecoManager>();

            if (deco != null)
            {
                Debug.Log($"[CatStats] RoomDecoManager 발견, SetFurnitureVisibility({currentLevel}) 호출");
                deco.SetFurnitureVisibility(currentLevel);
                Debug.Log("🏠 가구 배치 업데이트 완료!");
            }
            else
            {
                Debug.LogWarning("[CatStats] RoomDecoManager를 찾을 수 없습니다!");
            }
        }
    }
}