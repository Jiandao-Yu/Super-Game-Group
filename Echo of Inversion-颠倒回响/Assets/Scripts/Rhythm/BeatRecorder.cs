using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BeatRecorder : MonoBehaviour
{
    public AudioSource audioSource;
    private List<float> beats = new List<float>();
    private bool isRecording = false;

    void Update()
    {
        // 按 R 键开始/停止录制
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isRecording) StartRecording();
            else StopAndSave();
        }

        // 录制中按空格打点
        if (isRecording && Input.GetKeyDown(KeyCode.Space))
        {
            beats.Add(audioSource.time);
            Debug.Log($"🎵 打点 {beats.Count}: {audioSource.time:F2}s");
        }
    }

    void StartRecording()
    {
        beats.Clear();
        isRecording = true;
        audioSource.time = 0;
        audioSource.Play();
        Debug.Log("=== 录制开始！听音乐按空格打点，按 R 结束 ===");
    }

    void StopAndSave()
    {
        isRecording = false;
        audioSource.Stop();

        BeatData data = new BeatData();
        data.beats = beats.ToArray();
        string json = JsonUtility.ToJson(data, true);
        string path = Application.dataPath + "/beats.txt";
        File.WriteAllText(path, json);

        Debug.Log($"✅ 保存了 {beats.Count} 个节拍点到: {path}");
    }

    [System.Serializable]
    public class BeatData
    {
        public float[] beats;
    }
}