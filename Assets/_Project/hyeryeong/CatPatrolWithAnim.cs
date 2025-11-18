using UnityEngine;
using UnityEngine.AI;

// 이 스크립트에는 NavMeshAgent와 Animator 컴포넌트가 필요합니다.
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CatPatrolWithAnim : MonoBehaviour
{
    // Inspector에서 설정할 순찰 지점 목록
    [Header("Patrol Settings")]
    public Transform[] waypoints; 
    public float patrolSpeed = 2f;      // 이동 속도
    public float arrivalDistance = 0.5f; // 목표 지점 도착으로 간주할 거리

    private NavMeshAgent agent;
    private Animator animator;
    private int currentWaypointIndex = 0;
    private const string ANIM_SPEED_PARAM = "Speed"; // Animator의 Float 파라미터 이름

    void Start()
    {
        // 필수 컴포넌트 참조
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // NavMeshAgent 설정
        agent.speed = patrolSpeed;
        agent.autoBraking = true; // 자동으로 멈추게 설정

        // 순찰 시작
        if (waypoints != null && waypoints.Length > 0)
        {
            GotoNextWaypoint();
        }
        else
        {
            Debug.LogError("순찰 지점(Waypoints)이 설정되지 않았습니다! Inspector에서 Waypoints 배열에 지점을 추가해주세요.");
        }
    }

    void Update()
    {
        // 1. 순찰 로직
        // 목표 지점에 거의 도착했고, 경로 계산 중이 아니라면 다음 목표로 이동
        if (!agent.pathPending && agent.remainingDistance < arrivalDistance)
        {
            GotoNextWaypoint();
        }

        // 2. 애니메이션 제어
        // 현재 이동 속도(velocity)를 기반으로 Speed 파라미터를 업데이트하여 Idle/Walk 상태 전환
        float speed = agent.velocity.magnitude / patrolSpeed;
        animator.SetFloat(ANIM_SPEED_PARAM, speed);
    }

    void GotoNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        // 다음 지점 인덱스 계산 (순차적으로 순환)
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        
        // 새로운 목표 지점 설정
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }
}