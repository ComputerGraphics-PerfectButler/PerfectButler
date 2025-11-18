using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CatWander : MonoBehaviour
{
    [Header("Wander Settings")]
    public Transform wanderCenter; // 맴돌기 중심점 (예: WayPoint1)
    public float wanderRadius = 3f;  // 이 반경(미터) 내에서 맴돕니다.
    public float wanderTimer = 5f;   // 몇 초마다 새로운 위치로 갈지

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;

    private const string ANIM_SPEED_PARAM = "Speed"; // Animator의 Float 파라미터 이름

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // 주의: Animator가 자식에 있다면 GetComponentInChildren<Animator>(); 로 바꿔주세요.
        animator = GetComponentInChildren<Animator>(); 
        if (animator == null)
        {
            Debug.LogError("Animator를 찾을 수 없습니다!", this.gameObject);
        }

        timer = wanderTimer;
    }

    void Update()
    {
        // 1. 타이머 작동
        timer += Time.deltaTime;

        // 2. 타이머가 다 되면 새로운 무작위 위치로 이동
        if (timer >= wanderTimer)
        {
            // 중심점(wanderCenter) 주변의 무작위 위치 계산
            Vector3 newPos = GetRandomNavLocation(wanderCenter.position, wanderRadius);
            
            // NavMeshAgent에 새로운 목표 지점 설정
            agent.SetDestination(newPos);
            
            // 타이머 초기화
            timer = 0;
        }

        // 3. 애니메이션 제어 (이전과 동일)
        if (animator != null)
        {
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat(ANIM_SPEED_PARAM, speed);
        }
    }

    // NavMesh 위에서 랜덤 위치를 찾는 함수
    public static Vector3 GetRandomNavLocation(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);
        
        return navHit.position;
    }
}