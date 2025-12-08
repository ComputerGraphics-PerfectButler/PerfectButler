using UnityEngine;
using UnityEngine.SceneManagement;
using PerfectButler.GameSystem;

namespace PerfectButler.MiniGames
{
    /// 모든 미니게임의 기본이 되는 베이스 클래스
    public abstract class MiniGameBase : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] protected float gameTimeLimit = 15f; // 게임 제한시간
        
        protected float elapsedTime = 0f;
        protected bool isGameActive = false;
        protected MiniGameResult currentResult = MiniGameResult.Normal;
        
        protected virtual void Start()
        {
            StartGame();
        }
        
        protected virtual void Update()
        {
            if (!isGameActive) return;
            
            elapsedTime += Time.deltaTime;
            
            // 시간 초과 체크
            if (elapsedTime >= gameTimeLimit)
            {
                EndGame();
            }
        }
        
        /// 게임 시작
        protected virtual void StartGame()
        {
            isGameActive = true;
            elapsedTime = 0f;
            Debug.Log($"{GetGameName()} 시작!");
        }
        
        /// 게임 종료 및 결과 처리
        protected virtual void EndGame()
        {
            if (!isGameActive) return;

            isGameActive = false;

            // 결과 계산
            currentResult = CalculateResult();

            // 결과 저장
            Debug.Log($"[MiniGameBase] 미니게임 결과 저장 시작: {currentResult}");
            MiniGameResultManager.SaveResult(currentResult);

            // 미니게임 완료 플래그 설정
            PlayerPrefs.SetInt("MiniGameCompleted", 1);
            PlayerPrefs.Save();

            Debug.Log($"[MiniGameBase] 미니게임 결과 저장 완료, 완료 플래그 설정");
            Debug.Log($"{GetGameName()} 종료! 결과: {currentResult}");

            // 이전 씬으로 돌아가기
            ReturnToPreviousScene();
        }
        
        /// 게임 결과 계산 (각 미니게임에서 구현)
        protected abstract MiniGameResult CalculateResult();
        
        /// 게임 이름 반환 (각 미니게임에서 구현)
        protected abstract string GetGameName();
        
        /// 이전 씬으로 돌아가기
        protected void ReturnToPreviousScene()
        {
            string previousScene = PlayerPrefs.GetString("PreviousScene", "room");
            SceneManager.LoadScene(previousScene);
        }
        
        /// 남은 시간 반환
        protected float GetRemainingTime()
        {
            return Mathf.Max(0f, gameTimeLimit - elapsedTime);
        }
        
        /// 진행률 반환 (0~1)
        protected float GetProgress()
        {
            return Mathf.Clamp01(elapsedTime / gameTimeLimit);
        }
    }
}