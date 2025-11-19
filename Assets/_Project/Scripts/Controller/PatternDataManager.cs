using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 패턴 데이터를 JSON 파일로 저장/불러오기
/// </summary>
public class PatternDataManager : MonoBehaviour
{
    [Header("File Settings")]
    public string saveFileName = "CatPatterns_updated.json";
    
    private string SaveFilePath => Path.Combine(Application.streamingAssetsPath, saveFileName);
    
    private PatternRecorder recorder;

    void Start()
    {
        recorder = GetComponent<PatternRecorder>();
        
        if (recorder == null)
        {
            Debug.LogWarning("PatternRecorder 컴포넌트가 없습니다. 자동 불러오기 전용 모드로 실행됩니다.");
            // PatternRecorder가 없어도 작동하도록 임시 생성
            recorder = gameObject.AddComponent<PatternRecorder>();
        }
    }

    /// <summary>
    /// 현재 녹화된 패턴들을 JSON 파일로 저장
    /// </summary>
    [ContextMenu("Save Patterns to File")]
    public void SavePatternsToFile()
    {
        if (recorder.savedPatterns.Count == 0)
        {
            Debug.LogWarning("저장할 패턴이 없습니다!");
            return;
        }

        try
        {
            PatternDataWrapper wrapper = new PatternDataWrapper
            {
                patterns = recorder.savedPatterns
            };

            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(SaveFilePath, json);

            Debug.Log($"<color=green>✓ 패턴 저장 완료!</color>\n경로: {SaveFilePath}\n패턴 수: {recorder.savedPatterns.Count}개");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"패턴 저장 실패: {e.Message}");
        }
    }

    /// <summary>
    /// JSON 파일에서 패턴들을 불러와서 반환
    /// </summary>
    public List<MovementPattern> LoadAndGetPatterns()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning($"저장된 파일이 없습니다: {SaveFilePath}");
            Debug.LogWarning("파일 경로를 확인하세요. StreamingAssets 폴더에 파일이 있는지 확인하세요.");
            return new List<MovementPattern>();
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("파일이 비어있습니다.");
                return new List<MovementPattern>();
            }
            
            PatternDataWrapper wrapper = JsonUtility.FromJson<PatternDataWrapper>(json);

            if (wrapper == null)
            {
                Debug.LogWarning("JSON 파싱 실패! 파일 형식을 확인하세요.");
                return new List<MovementPattern>();
            }

            if (wrapper.patterns != null && wrapper.patterns.Count > 0)
            {
                Debug.Log($"<color=cyan>✓ 패턴 불러오기 완료!</color>\n패턴 수: {wrapper.patterns.Count}개");
                
                // 패턴 목록 출력
                for (int i = 0; i < wrapper.patterns.Count; i++)
                {
                    var p = wrapper.patterns[i];
                    Debug.Log($"  {i + 1}. {p.patternName} - 시작: {p.startPosition}, 프레임: {p.frames.Count}개");
                }
                
                return wrapper.patterns;
            }
            else
            {
                Debug.LogWarning("파일에 패턴 데이터가 없습니다.");
                return new List<MovementPattern>();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"패턴 불러오기 실패: {e.Message}");
            Debug.LogError($"스택 트레이스: {e.StackTrace}");
            return new List<MovementPattern>();
        }
    }

    /// <summary>
    /// JSON 파일에서 패턴들을 불러오기 (기존 메서드 - 호환성 유지)
    /// </summary>
    [ContextMenu("Load Patterns from File")]
    public void LoadPatternsFromFile()
    {
        var patterns = LoadAndGetPatterns();
        
        if (patterns.Count > 0)
        {
            // recorder가 null이면 생성
            if (recorder == null)
            {
                Debug.LogWarning("PatternRecorder가 없어서 자동 생성합니다.");
                recorder = gameObject.AddComponent<PatternRecorder>();
            }
            
            recorder.savedPatterns = patterns;
        }
    }

    /// <summary>
    /// 저장된 파일 경로 출력
    /// </summary>
    [ContextMenu("Show Save File Path")]
    public void ShowSaveFilePath()
    {
        Debug.Log($"저장 경로: {SaveFilePath}");
        Debug.Log($"파일 존재 여부: {File.Exists(SaveFilePath)}");
    }

    /// <summary>
    /// 저장된 파일 삭제
    /// </summary>
    [ContextMenu("Delete Save File")]
    public void DeleteSaveFile()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("저장 파일 삭제됨");
        }
        else
        {
            Debug.Log("삭제할 파일이 없습니다.");
        }
    }
}

/// <summary>
/// JSON 직렬화를 위한 래퍼 클래스
/// </summary>
[System.Serializable]
public class PatternDataWrapper
{
    public List<MovementPattern> patterns = new List<MovementPattern>();
}
