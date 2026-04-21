using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RhythmGameCore : MonoBehaviour
{
    [Header("=== 音频设置 ===")]
    public AudioSource audioSource;
    public AudioClip[] musicList;

    [Header("=== 判定设置 ===")]
    public float perfectWindow = 0.1f;
    public float goodWindow = 0.3f;

    [Header("=== 连击设置 ===")]
    public int combo = 0;
    public int maxCombo = 0;

    [Header("=== 视觉反馈（拖拽进来）===")]
    public GameObject cubeForFeedback;
    public TMP_Text comboText;
    public Transform canvasTransform;
    public GameObject judgeTextPrefab;

    [Header("=== 手动卡点模式 ===")]
    public string beatFilePath = "beats.txt";
    public float beatOffset = 0f;
    private float[] manualBeats;
    private int beatIndex = 0;
    private float lastProcessedBeatTime = -1f;

    [Header("=== 颜色 ===")]
    public Color perfectColor = Color.yellow;
    public Color goodColor = Color.cyan;
    public Color missColor = Color.red;
    public Color normalColor = Color.white;

    // 内部变量
    private float lastBeatTime = 0f;
    private float lastClickTime = -1f;
    private bool hasBufferedInput = false;
    private Renderer cubeRenderer;
    private Material cubeMaterial;

    // 事件（给 NoteController 和组长用）
    public static event System.Action<string, int> OnJudgeResult;
    public static event System.Action<int> OnComboChanged;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSource != null && musicList != null && musicList.Length > 0)
        {
            audioSource.clip = musicList[0];
        }

        if (cubeForFeedback != null)
        {
            cubeRenderer = cubeForFeedback.GetComponent<Renderer>();
            if (cubeRenderer != null) cubeMaterial = cubeRenderer.material;
        }

        LoadManualBeats();

        Debug.Log("节奏游戏核心启动 | 手动卡点模式");
    }

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying) return;

        if (manualBeats != null && beatIndex < manualBeats.Length)
        {
            float nextBeatTime = manualBeats[beatIndex] + beatOffset;
            if (audioSource.time >= nextBeatTime - 0.03f)
            {
                OnBeatDetected(nextBeatTime);
                beatIndex++;
            }
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            lastClickTime = Time.time;
            hasBufferedInput = true;
            EvaluateInput();
        }
    }

    void OnBeatDetected(float beatTime)
    {
        lastBeatTime = Time.time;
        lastProcessedBeatTime = -1f;

        Debug.Log($"节拍！{beatTime:F2}s (音乐时间)");

        if (hasBufferedInput)
        {
            EvaluateInput();
        }
    }

    void EvaluateInput()
    {
        if (lastClickTime < 0) return;

        string judgeResult = "Miss";

        if (lastBeatTime <= lastProcessedBeatTime)
        {
            judgeResult = "Miss";
            combo = 0;
            Debug.Log("节拍已用过！Miss，连击中断");

            if (cubeMaterial != null) cubeMaterial.color = missColor;
            Invoke(nameof(ResetCubeColor), 0.3f);

            if (comboText != null) comboText.text = "";

            hasBufferedInput = false;
            lastClickTime = -1f;
            return;
        }

        float timeToLastBeat = Mathf.Abs(lastClickTime - lastBeatTime);
        float minDistance = timeToLastBeat;

        if (minDistance <= perfectWindow)
        {
            judgeResult = "Perfect";
            combo++;
            Debug.Log($"Perfect！差值 {minDistance:F3}s 连击 x{combo}");
            lastProcessedBeatTime = lastBeatTime;
        }
        else if (minDistance <= goodWindow)
        {
            judgeResult = "Good";
            combo++;
            Debug.Log($"Good！差值 {minDistance:F3}s 连击 x{combo}");
            lastProcessedBeatTime = lastBeatTime;
        }
        else
        {
            judgeResult = "Miss";
            combo = 0;
            Debug.Log($"Miss！差值 {minDistance:F3}s 连击中断");
        }

        if (combo > maxCombo) maxCombo = combo;

        if (cubeMaterial != null)
        {
            switch (judgeResult)
            {
                case "Perfect":
                    cubeMaterial.color = perfectColor;
                    break;
                case "Good":
                    cubeMaterial.color = goodColor;
                    break;
                case "Miss":
                    cubeMaterial.color = missColor;
                    break;
            }
            Invoke(nameof(ResetCubeColor), 0.3f);
        }

        if (judgeTextPrefab != null && canvasTransform != null)
        {
            GameObject textObj = CreateJudgeText(judgeResult);
            if (textObj != null) StartCoroutine(AnimateFloatingText(textObj));
        }

        if (comboText != null)
        {
            if (combo >= 2)
                comboText.text = $"{combo} Combo!";
            else
                comboText.text = "";
        }

        OnJudgeResult?.Invoke(judgeResult, combo);
        OnComboChanged?.Invoke(combo);

        hasBufferedInput = false;
        lastClickTime = -1f;
    }

    GameObject CreateJudgeText(string judgeResult)
    {
        if (judgeTextPrefab == null) return null;

        GameObject textObj = Instantiate(judgeTextPrefab, canvasTransform);
        TMP_Text tmpText = textObj.GetComponent<TMP_Text>();

        if (tmpText != null)
        {
            tmpText.text = judgeResult;
            switch (judgeResult)
            {
                case "Perfect":
                    tmpText.color = perfectColor;
                    break;
                case "Good":
                    tmpText.color = goodColor;
                    break;
                case "Miss":
                    tmpText.color = missColor;
                    break;
            }
        }

        RectTransform rect = textObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = new Vector2(0, 100);
        }

        return textObj;
    }

    void ResetCubeColor()
    {
        if (cubeMaterial != null) cubeMaterial.color = normalColor;
    }

    System.Collections.IEnumerator AnimateFloatingText(GameObject obj)
    {
        TMP_Text text = obj.GetComponent<TMP_Text>();
        float duration = 0.8f;
        float elapsed = 0f;
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector3 startPos = rect.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rect.anchoredPosition = startPos + Vector3.up * (t * 80f);

            if (text != null)
            {
                Color c = text.color;
                c.a = 1f - t;
                text.color = c;
            }

            yield return null;
        }

        Destroy(obj);
    }

    // ========== 给组长调用的接口 ==========
    public bool TryFlip(out string judgeResult, out int currentCombo)
    {
        if (lastBeatTime > 0 && Time.time - lastBeatTime <= goodWindow)
        {
            float diff = Mathf.Abs(Time.time - lastBeatTime);

            if (diff <= perfectWindow)
                judgeResult = "Perfect";
            else
                judgeResult = "Good";

            currentCombo = ++combo;
            return true;
        }
        else
        {
            judgeResult = "Miss";
            currentCombo = combo = 0;
            return false;
        }
    }

    // ========== 给 NoteController 调用的接口 ==========
    public float[] GetAllBeatTimes()
    {
        return manualBeats;
    }

    public float GetCurrentMusicTime()
    {
        if (audioSource != null)
            return audioSource.time;
        return 0f;
    }

    public void SwitchMusic(int index)
    {
        if (audioSource != null && musicList != null && index >= 0 && index < musicList.Length)
        {
            audioSource.Stop();
            audioSource.clip = musicList[index];
            audioSource.Play();

            ResetDetector();
            beatIndex = 0;
            lastProcessedBeatTime = -1f;
        }
    }

    void ResetDetector()
    {
        combo = 0;
        maxCombo = 0;
        lastBeatTime = 0;
        hasBufferedInput = false;
        lastClickTime = -1f;
    }

    void LoadManualBeats()
    {
        string path = Application.dataPath + "/" + beatFilePath;
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning($"节拍文件不存在: {path}，请先录制节拍");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        BeatData data = JsonUtility.FromJson<BeatData>(json);
        manualBeats = data.beats;

        if (manualBeats != null)
            Debug.Log($"加载了 {manualBeats.Length} 个手动节拍点");
    }

    [System.Serializable]
    public class BeatData
    {
        public float[] beats;
    }
}