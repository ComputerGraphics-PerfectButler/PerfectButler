using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PerfectButler.UI
{
    /// 버튼에 마우스를 올렸을 때 크기가 커지는 효과
    public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float animationSpeed = 10f;
        
        private Vector3 originalScale;
        private Vector3 targetScale;
        private bool isHovering = false;
        
        private void Start()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;
        }
        
        private void Update()
        {
            // 부드럽게 크기 변경
            transform.localScale = Vector3.Lerp(
                transform.localScale, 
                targetScale, 
                animationSpeed * Time.deltaTime
            );
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale;
            isHovering = true;
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = originalScale;
            isHovering = false;
        }
    }
}