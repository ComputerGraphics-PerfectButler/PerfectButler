using UnityEngine;
using UnityEngine.SceneManagement;
using PerfectButler.GameSystem;

namespace PerfectButler.SaveSystem
{
    /// 게임 진행상황을 저장하고 불러오는 매니저
    /// CatStats 스크립트에 추가해서 사용하거나, 별도 GameObject에 추가
    public class SaveLoadManager : MonoBehaviour
    {
        private static SaveLoadManager instance;
        public static SaveLoadManager Instance => instance;
        
        private void Awake()
        {
            // 싱글톤 패턴
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // ===== 게임 저장 =====
        public void SaveGame(CatStats catStats)
        {
            if (catStats == null)
            {
                Debug.LogError("CatStats를 찾을 수 없어 저장할 수 없습니다!");
                return;
            }
            
            // 현재 Scene 이름 저장
            string currentScene = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("SavedScene", currentScene);
            
            // 레벨 & 경험치
            PlayerPrefs.SetInt("SavedLevel", catStats.CurrentLevel);
            PlayerPrefs.SetFloat("SavedExp", catStats.Experience);
            
            // 4가지 스탯
            PlayerPrefs.SetFloat("SavedHunger", catStats.Hunger);
            PlayerPrefs.SetFloat("SavedCleanliness", catStats.Cleanliness);
            PlayerPrefs.SetFloat("SavedFun", catStats.Fun);
            PlayerPrefs.SetFloat("SavedHealth", catStats.Health);
            
            // 저장 시간 기록
            PlayerPrefs.SetString("SavedTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            PlayerPrefs.Save();
            
            Debug.Log($"게임 저장 완료! Scene: {currentScene}, Lv.{catStats.CurrentLevel}");
        }
        
        // ===== 게임 불러오기 =====
        public void LoadGame(CatStats catStats)
        {
            if (catStats == null)
            {
                Debug.LogError("CatStats를 찾을 수 없어 불러올 수 없습니다!");
                return;
            }
            
            if (!HasSaveData())
            {
                Debug.LogWarning("저장된 데이터가 없습니다!");
                return;
            }
            
            // 레벨 & 경험치 복구
            int savedLevel = PlayerPrefs.GetInt("SavedLevel", 0);
            float savedExp = PlayerPrefs.GetFloat("SavedExp", 0f);
            
            // 4가지 스탯 복구
            float savedHunger = PlayerPrefs.GetFloat("SavedHunger", 80f);
            float savedCleanliness = PlayerPrefs.GetFloat("SavedCleanliness", 80f);
            float savedFun = PlayerPrefs.GetFloat("SavedFun", 80f);
            float savedHealth = PlayerPrefs.GetFloat("SavedHealth", 80f);
            
            // CatStats에 적용 (private 변수이므로 리플렉션 사용)
            // 더 나은 방법은 CatStats에 LoadData 메서드를 추가하는 것
            catStats.GetType().GetField("currentLevel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(catStats, savedLevel);
            
            catStats.GetType().GetField("experience", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(catStats, savedExp);
            
            catStats.GetType().GetField("hunger", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(catStats, savedHunger);
            
            catStats.GetType().GetField("cleanliness", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(catStats, savedCleanliness);
            
            catStats.GetType().GetField("fun", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(catStats, savedFun);
            
            catStats.GetType().GetField("health", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(catStats, savedHealth);
            
            string savedTime = PlayerPrefs.GetString("SavedTime", "Unknown");
            Debug.Log($"게임 불러오기 완료! (저장시간: {savedTime})");
        }
        
        // ===== 세이브 데이터 확인 =====
        public bool HasSaveData()
        {
            return PlayerPrefs.HasKey("SavedLevel") && PlayerPrefs.HasKey("SavedScene");
        }
        
        // ===== 자동 저장 (특정 시점에 호출) =====
        public void AutoSave()
        {
            CatStats catStats = FindObjectOfType<CatStats>();
            if (catStats != null)
            {
                SaveGame(catStats);
                Debug.Log("자동 저장 완료!");
            }
        }
    }
}