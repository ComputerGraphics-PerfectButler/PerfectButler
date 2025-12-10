using UnityEngine;

public class CatWanderAI : MonoBehaviour
{
    [Header("Settings")]
    public Transform catModel;
    public float wanderRadius = 1.5f;
    public float moveSpeed = 0.3f;
    public float rotateSpeed = 2f;
    
    [Header("Cat Info")]
    public CatItem preferredItem = CatItem.Toy1;
    public string catName = "고양이";
    public Material catMaterial; // 고양이 Material

    [Header("Follow")]
    public float followDistance = 2f;  // 이 거리보다 멀면 따라감
    public float stopDistance = 1.5f;  // ✅ 이 거리보다 가까우면 멈춤 (떨림 방지)
    public float followSpeed = 2f;

    private Vector3 startPosition;
    private Quaternion initialModelRotation;
    private float angle = 0f;
    private bool isFollowing = false;
    private Transform owner;
    private Rigidbody rb;

    void Start()
    {
        startPosition = transform.position;

        if (startPosition.y < 0.1f)
        {
            startPosition.y = 0.5f;
            transform.position = startPosition;
        }

        // 모델 자동 찾기
        if (catModel == null && transform.childCount > 0)
        {
            catModel = transform.GetChild(0);
        }

        // 모델의 초기 로컬 회전값 저장
        if (catModel != null)
        {
            initialModelRotation = catModel.localRotation;

            // Material 자동 감지
            Renderer renderer = catModel.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                catMaterial = renderer.sharedMaterial;
            }
        }

        angle = Random.Range(0f, 360f);
        
        // ✅ Rigidbody 설정
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | 
                         RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationZ;
        rb.mass = 5f;
        rb.linearDamping = 5f;
        rb.angularDamping = 5f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // ✅ Physics Material (튕김 방지)
        PhysicsMaterial catPhysics = new PhysicsMaterial("CatPhysics");
        catPhysics.bounciness = 0f;
        catPhysics.dynamicFriction = 0.8f;
        catPhysics.staticFriction = 0.8f;
        catPhysics.frictionCombine = PhysicsMaterialCombine.Maximum;
        catPhysics.bounceCombine = PhysicsMaterialCombine.Minimum;
        
        // ✅ Collider 설정
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
            capsule.radius = 0.3f;
            capsule.height = 1f;
            capsule.center = new Vector3(0, 0.5f, 0);
            capsule.material = catPhysics;
        }
        else
        {
            col.isTrigger = false;
            col.material = catPhysics;
        }
        
        Debug.Log($"[{catName}] 배회 시작");
    }

    void Update()
    {
        // ✅ Update에서 처리 (FixedUpdate는 물리 계산용)
        if (isFollowing)
        {
            FollowOwner();
        }
        else
        {
            WanderInCircle();
        }
    }

    void WanderInCircle()
    {
        // 원 궤도 각도 증가
        angle += moveSpeed * 20f * Time.deltaTime;

        // 목표 위치 계산
        float x = startPosition.x + Mathf.Cos(angle * Mathf.Deg2Rad) * wanderRadius;
        float z = startPosition.z + Mathf.Sin(angle * Mathf.Deg2Rad) * wanderRadius;
        Vector3 targetPos = new Vector3(x, transform.position.y, z);
        
        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed * 2f);

        // 이동 방향으로 회전
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;

        if (direction.magnitude > 0.01f)
        {
            RotateTowards(direction);
        }
    }

    void FollowOwner()
    {
        if (owner == null) return;

        float distance = Vector3.Distance(transform.position, owner.position);

        // ✅ stopDistance보다 가까우면 멈춤 (떨림 방지)
        if (distance <= stopDistance)
        {
            return; // 아무것도 안 함
        }

        // ✅ followDistance보다 멀면 따라감
        if (distance > followDistance)
        {
            Vector3 direction = (owner.position - transform.position).normalized;
            direction.y = 0; // 수평 방향만

            // 직선으로 이동
            Vector3 newPosition = transform.position + direction * followSpeed * Time.deltaTime;
            
            // ✅ Y축 높이를 플레이어 높이에 맞춤 (길 높이 적응)
            newPosition.y = Mathf.Lerp(transform.position.y, owner.position.y, Time.deltaTime * 3f);
            
            transform.position = newPosition;

            // 플레이어 방향 바라보기
            RotateTowards(direction);
        }
    }

    // ✅ 회전 처리 (공통 함수)
    void RotateTowards(Vector3 direction)
    {
        if (catModel != null)
        {
            // 타겟 Y 회전값
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float targetY = targetRotation.eulerAngles.y;
            
            // 현재 Y 회전값
            float currentY = catModel.localEulerAngles.y;
            
            // 부드럽게 회전
            float newY = Mathf.LerpAngle(currentY, targetY, Time.deltaTime * rotateSpeed);
            
            // X 회전은 초기값 유지
            Vector3 initialEuler = initialModelRotation.eulerAngles;
            catModel.localRotation = Quaternion.Euler(initialEuler.x, newY, initialEuler.z);
        }
        else
        {
            // catModel이 없으면 부모 오브젝트 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }

    public bool TryAcceptItem(CatItem item, Transform player)
    {
        if (item == preferredItem)
        {
            Debug.Log($"[{catName}] {item}을(를) 좋아해요! 따라갑니다.");
            isFollowing = true;
            owner = player;
            return true;
        }
        else
        {
            Debug.Log($"[{catName}] {item}은(는) 싫어요... (좋아하는 건: {preferredItem})");
            return false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        
        // 배회 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, wanderRadius);
        
        // 시작 지점
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(center, 0.1f);
        
        if (isFollowing && owner != null)
        {
            // 멈추는 거리 (파란색)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(owner.position, stopDistance);
            
            // 따라가는 거리 (빨간색)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(owner.position, followDistance);
        }
    }
}

// using UnityEngine;

// public class CatWanderAI : MonoBehaviour
// {
//     [Header("Settings")]
//     public Transform catModel;
//     public float wanderRadius = 1.5f;
//     public float moveSpeed = 0.3f;
//     public float rotateSpeed = 2f;
    
//     [Header("Cat Info")]
//     public CatItem preferredItem = CatItem.Toy1;
//     public string catName = "고양이";
    
//     [Header("Follow")]
//     public float followDistance = 2f;  // 이 거리보다 멀면 따라감
//     public float stopDistance = 1.5f;  // ✅ 이 거리보다 가까우면 멈춤 (떨림 방지)
//     public float followSpeed = 2f;

//     private Vector3 startPosition;
//     private Quaternion initialModelRotation;
//     private float angle = 0f;
//     private bool isFollowing = false;
//     private Transform owner;
//     private Rigidbody rb;

//     void Start()
//     {
//         startPosition = transform.position;
        
//         if (startPosition.y < 0.1f)
//         {
//             startPosition.y = 0.5f;
//             transform.position = startPosition;
//         }

//         // 모델 자동 찾기
//         if (catModel == null && transform.childCount > 0)
//         {
//             catModel = transform.GetChild(0);
//         }

//         // 모델의 초기 로컬 회전값 저장
//         if (catModel != null)
//         {
//             initialModelRotation = catModel.localRotation;
//         }

//         angle = Random.Range(0f, 360f);
        
//         // ✅ Rigidbody 설정
//         rb = GetComponent<Rigidbody>();
//         if (rb == null)
//         {
//             rb = gameObject.AddComponent<Rigidbody>();
//         }
        
//         rb.useGravity = false;
//         rb.isKinematic = false;
//         rb.constraints = RigidbodyConstraints.FreezePositionY | 
//                          RigidbodyConstraints.FreezeRotationX | 
//                          RigidbodyConstraints.FreezeRotationZ;
//         rb.mass = 5f;
//         rb.linearDamping = 5f;
//         rb.angularDamping = 5f;
//         rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
//         // ✅ Physics Material (튕김 방지)
//         PhysicsMaterial catPhysics = new PhysicsMaterial("CatPhysics");
//         catPhysics.bounciness = 0f;
//         catPhysics.dynamicFriction = 0.8f;
//         catPhysics.staticFriction = 0.8f;
//         catPhysics.frictionCombine = PhysicsMaterialCombine.Maximum;
//         catPhysics.bounceCombine = PhysicsMaterialCombine.Minimum;
        
//         // ✅ Collider 설정
//         Collider col = GetComponent<Collider>();
//         if (col == null)
//         {
//             CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
//             capsule.radius = 0.3f;
//             capsule.height = 1f;
//             capsule.center = new Vector3(0, 0.5f, 0);
//             capsule.material = catPhysics;
//         }
//         else
//         {
//             col.isTrigger = false;
//             col.material = catPhysics;
//         }
        
//         Debug.Log($"[{catName}] 배회 시작");
//     }

//     void Update()
//     {
//         // ✅ Update에서 처리 (FixedUpdate는 물리 계산용)
//         if (isFollowing)
//         {
//             FollowOwner();
//         }
//         else
//         {
//             WanderInCircle();
//         }
//     }

//     void WanderInCircle()
//     {
//         // 원 궤도 각도 증가
//         angle += moveSpeed * 20f * Time.deltaTime;

//         // 목표 위치 계산
//         float x = startPosition.x + Mathf.Cos(angle * Mathf.Deg2Rad) * wanderRadius;
//         float z = startPosition.z + Mathf.Sin(angle * Mathf.Deg2Rad) * wanderRadius;
//         Vector3 targetPos = new Vector3(x, transform.position.y, z);
        
//         // 부드럽게 이동
//         transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed * 2f);

//         // 이동 방향으로 회전
//         Vector3 direction = (targetPos - transform.position).normalized;
//         direction.y = 0;

//         if (direction.magnitude > 0.01f)
//         {
//             RotateTowards(direction);
//         }
//     }

//     void FollowOwner()
//     {
//         if (owner == null) return;

//         float distance = Vector3.Distance(transform.position, owner.position);

//         // ✅ stopDistance보다 가까우면 멈춤 (떨림 방지)
//         if (distance <= stopDistance)
//         {
//             return; // 아무것도 안 함
//         }

//         // ✅ followDistance보다 멀면 따라감
//         if (distance > followDistance)
//         {
//             Vector3 direction = (owner.position - transform.position).normalized;
//             direction.y = 0;

//             // 직선으로 이동
//             transform.position += direction * followSpeed * Time.deltaTime;

//             // 플레이어 방향 바라보기
//             RotateTowards(direction);
//         }
//     }

//     // ✅ 회전 처리 (공통 함수)
//     void RotateTowards(Vector3 direction)
//     {
//         if (catModel != null)
//         {
//             // 타겟 Y 회전값
//             Quaternion targetRotation = Quaternion.LookRotation(direction);
//             float targetY = targetRotation.eulerAngles.y;
            
//             // 현재 Y 회전값
//             float currentY = catModel.localEulerAngles.y;
            
//             // 부드럽게 회전
//             float newY = Mathf.LerpAngle(currentY, targetY, Time.deltaTime * rotateSpeed);
            
//             // X 회전은 초기값 유지
//             Vector3 initialEuler = initialModelRotation.eulerAngles;
//             catModel.localRotation = Quaternion.Euler(initialEuler.x, newY, initialEuler.z);
//         }
//         else
//         {
//             // catModel이 없으면 부모 오브젝트 회전
//             Quaternion targetRotation = Quaternion.LookRotation(direction);
//             transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
//         }
//     }

//     public bool TryAcceptItem(CatItem item, Transform player)
//     {
//         if (item == preferredItem)
//         {
//             Debug.Log($"[{catName}] {item}을(를) 좋아해요! 따라갑니다.");
//             isFollowing = true;
//             owner = player;
//             return true;
//         }
//         else
//         {
//             Debug.Log($"[{catName}] {item}은(는) 싫어요... (좋아하는 건: {preferredItem})");
//             return false;
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         Vector3 center = Application.isPlaying ? startPosition : transform.position;
        
//         // 배회 범위
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(center, wanderRadius);
        
//         // 시작 지점
//         Gizmos.color = Color.green;
//         Gizmos.DrawSphere(center, 0.1f);
        
//         if (isFollowing && owner != null)
//         {
//             // 멈추는 거리 (파란색)
//             Gizmos.color = Color.blue;
//             Gizmos.DrawWireSphere(owner.position, stopDistance);
            
//             // 따라가는 거리 (빨간색)
//             Gizmos.color = Color.red;
//             Gizmos.DrawWireSphere(owner.position, followDistance);
//         }
//     }
// }

// using UnityEngine;

// public class CatWanderAI : MonoBehaviour
// {
//     [Header("Settings")]
//     public Transform catModel;
//     public float wanderRadius = 1.5f;
//     public float moveSpeed = 0.3f;
//     public float rotateSpeed = 2f;
    
//     [Header("Cat Info")]
//     public CatItem preferredItem = CatItem.Toy1;
//     public string catName = "고양이";
    
//     [Header("Follow")]
//     public float followDistance = 2f;
//     public float followSpeed = 2f;

//     private Vector3 startPosition;
//     private Quaternion initialModelRotation; // ✅ 초기 회전 저장
//     private float angle = 0f;
//     private bool isFollowing = false;
//     private Transform owner;

//     void Start()
//     {
//         startPosition = transform.position;
        
//         if (startPosition.y < 0.1f)
//         {
//             startPosition.y = 0.5f;
//             transform.position = startPosition;
//         }

//         // 모델 자동 찾기
//         if (catModel == null && transform.childCount > 0)
//         {
//             catModel = transform.GetChild(0);
//         }

//         // ✅ 모델의 초기 로컬 회전값 저장 (X -90 유지)
//         if (catModel != null)
//         {
//             initialModelRotation = catModel.localRotation;
//             Debug.Log($"[{catName}] 초기 회전 저장: {initialModelRotation.eulerAngles}");
//         }

//         angle = Random.Range(0f, 360f);
        
//         Debug.Log($"[{catName}] 배회 시작 - 위치: {startPosition}, 반경: {wanderRadius}");
//     }

//     void Update()
//     {
//         if (isFollowing)
//         {
//             FollowOwner();
//         }
//         else
//         {
//             WanderInCircle();
//         }
//     }

//     void WanderInCircle()
//     {
//         angle += moveSpeed * 20f * Time.deltaTime;

//         float x = startPosition.x + Mathf.Cos(angle * Mathf.Deg2Rad) * wanderRadius;
//         float z = startPosition.z + Mathf.Sin(angle * Mathf.Deg2Rad) * wanderRadius;

//         Vector3 targetPos = new Vector3(x, startPosition.y, z);
//         transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed * 2f);

//         Vector3 direction = (targetPos - transform.position).normalized;
//         direction.y = 0;

//         if (direction.sqrMagnitude > 0.01f)
//         {
//             Quaternion targetRotation = Quaternion.LookRotation(direction);
            
//             if (catModel != null)
//             {
//                 // ✅ 초기 X 회전 유지 + Y축만 회전
//                 float targetY = targetRotation.eulerAngles.y;
//                 float currentY = catModel.localEulerAngles.y;
//                 float newY = Mathf.LerpAngle(currentY, targetY, Time.deltaTime * rotateSpeed);
                
//                 // 초기 X 회전(-90) 유지하면서 Y축만 업데이트
//                 Vector3 initialEuler = initialModelRotation.eulerAngles;
//                 catModel.localRotation = Quaternion.Euler(initialEuler.x, newY, initialEuler.z);
//             }
//             else
//             {
//                 transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
//             }
//         }
//     }

//     void FollowOwner()
//     {
//         if (owner == null) return;

//         float distance = Vector3.Distance(transform.position, owner.position);

//         if (distance > followDistance)
//         {
//             Vector3 direction = (owner.position - transform.position).normalized;
//             direction.y = 0;

//             transform.position += direction * followSpeed * Time.deltaTime;

//             if (direction.sqrMagnitude > 0.01f)
//             {
//                 Quaternion targetRotation = Quaternion.LookRotation(direction);
                
//                 if (catModel != null)
//                 {
//                     // ✅ 초기 X 회전 유지 + Y축만 회전
//                     float targetY = targetRotation.eulerAngles.y;
//                     float currentY = catModel.localEulerAngles.y;
//                     float newY = Mathf.LerpAngle(currentY, targetY, Time.deltaTime * rotateSpeed * 2f);
                    
//                     Vector3 initialEuler = initialModelRotation.eulerAngles;
//                     catModel.localRotation = Quaternion.Euler(initialEuler.x, newY, initialEuler.z);
//                 }
//                 else
//                 {
//                     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed * 2f);
//                 }
//             }
//         }
//     }

//     public bool TryAcceptItem(CatItem item, Transform player)
//     {
//         if (item == preferredItem)
//         {
//             Debug.Log($"[{catName}] {item}을(를) 좋아해요! 따라갑니다.");
//             isFollowing = true;
//             owner = player;
//             return true;
//         }
//         else
//         {
//             Debug.Log($"[{catName}] {item}은(는) 싫어요... (좋아하는 건: {preferredItem})");
//             return false;
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         Vector3 center = Application.isPlaying ? startPosition : transform.position;
        
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(center, wanderRadius);
        
//         Gizmos.color = Color.green;
//         Gizmos.DrawSphere(center, 0.1f);
//     }
// }
