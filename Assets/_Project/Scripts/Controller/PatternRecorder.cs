using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[System.Serializable]
public class MovementFrame
{
    public float time;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public bool isGrounded;
}

[System.Serializable]
public class MovementPattern
{
    public string patternName;
    public Vector3 startPosition;
    public List<MovementFrame> frames = new List<MovementFrame>();
    public float duration;
}

public class PatternRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    public float recordInterval = 0.02f; // 50fps로 녹화
    public KeyCode startRecordKey = KeyCode.F1;
    public KeyCode stopRecordKey = KeyCode.F2;
    public KeyCode savePatternKey = KeyCode.F3;
    
    [Header("Pattern Storage")]
    public List<MovementPattern> savedPatterns = new List<MovementPattern>();
    
    private bool isRecording = false;
    private MovementPattern currentPattern;
    private float recordTimer = 0f;
    private float startTime = 0f;
    private Rigidbody rb;
    private CatController_NewInput catController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        catController = GetComponent<CatController_NewInput>();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // F1: 녹화 시작
        if (kb.f1Key.wasPressedThisFrame && !isRecording)
        {
            StartRecording();
        }

        // F2: 녹화 중지
        if (kb.f2Key.wasPressedThisFrame && isRecording)
        {
            StopRecording();
        }

        // F3: 패턴 저장
        if (kb.f3Key.wasPressedThisFrame && currentPattern != null && !isRecording)
        {
            SavePattern();
        }

        // 녹화 중일 때 프레임 기록
        if (isRecording)
        {
            recordTimer += Time.deltaTime;
            if (recordTimer >= recordInterval)
            {
                RecordFrame();
                recordTimer = 0f;
            }
        }
    }

    void StartRecording()
    {
        isRecording = true;
        startTime = Time.time;
        recordTimer = 0f;

        currentPattern = new MovementPattern
        {
            patternName = $"Pattern_{savedPatterns.Count + 1}",
            startPosition = transform.position,
            frames = new List<MovementFrame>()
        };

        Debug.Log($"<color=green>녹화 시작!</color> 시작 위치: {transform.position}");
    }

    void RecordFrame()
    {
        MovementFrame frame = new MovementFrame
        {
            time = Time.time - startTime,
            position = transform.position,
            rotation = transform.rotation,
            velocity = rb.linearVelocity,
            isGrounded = IsGrounded()
        };

        currentPattern.frames.Add(frame);
    }

    void StopRecording()
    {
        if (currentPattern == null || currentPattern.frames.Count == 0)
        {
            Debug.LogWarning("녹화된 프레임이 없습니다!");
            isRecording = false;
            return;
        }

        isRecording = false;
        currentPattern.duration = Time.time - startTime;

        Debug.Log($"<color=yellow>녹화 중지!</color> 총 {currentPattern.frames.Count}프레임, {currentPattern.duration:F2}초");
        Debug.Log("F3을 눌러 패턴을 저장하세요.");
    }

    void SavePattern()
    {
        if (currentPattern == null)
        {
            Debug.LogWarning("저장할 패턴이 없습니다!");
            return;
        }

        savedPatterns.Add(currentPattern);
        Debug.Log($"<color=cyan>패턴 저장 완료!</color> 이름: {currentPattern.patternName}, 시작 위치: {currentPattern.startPosition}");
        Debug.Log($"현재 저장된 패턴 수: {savedPatterns.Count}");
        
        currentPattern = null;
    }

    bool IsGrounded()
    {
        // CatController의 지면 체크 로직 재사용
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f);
    }

    // 디버그용: 현재 저장된 패턴 목록 출력
    [ContextMenu("Show Saved Patterns")]
    void ShowSavedPatterns()
    {
        Debug.Log($"=== 저장된 패턴 목록 ({savedPatterns.Count}개) ===");
        for (int i = 0; i < savedPatterns.Count; i++)
        {
            var pattern = savedPatterns[i];
            Debug.Log($"{i + 1}. {pattern.patternName} - 시작위치: {pattern.startPosition}, 프레임: {pattern.frames.Count}, 시간: {pattern.duration:F2}초");
        }
    }
}
