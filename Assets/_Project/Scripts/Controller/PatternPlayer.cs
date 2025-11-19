using UnityEngine;
using System.Collections;

public class PatternPlayer : MonoBehaviour
{
    [Header("Playback Settings")]
    public bool usePhysics = true; // true: Rigidbody 사용, false: Transform 직접 조작
    public float playbackSpeed = 1.0f;
    
    private Rigidbody rb;
    private bool isPlaying = false;
    private MovementPattern currentPlayingPattern;
    private float playbackTimer = 0f;
    private int currentFrameIndex = 0;
    private CatController_NewInput catController;

    public bool IsPlaying => isPlaying;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // CatController는 선택사항
        catController = GetComponent<CatController_NewInput>();
    }

    /// <summary>
    /// 패턴 재생 시작
    /// </summary>
    public void PlayPattern(MovementPattern pattern)
    {
        if (pattern == null || pattern.frames.Count == 0)
        {
            Debug.LogWarning("재생할 패턴이 비어있습니다!");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(PlayPatternCoroutine(pattern));
    }

    IEnumerator PlayPatternCoroutine(MovementPattern pattern)
    {
        isPlaying = true;
        currentPlayingPattern = pattern;
        playbackTimer = 0f;
        currentFrameIndex = 0;

        // CatController 비활성화 (수동 조작 방지)
        if (catController != null)
            catController.enabled = false;

        Debug.Log($"<color=green>패턴 재생 시작:</color> {pattern.patternName}");

        // 시작 위치로 이동
        transform.position = pattern.startPosition;

        while (currentFrameIndex < pattern.frames.Count)
        {
            playbackTimer += Time.deltaTime * playbackSpeed;

            // 현재 시간에 해당하는 프레임 찾기
            while (currentFrameIndex < pattern.frames.Count &&
                   pattern.frames[currentFrameIndex].time <= playbackTimer)
            {
                ApplyFrame(pattern.frames[currentFrameIndex]);
                currentFrameIndex++;
            }

            yield return null;
        }

        // 재생 완료
        isPlaying = false;
        currentPlayingPattern = null;

        // CatController 다시 활성화
        if (catController != null)
            catController.enabled = true;

        Debug.Log($"<color=cyan>패턴 재생 완료:</color> {pattern.patternName}");
    }

    void ApplyFrame(MovementFrame frame)
    {
        if (usePhysics && rb != null)
        {
            // Rigidbody를 사용한 물리 기반 재생
            rb.MovePosition(frame.position);
            rb.MoveRotation(frame.rotation);
            rb.linearVelocity = frame.velocity;
        }
        else
        {
            // Transform 직접 조작 (더 정확하지만 물리 무시)
            transform.position = frame.position;
            transform.rotation = frame.rotation;
        }
    }

    /// <summary>
    /// 재생 중지
    /// </summary>
    public void StopPlayback()
    {
        if (isPlaying)
        {
            StopAllCoroutines();
            isPlaying = false;
            currentPlayingPattern = null;

            if (catController != null)
                catController.enabled = true;

            Debug.Log("패턴 재생 중지");
        }
    }
}
