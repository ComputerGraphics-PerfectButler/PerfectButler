using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // 코루틴 사용을 위해 필수적입니다.

public class MiniGameManager : MonoBehaviour
{
    // === 1. UI 및 게임 요소 연결 변수 ===
    
    [Header("UI Components")]
    public Button[] catButtons; 
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Button startButton; 

    [Header("Game Graphics")]
    // Inspector에 클릭 전/후 이미지를 연결하세요.
    public Sprite defaultMoleSprite; // 클릭 전 기본 이미지 (Mouse_enter.jpg)
    public Sprite hitMoleSprite;     // 클릭 후 맞은 이미지 (Mouse_out.jpg)
    
    // === 2. 게임 상태 및 설정 변수 ===
    
    private int score = 0;
    private float gameTime = 30f; 
    private bool isGameRunning = false;
    
    [Header("Game Settings")]
    public float minSpawnTime = 1.0f;
    public float maxSpawnTime = 2.0f;
    public float popUpDuration = 0.8f;
    private const float hitShowDuration = 0.3f; // 맞았을 때 이미지가 보이는 시간

    // ===================================================
    //                  게임 시작 및 타이머
    // ===================================================

    public void OnStartButtonClicked()
    {
        if (isGameRunning) return;

        // Null 체크: 필수 변수 연결 확인
        if (startButton == null || scoreText == null || timerText == null || catButtons == null || catButtons.Length == 0)
        {
            Debug.LogError("Error: 필수 UI 컴포넌트 또는 Cat Buttons 배열이 Inspector에 연결되지 않았습니다!");
            return;
        }

        // 1. 상태 초기화 및 변경
        score = 0;
        gameTime = 30f; 
        scoreText.text = "점수: 0";
        timerText.text = "남은 시간: 30";

        isGameRunning = true;
        
        // UX 개선: 시작 버튼 숨김
        startButton.gameObject.SetActive(false); 

        // 2. 코루틴 시작
        StartCoroutine(GameTimer());
        StartCoroutine(SpawnMoles());
        
        Debug.Log("미니게임이 시작되었습니다!");
    }

    private IEnumerator GameTimer()
    {
        while (gameTime > 0)
        {
            yield return new WaitForSeconds(1f);
            gameTime -= 1f;
            timerText.text = "남은 시간: " + Mathf.CeilToInt(gameTime).ToString();
        }
        
        // 3. 게임 종료
        isGameRunning = false;
        timerText.text = "게임 종료!";
        
        // 게임 종료 후 버튼 다시 보이게 함
        if (startButton != null) startButton.gameObject.SetActive(true); 
        
        StopAllMoles();
    }
    
    // ===================================================
    //                  고양이 생성 로직
    // ===================================================

    private IEnumerator SpawnMoles()
    {
        while (isGameRunning)
        {
            // 1. 무작위 대기 시간 설정
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (!isGameRunning) break;

            // 2. 랜덤으로 고양이 버튼 선택
            int randomIndex = Random.Range(0, catButtons.Length);
            Button selectedButton = catButtons[randomIndex];

            // 3. 고양이가 현재 숨어있고, 버튼이 null이 아닐 때 활성화
            if (selectedButton != null && !selectedButton.gameObject.activeInHierarchy)
            {
                // 고양이 활성화 (튀어나오게 함)
                selectedButton.gameObject.SetActive(true);
                
                // 튀어나올 때 기본 이미지로 설정
                if (selectedButton.image != null && defaultMoleSprite != null)
                {
                    selectedButton.image.sprite = defaultMoleSprite;
                }
                
                // 4. 고양이가 팝업 지속 시간만큼만 보이도록 대기
                yield return new WaitForSeconds(popUpDuration); 

                // 5. 다시 비활성화 (숨기기)
                selectedButton.gameObject.SetActive(false);
            }
        }
    }
    
    // ===================================================
    //                     점수 및 처리
    // ===================================================

    // 각 고양이 버튼의 On Click()에 연결될 함수입니다.
    public void HitMole(GameObject moleObject)
    {
        if (!isGameRunning) return;

        score += 10;
        if (scoreText != null) scoreText.text = "점수: " + score;
        
        // --- 클릭 시 이미지 변경 로직 ---
        Button clickedButton = moleObject.GetComponent<Button>();
        if (clickedButton != null && hitMoleSprite != null)
        {
            // 1. 클릭 후 이미지로 변경
            clickedButton.image.sprite = hitMoleSprite; 
            
            // 2. 점수 증가 후 0.3초간 맞은 이미지를 보여주고 숨김
            StartCoroutine(HideMoleAfterHit(clickedButton));
        }
    }

    // 맞은 후 잠시 이미지를 보여주고 숨기는 코루틴
    private IEnumerator HideMoleAfterHit(Button hitButton)
    {
        // 0.3초 동안 맞은 이미지 보여주기
        yield return new WaitForSeconds(hitShowDuration); 
        
        // 버튼 숨기기
        hitButton.gameObject.SetActive(false);
        
        // (선택 사항) 다음 팝업을 위해 기본 이미지로 미리 되돌릴 필요는 없음
        // 왜냐하면 SpawnMoles에서 팝업 시 기본 이미지로 다시 설정하기 때문
    }

    private void StopAllMoles()
    {
        foreach (Button button in catButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}