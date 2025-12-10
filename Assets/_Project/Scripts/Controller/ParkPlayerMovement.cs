using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ParkPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("SFX")]
    [SerializeField] private AudioClip footstepSound;    // 발자국 소리
    [SerializeField] private float footstepInterval = 1.5f; // 발자국 소리 간격

    private CharacterController controller;
    private Animator animator;

    private float yVelocity = 0f;
    private float footstepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        // float h = 0f;
        // float v = 0f;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool up = keyboard.wKey.isPressed;
        bool down = keyboard.sKey.isPressed;
        bool left = keyboard.aKey.isPressed;
        bool right = keyboard.dKey.isPressed;

        // 입력 값 계산
        if (up && down) v = 0f;
        else if (up) v += 1f;
        else if (down) v -= 1f;

        if (left && right) h = 0f;
        else if (right) h += 1f;
        else if (left) h -= 1f;

        // 이동 입력이 있는지 확인 (키를 조금이라도 눌렀으면 true)
        bool isMoving = (h != 0 || v != 0);

        // 애니메이터에게 알려줌 (Animator 창의 파라미터 이름이 "isWalk"라고 가정)
        animator.SetBool("isWalk", isMoving);

        // 발자국 소리 재생
        if (isMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        // *** 카메라 기준 이동 방향 계산 ***
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // *** 회전 ***
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // *** 중력 처리 ***
        if (controller.isGrounded)
            yVelocity = 0f;
        else
            yVelocity += gravity * Time.deltaTime;


        Vector3 move = moveDir * moveSpeed + new Vector3(0, yVelocity, 0);

        controller.Move(move * Time.deltaTime);
    }

    void PlayFootstepSound()
    {
        if (SFXManager.Instance == null)
        {
            Debug.LogWarning("SFXManager.Instance가 null입니다!");
            return;
        }

        if (footstepSound == null)
        {
            Debug.LogWarning("footstepSound가 할당되지 않았습니다!");
            return;
        }

        SFXManager.Instance.PlaySound(footstepSound, 0.6f);
        Debug.Log("발자국 소리 재생!");
    }
}
