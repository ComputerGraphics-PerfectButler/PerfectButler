using UnityEngine;

namespace PerfectButler.GameSystem
{
    /// <summary>
    /// room 씬에서 냥줍한 고양이 정보를 불러와서 적용하는 스크립트
    /// </summary>
    public class CaughtCatLoader : MonoBehaviour
    {
        [Header("Cat Object")]
        [SerializeField] private GameObject catObject; // 방에 있는 고양이 오브젝트
        [SerializeField] private Renderer catRenderer; // 고양이의 Renderer (Material 변경용)

        [Header("Available Cat Materials")]
        [SerializeField] private Material[] catMaterials; // 사용 가능한 모든 고양이 Material들

        void Start()
        {
            LoadCaughtCatData();
        }

        /// <summary>
        /// 냥줍한 고양이 데이터를 불러와서 적용
        /// </summary>
        void LoadCaughtCatData()
        {
            // 저장된 고양이 데이터가 있는지 확인
            if (!PlayerPrefs.HasKey("CaughtCat_MaterialName"))
            {
                Debug.Log("[CaughtCatLoader] 냥줍한 고양이 데이터가 없습니다.");
                return;
            }

            // Material 이름 불러오기
            string materialName = PlayerPrefs.GetString("CaughtCat_MaterialName", "");

            // 고양이 이름 불러오기
            string catName = PlayerPrefs.GetString("CaughtCat_Name", "고양이");

            Debug.Log($"[CaughtCatLoader] 냥줍한 고양이 불러오기: {catName}, Material: {materialName}");

            // Material 찾아서 적용
            ApplyCatMaterial(materialName);
        }

        /// <summary>
        /// 고양이 오브젝트에 Material 적용
        /// </summary>
        void ApplyCatMaterial(string materialName)
        {
            if (catRenderer == null && catObject != null)
            {
                // Renderer 자동 찾기
                catRenderer = catObject.GetComponentInChildren<Renderer>();
            }

            if (catRenderer == null)
            {
                Debug.LogWarning("[CaughtCatLoader] 고양이 Renderer를 찾을 수 없습니다!");
                return;
            }

            // Material 배열에서 이름이 일치하는 Material 찾기
            Material targetMaterial = null;
            foreach (Material mat in catMaterials)
            {
                if (mat != null && mat.name == materialName)
                {
                    targetMaterial = mat;
                    break;
                }
            }

            // Material 적용
            if (targetMaterial != null)
            {
                catRenderer.sharedMaterial = targetMaterial;
                Debug.Log($"[CaughtCatLoader] 고양이 Material 적용 완료: {materialName}");
            }
            else
            {
                Debug.LogWarning($"[CaughtCatLoader] Material '{materialName}'을(를) 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// Inspector에서 catObject 설정 후 자동으로 Renderer 찾기
        /// </summary>
        [ContextMenu("Auto Find Renderer")]
        void AutoFindRenderer()
        {
            if (catObject != null)
            {
                catRenderer = catObject.GetComponentInChildren<Renderer>();
                if (catRenderer != null)
                {
                    Debug.Log($"[CaughtCatLoader] Renderer 자동 찾기 성공: {catRenderer.name}");
                }
                else
                {
                    Debug.LogWarning("[CaughtCatLoader] Renderer를 찾을 수 없습니다!");
                }
            }
        }
    }
}
