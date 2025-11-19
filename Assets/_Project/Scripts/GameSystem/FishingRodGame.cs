using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using PerfectButler.GameSystem;

namespace PerfectButler.MiniGames
{
    /// 낚싯대 흔들기 미니게임
    /// 마우스를 좌우로 빠르게 움직여 점수를 얻는 게임
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
        [SerializeField] private float shakeThreshold = 10f; // 흔들기 감지 임계값 (픽셀 단위)
        [SerializeField] private float shakeCooldown = 0.2f; // 흔들기 인정 쿨다운 (초)
        [SerializeField] private int scorePerShake = 1; // 한 번 흔들 때마다 얻는 점수

        [Header("Fishing Rod Movement")]
        [SerializeField] private float rodMoveSpeed = 5f; // 낚싯대 이동 속도
        [SerializeField] private float rodMoveRange = 200f; // 낚싯대 이동 가능 범위 (픽셀)

        private int currentScore = 0;
        private Vector3 lastMousePosition;
        private float mouseMoveDelta = 0f;
        private float lastShakeTime = 0f; // 마지막으로 점수를 얻은 시간
        private Vector3 initialRodPosition; // 낚싯대 초기 위치
        
        protected override void Start()
        {
            base.Start();
            lastMousePosition = Mouse.current.position.ReadValue();

            // 낚싯대 초기 위치 저장
            if (fishingRodImage != null)
            {
                initialRodPosition = fishingRodImage.transform.localPosition;
            }

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
        
        /// 마우스 흔들기 감지 및 점수 증가
        private void DetectMouseShake()
        {
            Vector3 currentMousePosition = Mouse.current.position.ReadValue();

            // 좌우 이동 거리 계산
            float deltaX = currentMousePosition.x - lastMousePosition.x;
            float absDeltaX = Mathf.Abs(deltaX);
            mouseMoveDelta = absDeltaX;

            // 낚싯대를 마우스 움직임에 따라 좌우로 이동
            if (fishingRodImage != null)
            {
                // 현재 위치에서 목표 위치 계산 (마우스 이동 방향과 속도 반영)
                float targetOffsetX = Mathf.Clamp(deltaX * rodMoveSpeed, -rodMoveRange, rodMoveRange);
                Vector3 targetPosition = initialRodPosition + new Vector3(targetOffsetX, 0, 0);

                // 부드럽게 이동
                fishingRodImage.transform.localPosition = Vector3.Lerp(
                    fishingRodImage.transform.localPosition,
                    targetPosition,
                    Time.deltaTime * 10f
                );

                // 회전 효과도 추가 (움직이는 방향으로 살짝 기울임)
                float targetAngle = Mathf.Clamp(deltaX * 0.5f, -15f, 15f);
                fishingRodImage.transform.rotation = Quaternion.Lerp(
                    fishingRodImage.transform.rotation,
                    Quaternion.Euler(0, 0, -targetAngle),
                    Time.deltaTime * 10f
                );
            }

            // 임계값 이상 움직이고 쿨다운이 지났으면 점수 증가
            if (absDeltaX > shakeThreshold && Time.time - lastShakeTime >= shakeCooldown)
            {
                currentScore += scorePerShake;
                lastShakeTime = Time.time;
            }

            lastMousePosition = currentMousePosition;
        }
        
        /// UI 업데이트
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