using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using PerfectButler.GameSystem;

namespace PerfectButler.MiniGames
{
    /// 두더지잡기 미니게임
    /// 튀어나오는 고양이를 클릭하여 점수를 얻는 게임
    public class WhackAMoleGame : MiniGameBase
    {
        [Header("Game UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;

        [Header("Mole Settings")]
        [SerializeField] private Button[] moleButtons;
        [SerializeField] private Sprite defaultMoleSprite; // 클릭 전 기본 이미지
        [SerializeField] private Sprite hitMoleSprite;     // 클릭 후 맞은 이미지

        [Header("Game Balance")]
        [SerializeField] private int perfectScore = 100;
        [SerializeField] private int normalScore = 50;
        [SerializeField] private int scorePerHit = 10;

        [Header("Spawn Settings")]
        [SerializeField] private float minSpawnTime = 1.0f;
        [SerializeField] private float maxSpawnTime = 2.0f;
        [SerializeField] private float popUpDuration = 0.8f;
        [SerializeField] private float hitShowDuration = 0.3f; // 맞았을 때 이미지가 보이는 시간

        private int currentScore = 0;
        private Coroutine spawnCoroutine;

        protected override void Start()
        {
            base.Start();

            // 모든 두더지 숨기기
            HideAllMoles();

            // UI 초기화
            UpdateUI();
        }

        protected override void Update()
        {
            base.Update();

            if (!isGameActive) return;

            // UI 업데이트
            UpdateUI();
        }

        protected override void StartGame()
        {
            base.StartGame();

            // 점수 초기화
            currentScore = 0;

            // 모든 두더지 숨기기
            HideAllMoles();

            // 두더지 스폰 시작
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
            spawnCoroutine = StartCoroutine(SpawnMoles());

            UpdateUI();
        }

        protected override void EndGame()
        {
            // 스폰 중지
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }

            // 모든 두더지 숨기기
            HideAllMoles();

            base.EndGame();
        }

        /// 두더지 스폰 코루틴
        private IEnumerator SpawnMoles()
        {
            while (isGameActive)
            {
                // 무작위 대기 시간
                float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
                yield return new WaitForSeconds(waitTime);

                if (!isGameActive) break;

                // 랜덤으로 두더지 버튼 선택
                int randomIndex = Random.Range(0, moleButtons.Length);
                Button selectedButton = moleButtons[randomIndex];

                // 두더지가 현재 숨어있을 때만 활성화
                if (selectedButton != null && !selectedButton.gameObject.activeInHierarchy)
                {
                    // 두더지 활성화
                    selectedButton.gameObject.SetActive(true);

                    // 기본 이미지로 설정
                    if (selectedButton.image != null && defaultMoleSprite != null)
                    {
                        selectedButton.image.sprite = defaultMoleSprite;
                    }

                    // 일정 시간 후 자동으로 숨기기
                    StartCoroutine(HideMoleAfterDelay(selectedButton, popUpDuration));
                }
            }
        }

        /// 일정 시간 후 두더지 숨기기
        private IEnumerator HideMoleAfterDelay(Button moleButton, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (moleButton != null && moleButton.gameObject.activeInHierarchy)
            {
                moleButton.gameObject.SetActive(false);
            }
        }

        /// 두더지 클릭 처리 (각 버튼의 OnClick에 연결)
        public void OnMoleClicked(GameObject moleObject)
        {
            if (!isGameActive) return;

            Button clickedButton = moleObject.GetComponent<Button>();
            if (clickedButton == null) return;

            // 점수 증가
            currentScore += scorePerHit;
            UpdateUI();

            // 클릭 이미지로 변경
            if (clickedButton.image != null && hitMoleSprite != null)
            {
                clickedButton.image.sprite = hitMoleSprite;
            }

            // 잠시 후 숨기기
            StartCoroutine(HideMoleAfterHit(clickedButton));
        }

        /// 맞은 후 잠시 이미지를 보여주고 숨기기
        private IEnumerator HideMoleAfterHit(Button hitButton)
        {
            yield return new WaitForSeconds(hitShowDuration);

            if (hitButton != null)
            {
                hitButton.gameObject.SetActive(false);
            }
        }

        /// 모든 두더지 숨기기
        private void HideAllMoles()
        {
            if (moleButtons == null) return;

            foreach (Button button in moleButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        /// UI 업데이트
        private void UpdateUI()
        {
            if (scoreText != null)
                scoreText.text = $"점수: {currentScore}";

            if (timeText != null)
                timeText.text = $"남은 시간: {GetRemainingTime():F0}초";
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
            return "두더지잡기";
        }
    }
}
