using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ParkPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Animator animator;

    private float yVelocity = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        float h = 0f;
        float v = 0f;

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
            yVelocity = -1f;  
        else
            yVelocity += gravity * Time.deltaTime;

        Vector3 move = moveDir * moveSpeed + new Vector3(0, yVelocity, 0);

        controller.Move(move * Time.deltaTime);
    }
}
