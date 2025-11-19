using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(PatternPlayer))]
[RequireComponent(typeof(PatternDataManager))]
public class AutoCatController : MonoBehaviour
{
    [Header("Auto Mode Settings")]
    public bool autoModeEnabled = false;
    public float startDelay = 5f; // 자동 모드 시작 전 대기 시간
    
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float arrivalDistance = 0.3f;
    
    [Header("Timing")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    
    [Header("Navigation")]
    public bool useNavMeshAgent = false;
    
    // 패턴 직접 저장 (PatternRecorder 의존성 제거)
    [HideInInspector]
    public List<MovementPattern> loadedPatterns = new List<MovementPattern>();
    
    private PatternPlayer player;
    private PatternDataManager dataManager;
    private Rigidbody rb;
    private NavMeshAgent navAgent;
    private CatController_NewInput catController;
    
    private enum AutoState
    {
        Idle,
        MovingToStart,
        PlayingPattern
    }
    
    private AutoState currentState = AutoState.Idle;
    private Vector3 targetPosition;
    private MovementPattern selectedPattern;

    void Start()
    {
        Debug.Log("=== AutoCatController Start ===");
        
        player = GetComponent<PatternPlayer>();
        dataManager = GetComponent<PatternDataManager>();
        rb = GetComponent<Rigidbody>();
        
        // CatController는 선택사항
        catController = GetComponent<CatController_NewInput>();
        if (catController == null)
        {
            Debug.Log("CatController_NewInput이 없습니다. 자동 모드 전용으로 실행됩니다.");
        }
        
        // NavMeshAgent 확인
        navAgent = GetComponent<NavMeshAgent>();
        if (useNavMeshAgent && navAgent == null)
        {
            Debug.LogWarning("NavMeshAgent가 없습니다!");
            useNavMeshAgent = false;
        }
        
        if (useNavMeshAgent && navAgent != null)
        {
            navAgent.enabled = false;
        }

        // 자동 모드 시작
        if (autoModeEnabled)
        {
            StartCoroutine(InitializeAndStart());
        }
    }

    IEnumerator InitializeAndStart()
    {
        Debug.Log($"<color=yellow>{startDelay}초 후 자동 모드 시작...</color>");
        
        // 설정된 시간만큼 대기 (고양이가 땅에 떨어질 시간)
        yield return new WaitForSeconds(startDelay);
        
        Debug.Log("초기화 시작...");
        
        // 패턴 불러오기
        if (dataManager != null)
        {
            loadedPatterns = dataManager.LoadAndGetPatterns();
            
            if (loadedPatterns == null || loadedPatterns.Count == 0)
            {
                Debug.LogError("패턴 로딩 실패! 패턴이 없습니다.");
                autoModeEnabled = false;
                yield break;
            }
            
            Debug.Log($"<color=green>패턴 로딩 성공! 총 {loadedPatterns.Count}개</color>");
        }
        else
        {
            Debug.LogError("PatternDataManager가 없습니다!");
            yield break;
        }

        // CatController 비활성화
        if (catController != null)
            catController.enabled = false;

        Debug.Log("<color=green>자동 모드 시작!</color>");
        StartCoroutine(AutoBehaviorLoop());
    }

    void Update()
    {
        if (!autoModeEnabled) return;

        if (currentState == AutoState.MovingToStart && !useNavMeshAgent)
        {
            MoveTowardsTarget();
        }
    }

    IEnumerator AutoBehaviorLoop()
    {
        while (autoModeEnabled)
        {
            // 1. 랜덤 패턴 선택
            selectedPattern = GetRandomPattern();
            if (selectedPattern == null)
            {
                Debug.LogError("패턴을 선택할 수 없습니다!");
                yield break;
            }

            Debug.Log($"<color=cyan>━━━━━━━━━━━━━━━━━━━━</color>");
            Debug.Log($"<color=cyan>선택된 패턴:</color> {selectedPattern.patternName}");
            Debug.Log($"<color=cyan>시작 위치:</color> {selectedPattern.startPosition}");
            Debug.Log($"<color=cyan>현재 위치:</color> {transform.position}");

            // 2. 시작 위치로 이동 (Y 좌표 보정)
            currentState = AutoState.MovingToStart;
            targetPosition = selectedPattern.startPosition;
            
            // Y 좌표를 현재 고양이의 Y로 보정 (땅 아래로 안가게)
            targetPosition.y = transform.position.y;
            
            Debug.Log($"<color=yellow>→ 이동 시작... (보정된 위치: {targetPosition})</color>");
            yield return StartCoroutine(MoveToPosition(targetPosition));
            Debug.Log($"<color=green>✓ 도착 완료!</color>");

            // 3. 패턴 재생
            currentState = AutoState.PlayingPattern;
            Debug.Log($"<color=magenta>▶ 패턴 재생 시작...</color>");
            player.PlayPattern(selectedPattern);
            
            // 패턴 재생 완료 대기
            yield return new WaitWhile(() => player.IsPlaying);
            Debug.Log($"<color=green>✓ 패턴 재생 완료!</color>");

            // 4. 잠시 대기
            currentState = AutoState.Idle;
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            Debug.Log($"<color=yellow>💤 대기 중... {idleTime:F1}초</color>");
            yield return new WaitForSeconds(idleTime);
        }
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        if (useNavMeshAgent && navAgent != null)
        {
            navAgent.enabled = true;
            navAgent.speed = moveSpeed;
            navAgent.SetDestination(target);

            while (navAgent.enabled && navAgent.remainingDistance > arrivalDistance)
            {
                yield return null;
            }

            navAgent.enabled = false;
        }
        else
        {
            while (Vector3.Distance(transform.position, target) > arrivalDistance)
            {
                yield return null;
            }
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            Vector3 moveVelocity = direction * moveSpeed;
            moveVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = moveVelocity;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        }

        if (Vector3.Distance(transform.position, targetPosition) <= arrivalDistance)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    MovementPattern GetRandomPattern()
    {
        if (loadedPatterns == null || loadedPatterns.Count == 0)
            return null;

        int randomIndex = Random.Range(0, loadedPatterns.Count);
        return loadedPatterns[randomIndex];
    }

    void OnDrawGizmos()
    {
        if (loadedPatterns == null) return;

        Gizmos.color = Color.yellow;
        foreach (var pattern in loadedPatterns)
        {
            Gizmos.DrawWireSphere(pattern.startPosition, 0.3f);
            Gizmos.DrawLine(pattern.startPosition, pattern.startPosition + Vector3.up * 2f);
        }

        if (autoModeEnabled && currentState == AutoState.MovingToStart)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }
    }
}
