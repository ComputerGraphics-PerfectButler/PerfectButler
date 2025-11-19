using UnityEngine;

namespace PerfectButler.GameSystem
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public bool isGameStarted = false;

        [Header("References")]
        public CatStats CatStats { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // CatStats 참조 찾기 (같은 오브젝트 또는 자식)
                CatStats = GetComponentInChildren<CatStats>();
                if (CatStats == null)
                {
                    CatStats = FindObjectOfType<CatStats>();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            Debug.Log("PerfectButler Game Started!");

            if (CatStats == null)
            {
                Debug.LogWarning("CatStats를 찾을 수 없습니다. CatStats 오브젝트가 씬에 있는지 확인하세요.");
            }
        }
    }
}