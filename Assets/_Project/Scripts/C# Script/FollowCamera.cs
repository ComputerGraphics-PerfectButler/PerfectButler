using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 baseOffset = new Vector3(0, 8, 8); // 기본 포지션 값
    public float followSpeed = 5f;

    private Vector3 initialOffset;

    void Start()
    {
        // 카메라의 초기 위치를 저장해둠 (혹시 Inspector에서 직접 배치했을 경우 대비)
        initialOffset = baseOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ✅ stickman_9의 위치 + 기본 오프셋
        Vector3 desiredPosition = target.position + initialOffset;

        // 부드럽게 따라감
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // 회전은 Inspector에서 세팅된 값 고정 (따라가지 않음)
    }
}
