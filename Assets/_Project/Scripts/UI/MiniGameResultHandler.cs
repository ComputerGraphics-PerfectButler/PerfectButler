using UnityEngine;
using PerfectButler.GameSystem;

namespace PerfectButler.UI
{
    /// 메인 씬에서 미니게임 결과를 확인하고 처리하는 컴포넌트
    /// MainGameUI나 GameManager와 같은 오브젝트에 추가
    public class MiniGameResultHandler : MonoBehaviour
    {
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
            // 이미 처리했으면 스킵
            if (hasProcessedResult) return;
            
            // 미니게임 결과가 있는지 확인
            MiniGameResult result = MiniGameResultManager.LastResult;
            
            // 기본값(Normal)이 아니라면 미니게임을 플레이한 것으로 간주
            if (result != MiniGameResult.Normal || PlayerPrefs.HasKey("JustPlayedMiniGame"))
            {
                ApplyMiniGameReward(result);
                hasProcessedResult = true;
                
                // 결과 클리어
                MiniGameResultManager.ClearResult();
                PlayerPrefs.DeleteKey("JustPlayedMiniGame");
            }
        }
        
        /// 미니게임 결과에 따른 보상 적용
        private void ApplyMiniGameReward(MiniGameResult result)
        {
            if (CatStats.Instance == null) return;

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

            // 스탯 및 경험치 적용 + 쿨타임 기록
            CatStats.Instance.PerformActionWithoutCooldownCheck(
                StatType.Fun,
                funIncrease,
                expReward,
                $"미니게임 완료 ({result})"
            );

            Debug.Log($"미니게임 보상 지급 완료: 재미 +{funIncrease}, 경험치 +{expReward}");
        }
    }
}