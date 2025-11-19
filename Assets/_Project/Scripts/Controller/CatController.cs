using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class CatController_NewInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float bounceHeight = 0.2f;
    public float bounceSpeed = 20f;
    public float rotationSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 1.0f;
    public float jumpForwardMultiplier = 0.5f;
    public int maxAirJumps = 99;

    [Header("Flip Settings")]
    public float flipHeight = 5.0f;
    public float flipRotateSpeed = 360f;
    public float flipDuration = 1.0f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Sound")]
    public AudioClip meowClip;

    private AudioSource audioSource;
    private Vector3 startPos;
    private bool isFlipping = false;
    private bool isJumping = false;
    private float flipTimer = 0f;
    private Vector3 flipDir = Vector3.zero;
    private Quaternion flipBaseRotation;
    private float emotionTimer = 0f;
    private string currentEmotion = "";
    private Quaternion baseRotation;
    private Rigidbody rb;
    private Vector3 targetMoveDirection = Vector3.zero;
    private int currentAirJumps = 0;
    private bool isGrounded = false;
    private float walkBobTimer = 0f;

    void Start()
    {
        startPos = transform.position;
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        // Rigidbody 설정
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
    }

    void Update()
    {
        CheckGroundStatus();

        if (isFlipping)
        {
            HandleFlip();
            return;
        }

        HandleInput();
        HandleEmotion();
    }

    void FixedUpdate()
    {
        // 지면에 있을 때만 수평 이동 처리
        if (!isFlipping && targetMoveDirection.magnitude > 0.1f)
        {
            Vector3 horizontalVelocity = targetMoveDirection * moveSpeed;
            
            // 지면에 있고 점프 중이 아닐 때만 걷기 바운스 적용
            if (isGrounded && !isJumping)
            {
                walkBobTimer += Time.fixedDeltaTime * bounceSpeed;
                float bobOffset = Mathf.Sin(walkBobTimer) * bounceHeight;
                horizontalVelocity.y = bobOffset;
            }
            else
            {
                horizontalVelocity.y = rb.linearVelocity.y; // 기존 y 속도 유지 (중력/점프)
            }
            
            rb.linearVelocity = horizontalVelocity;
        }
        else if (isGrounded && !isFlipping && !isJumping)
        {
            // 정지 상태일 때
            walkBobTimer = 0f; // 타이머 리셋
            Vector3 velocity = rb.linearVelocity;
            velocity.x = 0;
            velocity.z = 0;
            rb.linearVelocity = velocity;
        }
    }

    void CheckGroundStatus()
    {
        // 지면 체크
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);

        // 레이캐스트가 작동하지 않을 경우 간단한 높이 체크
        if (!isGrounded)
        {
            isGrounded = rb.linearVelocity.y <= 0.01f && Mathf.Abs(rb.position.y - startPos.y) < 0.1f;
        }

        // 땅에 닿으면 공중 점프 리셋
        if (isGrounded && !isJumping)
        {
            currentAirJumps = 0;
        }
    }

    void HandleInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // WASD 이동
        Vector3 move = Vector3.zero;
        if (kb.wKey.isPressed) move += Vector3.forward;
        if (kb.sKey.isPressed) move += Vector3.back;
        if (kb.aKey.isPressed) move += Vector3.left;
        if (kb.dKey.isPressed) move += Vector3.right;

        bool moving = move.magnitude > 0.1f;
        if (moving)
        {
            Transform cam = Camera.main.transform;
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0; camRight.y = 0;
            camForward.Normalize(); camRight.Normalize();

            targetMoveDirection = (camForward * move.z + camRight * move.x).normalized;

            // 회전
            Quaternion targetRotation = Quaternion.LookRotation(targetMoveDirection);
            Quaternion newRotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            rb.MoveRotation(newRotation);
        }
        else
        {
            targetMoveDirection = Vector3.zero;
        }

        // L키로 롤링
        if (moving && kb.lKey.isPressed)
        {
            Quaternion rollRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 10f) * 0.5f);
            rb.MoveRotation(rb.rotation * rollRotation);
        }

        // 스페이스바 점프
        if (kb.spaceKey.wasPressedThisFrame)
        {
            if (isGrounded)
            {
                // 땅에서 점프
                currentAirJumps = 0;
                PerformJump(targetMoveDirection);
            }
            else if (currentAirJumps < maxAirJumps)
            {
                // 공중 점프
                currentAirJumps++;
                PerformJump(targetMoveDirection);
            }
        }

        // Q/E 덤블링
        if (kb.qKey.wasPressedThisFrame) StartFlip(Vector3.right);
        if (kb.eKey.wasPressedThisFrame) StartFlip(Vector3.left);

        // V 울음소리
        if (kb.vKey.wasPressedThisFrame && meowClip != null)
            audioSource.PlayOneShot(meowClip);

        // J/K/L 감정표현
        if (kb.jKey.wasPressedThisFrame)
        {
            currentEmotion = "shakeY";
            baseRotation = rb.rotation;
        }
        else if (kb.kKey.wasPressedThisFrame)
        {
            currentEmotion = "nodX";
            baseRotation = rb.rotation;
        }
        else if (kb.lKey.wasPressedThisFrame && !moving)
        {
            currentEmotion = "rollZ";
            baseRotation = rb.rotation;
        }

        if (kb.jKey.wasReleasedThisFrame || kb.kKey.wasReleasedThisFrame || kb.lKey.wasReleasedThisFrame)
        {
            emotionTimer = 0f;
        }
    }

    void PerformJump(Vector3 direction)
    {
        isJumping = true;

        // 수직 점프력
        Vector3 jumpVelocity = Vector3.up * jumpForce;

        // 방향키를 누르고 있으면 해당 방향으로 추가 힘
        if (direction.magnitude > 0.1f)
        {
            jumpVelocity += direction * moveSpeed * jumpForwardMultiplier;
        }

        rb.linearVelocity = jumpVelocity;

        // 짧은 딜레이 후 점프 상태 해제
        StartCoroutine(JumpCooldown());
    }

    System.Collections.IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        isJumping = false;
    }

    void HandleEmotion()
    {
        if (string.IsNullOrEmpty(currentEmotion)) return;

        emotionTimer += Time.deltaTime * 10f;
        float sin = Mathf.Sin(emotionTimer);
        float angle = sin * 25f;

        Quaternion relativeRotation = Quaternion.identity;
        if (currentEmotion == "shakeY")
            relativeRotation = Quaternion.Euler(0, angle, 0);
        else if (currentEmotion == "nodX")
            relativeRotation = Quaternion.Euler(angle, 0, 0);
        else if (currentEmotion == "rollZ")
            relativeRotation = Quaternion.Euler(0, 0, angle);

        rb.MoveRotation(baseRotation * relativeRotation);

        var kb = Keyboard.current;
        bool keyUp = (!kb.jKey.isPressed && !kb.kKey.isPressed && !kb.lKey.isPressed);

        if (keyUp && Mathf.Abs(sin) < 0.05f)
        {
            rb.MoveRotation(baseRotation);
            currentEmotion = "";
            emotionTimer = 0f;
        }
    }

    void StartFlip(Vector3 dir)
    {
        if (isFlipping) return;

        isFlipping = true;
        flipDir = dir;
        flipTimer = 0f;
        flipBaseRotation = rb.rotation;

        // 덤블링 중에는 중력 무시
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }

    void HandleFlip()
    {
        flipTimer += Time.deltaTime;
        float t = flipTimer / flipDuration;

        // 높이 곡선
        float height = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI) * flipHeight;
        Vector3 newPos = rb.position;
        newPos.y = startPos.y + height;
        rb.MovePosition(newPos);

        // 회전
        float angle = 360f * t;
        Quaternion relative = Quaternion.Euler(flipDir * angle);
        rb.MoveRotation(flipBaseRotation * relative);

        // 착지
        if (flipTimer >= flipDuration)
        {
            isFlipping = false;
            rb.MoveRotation(flipBaseRotation);
            rb.useGravity = true; // 중력 다시 활성화
        }
    }
}
