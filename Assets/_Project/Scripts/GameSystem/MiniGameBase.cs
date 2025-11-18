using UnityEngine;
using UnityEngine.SceneManagement;
using PerfectButler.GameSystem;

namespace PerfectButler.MiniGames
{
    /// <summary>
    /// 모든 미니게임의 기본이 되는 베이스 클래스
    /// </summary>
    public abstract class MiniGameBase : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] protected float gameTimeLimit = 30f; // 게임 제한시간
        
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
        
        /// <summary>
        /// 게임 시작
        /// </summary>
        protected virtual void StartGame()
        {
            isGameActive = true;
            elapsedTime = 0f;
            Debug.Log($"{GetGameName()} 시작!");
        }
        
        /// <summary>
        /// 게임 종료 및 결과 처리
        /// </summary>
        protected virtual void EndGame()
        {
            if (!isGameActive) return;
            
            isGameActive = false;
            
            // 결과 계산
            currentResult = CalculateResult();
            
            // 결과 저장
            MiniGameResultManager.SaveResult(currentResult);
            
            Debug.Log($"{GetGameName()} 종료! 결과: {currentResult}");
            
            // 이전 씬으로 돌아가기
            ReturnToPreviousScene();
        }
        
        /// <summary>
        /// 게임 결과 계산 (각 미니게임에서 구현)
        /// </summary>
        protected abstract MiniGameResult CalculateResult();
        
        /// <summary>
        /// 게임 이름 반환 (각 미니게임에서 구현)
        /// </summary>
        protected abstract string GetGameName();
        
        /// <summary>
        /// 이전 씬으로 돌아가기
        /// </summary>
        protected void ReturnToPreviousScene()
        {
            string previousScene = PlayerPrefs.GetString("PreviousScene", "room");
            SceneManager.LoadScene(previousScene);
        }
        
        /// <summary>
        /// 남은 시간 반환
        /// </summary>
        protected float GetRemainingTime()
        {
            return Mathf.Max(0f, gameTimeLimit - elapsedTime);
        }
        
        /// <summary>
        /// 진행률 반환 (0~1)
        /// </summary>
        protected float GetProgress()
        {
            return Mathf.Clamp01(elapsedTime / gameTimeLimit);
        }
    }
}