using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using PerfectButler.GameSystem;

namespace PerfectButler.MiniGames
{
    /// <summary>
    /// 낚싯대 흔들기 미니게임
    /// 마우스를 좌우로 빠르게 움직여 점수를 얻는 게임
    /// </summary>
    public class FishingRodGame : MiniGameBase
    {
        [Header("Game UI")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Image fishingRodImage;
        
        [Header("Game Balance")]
        [SerializeField] private int perfectScore = 100;
        [SerializeField] private int normalScore = 50;
        [SerializeField] private float shakeThreshold = 3f; // 흔들기 감지 임계값
        
        private int currentScore = 0;
        private Vector3 lastMousePosition;
        private float mouseMoveDelta = 0f;
        
        protected override void Start()
        {
            base.Start();
            lastMousePosition = Mouse.current.position.ReadValue();
            UpdateUI();
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (!isGameActive) return;
            
            // 마우스 움직임 감지
            DetectMouseShake();
            
            // UI 업데이트
            UpdateUI();
        }
        
        /// <summary>
        /// 마우스 흔들기 감지 및 점수 증가
        /// </summary>
        private void DetectMouseShake()
        {
            Vector3 currentMousePosition = Mouse.current.position.ReadValue();
            
            // 좌우 이동 거리 계산
            float deltaX = Mathf.Abs(currentMousePosition.x - lastMousePosition.x);
            mouseMoveDelta = deltaX;
            
            // 임계값 이상 움직이면 점수 증가
            if (deltaX > shakeThreshold)
            {
                currentScore++;
                
                // 낚싯대 이미지 흔들림 효과 (옵션)
                if (fishingRodImage != null)
                {
                    float randomAngle = Random.Range(-10f, 10f);
                    fishingRodImage.transform.rotation = Quaternion.Euler(0, 0, randomAngle);
                }
            }
            else
            {
                // 이미지 원위치
                if (fishingRodImage != null)
                {
                    fishingRodImage.transform.rotation = Quaternion.Lerp(
                        fishingRodImage.transform.rotation, 
                        Quaternion.identity, 
                        Time.deltaTime * 10f
                    );
                }
            }
            
            lastMousePosition = currentMousePosition;
        }
        
        /// <summary>
        /// UI 업데이트
        /// </summary>
        private void UpdateUI()
        {
            // 점수 표시
            if (scoreText != null)
                scoreText.text = $"점수: {currentScore}";
            
            // 시간 표시
            if (timeText != null)
                timeText.text = $"남은 시간: {GetRemainingTime():F0}초";
            
            // 진행도 바
            if (progressBar != null)
                progressBar.value = GetProgress();
        }
        
        protected override MiniGameResult CalculateResult()
        {
            if (currentScore >= perfectScore)
                return MiniGameResult.Perfect;
            else if (currentScore >= normalScore)
                return MiniGameResult.Normal;
            else
                return MiniGameResult.Fail;
        }
        
        protected override string GetGameName()
        {
            return "낚싯대 흔들기";
        }
    }
}