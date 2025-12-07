using UnityEngine;
using UnityEngine.SceneManagement;

/// Scene이 로드될 때 자동으로 해당 Scene의 BGM을 재생합니다.
/// BGMManager GameObject에 함께 추가하세요.
public class SceneBGMController : MonoBehaviour
{
    private void OnEnable()
    {
        // Scene이 로드될 때마다 호출되도록 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        // 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Start()
    {
        // 시작할 때 현재 Scene의 BGM 재생
        PlayBGMForCurrentScene();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Scene이 로드되면 해당 Scene의 BGM 재생
        PlayBGMForScene(scene.name);
    }
    
    private void PlayBGMForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayBGMForScene(currentSceneName);
    }
    
    private void PlayBGMForScene(string sceneName)
    {
        if (BGMManager.Instance == null)
        {
            Debug.LogWarning("BGMManager가 없습니다!");
            return;
        }
        
        // Scene 이름에 따라 적절한 BGM 재생
        switch (sceneName)
        {
            case "MainMenu":
                BGMManager.Instance.PlayMenuBGM();
                break;
                
            case "park":
                BGMManager.Instance.PlayParkBGM();
                break;
                
            case "room":
                BGMManager.Instance.PlayRoomBGM();
                break;
                
            default:
                Debug.Log($"Scene '{sceneName}'에 대한 BGM 설정이 없습니다.");
                break;
        }
    }
}