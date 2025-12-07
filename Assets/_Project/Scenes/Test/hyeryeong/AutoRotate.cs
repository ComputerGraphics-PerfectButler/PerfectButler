using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float speed = 50f; // 회전 속도

    void Update()
    {
        // [핵심 변경점]
        // Vector3.up은 (0, 1, 0) 즉, Y축을 의미합니다.
        // Space.World를 추가하면 물체가 어떻게 기울어져 있든 상관없이
        // 무조건 "세상(World)의 위쪽 방향"을 기준으로 돌게 됩니다. (팽이처럼)
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
    }
}