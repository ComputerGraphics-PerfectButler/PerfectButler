using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float fixedY = 0.3394098f; // Y값 고정

    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        // 시작 시 Y 위치를 고정값으로 설정
        Vector3 startPos = transform.position;
        startPos.y = fixedY;
        transform.position = startPos;
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

        if (up && down) v = 0f;
        else if (up) v -= 1f;
        else if (down) v += 1f;

        if (left && right) h = 0f;
        else if (right) h -= 1f;
        else if (left) h += 1f;

        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            controller.Move(moveDir * moveSpeed * Time.deltaTime);
            animator?.SetBool("isWalking", true);
        }
        else
        {
            animator?.SetBool("isWalking", false);
        }

        // Y 고정: 현재 Y와 목표 Y의 차이만큼 보정
        float yDifference = fixedY - transform.position.y;
        if (Mathf.Abs(yDifference) > 0.0001f)
        {
            controller.Move(new Vector3(0, yDifference, 0));
        }
    }
}
