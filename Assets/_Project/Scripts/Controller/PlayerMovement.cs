using UnityEngine;
using UnityEngine.InputSystem; // 👈 새 입력 시스템 네임스페이스 추가

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private CharacterController controller;
    private Animator animator;

    private Vector2 moveInput;
    private KeyCode lastKey = KeyCode.None;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 🔄 Input System의 Keyboard 클래스 사용
        float h = 0f;
        float v = 0f;

        var keyboard = Keyboard.current;

        if (keyboard.wKey.isPressed) { v -= 1; lastKey = KeyCode.W; }
        if (keyboard.sKey.isPressed) { v += 1; lastKey = KeyCode.S; }
        if (keyboard.aKey.isPressed) { h += 1; lastKey = KeyCode.A; }
        if (keyboard.dKey.isPressed) { h -= 1; lastKey = KeyCode.D; }

        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        if (moveDir != Vector3.zero)
        {
            float targetY = transform.localEulerAngles.y;

            switch (lastKey)
            {
                case KeyCode.W: targetY = 180f; break;
                case KeyCode.S: targetY = 0f; break;
                case KeyCode.A: targetY = 90f; break;
                case KeyCode.D: targetY = -90f; break;
            }

            transform.localEulerAngles = new Vector3(0, targetY, 0);
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
            animator?.SetBool("isWalking", true);
        }
        else
        {
            animator?.SetBool("isWalking", false);
        }
    }
}
