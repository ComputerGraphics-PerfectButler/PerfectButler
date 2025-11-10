using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class CatController_NewInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float bounceHeight = 0.05f;
    public float bounceSpeed = 20f;

    [Header("Flip Settings")]
    public float flipHeight = 2.5f;           // 점프 높이 (Q/E용)
    public float flipRotateSpeed = 360f;      // 초당 회전 속도
    public float flipDuration = 1.0f;         // 전체 동작 시간

    [Header("Sound")]
    public AudioClip meowClip;
    private AudioSource audioSource;

    private Vector3 startPos;
    private bool isFlipping = false;
    private bool isJumping = false;
    private float flipTimer = 0f;
    private Vector3 flipDir = Vector3.zero;
    private Quaternion flipBaseRotation;      // ← 덤블링 기준 회전

    // 감정표현 관련
    private float emotionTimer = 0f;
    private string currentEmotion = "";
    private Quaternion baseRotation;          // ← 감정표현 기준 회전

    void Start()
    {
        startPos = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isFlipping)
        {
            HandleFlip();
            return;
        }

        HandleInput();
        HandleEmotion();
    }

    void HandleInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // ===========================
        // 1️⃣ 카메라 기준 WASD 이동
        // ===========================
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

            Vector3 moveDir = (camForward * move.z + camRight * move.x).normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * 10f);

            float t = Time.time * bounceSpeed;
            float y = Mathf.Abs(Mathf.Sin(t)) * bounceHeight;
            transform.position = new Vector3(transform.position.x, startPos.y + y, transform.position.z);
        }

        if (moving && kb.lKey.isPressed)
            transform.Rotate(Vector3.forward * Mathf.Sin(Time.time * 10f) * 0.5f);

        if (kb.spaceKey.wasPressedThisFrame && !isJumping)
            StartCoroutine(Jump());

        // ===========================
        // 2️⃣ 덤블링 & 백덤블링 (Q/E)
        // ===========================
        if (kb.qKey.wasPressedThisFrame) StartFlip(Vector3.right);
        if (kb.eKey.wasPressedThisFrame) StartFlip(Vector3.left);

        // ===========================
        // 3️⃣ 울음소리
        // ===========================
        if (kb.vKey.wasPressedThisFrame && meowClip != null)
            audioSource.PlayOneShot(meowClip);

        // ===========================
        // 4️⃣ 감정표현 (J/K/L)
        // ===========================
        if (kb.jKey.wasPressedThisFrame)
        {
            currentEmotion = "shakeY";
            baseRotation = transform.rotation;
        }
        else if (kb.kKey.wasPressedThisFrame)
        {
            currentEmotion = "nodX";
            baseRotation = transform.rotation;
        }
        else if (kb.lKey.wasPressedThisFrame && !moving)
        {
            currentEmotion = "rollZ";
            baseRotation = transform.rotation;
        }

        // 키를 뗐을 때 타이머 초기화
        if (kb.jKey.wasReleasedThisFrame || kb.kKey.wasReleasedThisFrame || kb.lKey.wasReleasedThisFrame)
        {
            emotionTimer = 0f;
        }
    }

    // 🎭 감정표현 (현재 회전 기준으로 상대 회전 적용)
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

        transform.rotation = baseRotation * relativeRotation;

        var kb = Keyboard.current;
        bool keyUp = (!kb.jKey.isPressed && !kb.kKey.isPressed && !kb.lKey.isPressed);

        if (keyUp && Mathf.Abs(sin) < 0.05f)
        {
            transform.rotation = baseRotation;
            currentEmotion = "";
            emotionTimer = 0f;
        }
    }

    // 🌀 덤블링 / 백덤블링
    void StartFlip(Vector3 dir)
    {
        if (isFlipping) return;
        isFlipping = true;
        flipDir = dir;
        flipTimer = 0f;
        flipBaseRotation = transform.rotation; // 현재 회전값 저장
    }

    void HandleFlip()
    {
        flipTimer += Time.deltaTime;
        float t = flipTimer / flipDuration;

        // 1) 높이 곡선 (포물선)
        float height = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI) * flipHeight;
        transform.position = new Vector3(transform.position.x, startPos.y + height, transform.position.z);

        // 2) 상대 회전 (현재 바라보는 방향 기준으로 앞뒤 회전)
        float angle = 360f * t;
        Quaternion relative = Quaternion.Euler(flipDir * angle);
        transform.rotation = flipBaseRotation * relative;

        // 3) 착지 후 복귀
        if (flipTimer >= flipDuration)
        {
            isFlipping = false;
            transform.rotation = flipBaseRotation;
            transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
        }
    }

    // 🐈 일반 점프
    System.Collections.IEnumerator Jump()
    {
        isJumping = true;
        float jumpTime = 0f;
        float jumpDuration = 0.5f;

        while (jumpTime < jumpDuration)
        {
            jumpTime += Time.deltaTime;
            float y = Mathf.Sin(jumpTime / jumpDuration * Mathf.PI) * 1.0f;
            transform.position = new Vector3(transform.position.x, startPos.y + y, transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
        isJumping = false;
    }
}
