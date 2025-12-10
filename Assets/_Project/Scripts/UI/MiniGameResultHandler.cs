using UnityEngine;
using PerfectButler.GameSystem;

namespace PerfectButler.UI
{
    /// 메인 씬에서 미니게임 결과를 확인하고 처리하는 컴포넌트
    /// MainGameUI나 GameManager와 같은 오브젝트에 추가
    public class MiniGameResultHandler : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private AudioClip miniGameReturnSound; // 미니게임 완료 후 돌아왔을 때 소리

        private bool hasProcessedResult = false;

        private void Start()
        {
            // CatStats 인스턴스 확인
            if (CatStats.Instance == null)
            {
                Debug.LogError("CatStats 인스턴스를 찾을 수 없습니다!");
                return;
            }

            // 시작 시 미니게임 결과 체크
            CheckAndApplyMiniGameResult();
        }

        private void OnEnable()
        {
            // 활성화될 때마다 초기화 (씬 재진입 대비)
            hasProcessedResult = false;
        }
        
        /// 미니게임 결과 확인 및 적용
        private void CheckAndApplyMiniGameResult()
        {
            Debug.Log($"[MiniGameResultHandler] CheckAndApplyMiniGameResult 호출됨");

            // 이미 처리했으면 스킵
            if (hasProcessedResult)
            {
                Debug.Log($"[MiniGameResultHandler] 이미 처리됨, 스킵");
                return;
            }

            // 미니게임 완료 플래그 확인
            int miniGameCompleted = PlayerPrefs.GetInt("MiniGameCompleted", 0);
            MiniGameResult result = MiniGameResultManager.LastResult;

            Debug.Log($"[MiniGameResultHandler] 결과 확인: result={result}, miniGameCompleted={miniGameCompleted}");

            // 미니게임 완료 플래그가 설정되어 있으면
            if (miniGameCompleted == 1)
            {
                Debug.Log($"[MiniGameResultHandler] 미니게임 결과 발견! 보상 적용 중...");
                ApplyMiniGameReward(result);
                hasProcessedResult = true;

                // 결과 및 플래그 클리어
                MiniGameResultManager.ClearResult();
                PlayerPrefs.DeleteKey("MiniGameCompleted");
                PlayerPrefs.Save();

                Debug.Log($"[MiniGameResultHandler] 플래그 클리어 완료");
            }
            else
            {
                Debug.Log($"[MiniGameResultHandler] 미니게임 완료 플래그 없음");
            }
        }
        
        /// 미니게임 결과에 따른 보상 적용
        private void ApplyMiniGameReward(MiniGameResult result)
        {
            if (CatStats.Instance == null)
            {
                Debug.LogError("CatStats.Instance가 null입니다!");
                return;
            }

            // 결과에 따른 경험치 차등 지급
            float expReward = result switch
            {
                MiniGameResult.Perfect => 20f,
                MiniGameResult.Normal => 10f,
                MiniGameResult.Fail => 5f,
                _ => 10f
            };

            // 재미 스탯 증가 (미니게임이니까)
            float funIncrease = result switch
            {
                MiniGameResult.Perfect => 20f,
                MiniGameResult.Normal => 15f,
                MiniGameResult.Fail => 10f,
                _ => 15f
            };

            Debug.Log($"[MiniGameResultHandler] 보상 지급 시작: 결과={result}, 재미+{funIncrease}, 경험치+{expReward}");

            // 미니게임 완료 후 돌아왔을 때 소리 재생
            if (SFXManager.Instance != null && miniGameReturnSound != null)
            {
                SFXManager.Instance.PlaySound(miniGameReturnSound);
            }

            // 스탯 및 경험치 적용 + 쿨타임 기록
            CatStats.Instance.PerformActionWithoutCooldownCheck(
                StatType.Fun,
                funIncrease,
                expReward,
                $"미니게임 완료 ({result})"
            );

            Debug.Log($"[MiniGameResultHandler] 미니게임 보상 지급 완료!");

            // 씬이 새로 로드되었으므로 가구도 업데이트
            StartCoroutine(UpdateFurnitureAfterReward());
        }

        /// <summary>
        /// 보상 지급 후 가구 업데이트 (미니게임에서 돌아왔을 때)
        /// </summary>
        private System.Collections.IEnumerator UpdateFurnitureAfterReward()
        {
            yield return null; // 한 프레임 대기 (RoomDecoManager 초기화 대기)

            Debug.Log("[MiniGameResultHandler] 가구 업데이트 시작");

            RoomDecoManager decoManager = FindObjectOfType<RoomDecoManager>();
            if (decoManager != null && CatStats.Instance != null)
            {
                int currentLevel = CatStats.Instance.CurrentLevel;
                Debug.Log($"[MiniGameResultHandler] 현재 레벨: {currentLevel}로 가구 설정");
                decoManager.SetFurnitureVisibility(currentLevel);
            }
            else
            {
                Debug.LogWarning("[MiniGameResultHandler] RoomDecoManager 또는 CatStats를 찾을 수 없습니다!");
            }
        }
    }
}