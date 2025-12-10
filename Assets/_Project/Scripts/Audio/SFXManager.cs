using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    private AudioSource audioSource;
    [SerializeField, Range(0f, 1f)] private float volume = 0.8f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        SetVolume(savedVolume);
    }

    /// <summary>
    /// 효과음 재생 (한 번만)
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SFX 클립이 null입니다!");
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// 효과음 재생 (볼륨 조절 가능)
    /// </summary>
    public void PlaySound(AudioClip clip, float volumeScale)
    {
        if (clip == null)
        {
            Debug.LogWarning("SFX 클립이 null입니다!");
            return;
        }

        audioSource.PlayOneShot(clip, volume * volumeScale);
    }

    /// <summary>
    /// SFX 볼륨 설정
    /// </summary>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
        Debug.Log($"SFX 볼륨 설정: {volume * 100:F0}%");
    }

    /// <summary>
    /// 현재 볼륨 값 가져오기
    /// </summary>
    public float GetVolume()
    {
        return volume;
    }
}
