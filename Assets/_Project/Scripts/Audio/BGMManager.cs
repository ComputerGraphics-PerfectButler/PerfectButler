using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    
    [Header("BGM Settings")]
    [SerializeField] private AudioClip menuBGM;      // 메인메뉴 음악
    [SerializeField] private AudioClip parkBGM;      // 동네탐험 음악
    [SerializeField] private AudioClip roomBGM;      // 집 음악
    [SerializeField, Range(0f, 1f)] private float volume = 0.3f;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.playOnAwake = false;

            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        PlayMenuBGM();  // 시작 시 메뉴 BGM 재생
    }
    
    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
        SetVolume(savedVolume);
    }

    public void PlayMenuBGM()
    {
        PlayBGM(menuBGM);
    }
    
    public void PlayParkBGM()
    {
        PlayBGM(parkBGM);
    }
    
    public void PlayRoomBGM()
    {
        PlayBGM(roomBGM);
    }
    
    private void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("BGM 클립이 null입니다!");
            return;
        }
        
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;  // 이미 같은 곡 재생 중이면 무시
        
        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log($"BGM 변경: {clip.name}");
    }
    
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
        Debug.Log($"BGM 볼륨 설정: {volume * 100:F0}%");
    }

    /// 현재 볼륨 값 가져오기
    public float GetVolume()
    {
        return volume;
    }
    
    /// BGM 일시정지
    public void Pause()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
    }
    
    /// BGM 재개
    public void Resume()
    {
        if (!audioSource.isPlaying)
            audioSource.UnPause();
    }
    
    /// BGM 정지
    public void Stop()
    {
        audioSource.Stop();
    }
}