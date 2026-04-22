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
    public TMP_Text comboText;
    public Transform canvasTransform;
    public GameObject judgeImagePrefab;
    public Sprite perfectSprite;
    public Sprite goodSprite;
    public Sprite missSprite;
    public RectTransform judgeLine;

    [Header("=== UI 进度条 ===")]
    public Image musicProgressBar;
    public ParticleSystem progressParticle;
    [Header("=== 进度条尺寸 ===")]
    public float progressBarWidth = 800f;
    public float progressBarHeight = 20f;
    public float progressBarTopOffset = -30f;

    [Header("=== 打击感增强 ===")]
    public bool enableCameraShake = true;
    public float shakeAmount = 0.1f;
    public float shakeDuration = 0.05f;

    [Header("=== 手动卡点模式 ===")]
    public string beatFilePath = "beats.txt";
    public float beatOffset = 0f;
    private float[] manualBeats;
    private int beatIndex = 0;
    private float lastProcessedBeatTime = -1f;

    // 内部变量
    private float lastBeatTime = 0f;
    private float lastClickTime = -1f;
    private bool hasBufferedInput = false;

    // 事件
    public static event System.Action<string, int> OnJudgeResult;
    public static event System.Action<int> OnComboChanged;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSource != null && musicList != null && musicList.Length > 0)
        {
            audioSource.clip = musicList[0];
        }

        LoadManualBeats();

        // 设置进度条大小和位置
        if (musicProgressBar != null)
        {
            RectTransform rect = musicProgressBar.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(progressBarWidth, progressBarHeight);
                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.anchoredPosition = new Vector2(0, progressBarTopOffset);
            }
        }

        Debug.Log("节奏游戏核心启动 | 手动卡点模式");
    }

    void Update()
    {
        if (audioSource == null) return;

        // 音乐停止时重置节拍索引，不执行后续逻辑
        if (!audioSource.isPlaying)
        {
            if (beatIndex != 0)
            {
                beatIndex = 0;
                lastProcessedBeatTime = -1f;
            }
            return;
        }

        // 更新音乐进度条
        if (musicProgressBar != null && audioSource != null && audioSource.clip != null)
        {
            float progress = audioSource.time / audioSource.clip.length;
            musicProgressBar.fillAmount = progress;

            if (Time.frameCount % 45 == 0)
            {
                PlayProgressParticle();
            }
        }

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

    void PlayProgressParticle()
    {
        if (progressParticle != null && musicProgressBar != null)
        {
            RectTransform progressRect = musicProgressBar.GetComponent<RectTransform>();
            if (progressRect != null)
            {
                float fillAmount = musicProgressBar.fillAmount;
                float barWidth = progressRect.rect.width;
                float particleX = (fillAmount * barWidth) - (barWidth / 2);
                progressParticle.transform.localPosition = new Vector3(particleX, 0, 0);
            }

            var main = progressParticle.main;
            main.startSize = Random.Range(10f, 20f);
            main.startSpeed = Random.Range(1f, 2.5f);

            progressParticle.Play();
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

            if (enableCameraShake)
            {
                StartCoroutine(CameraShake(shakeDuration, shakeAmount));
            }
        }
        else if (minDistance <= goodWindow)
        {
            judgeResult = "Good";
            combo++;
            Debug.Log($"Good！差值 {minDistance:F3}s 连击 x{combo}");
            lastProcessedBeatTime = lastBeatTime;

            if (enableCameraShake)
            {
                StartCoroutine(CameraShake(shakeDuration * 0.5f, shakeAmount * 0.5f));
            }
        }
        else
        {
            judgeResult = "Miss";
            combo = 0;
            Debug.Log($"Miss！差值 {minDistance:F3}s 连击中断");
        }

        if (combo > maxCombo) maxCombo = combo;

        // 判定图片弹窗
        if (judgeImagePrefab != null && canvasTransform != null)
        {
            GameObject imgObj = CreateJudgeImage(judgeResult);
            if (imgObj != null) StartCoroutine(AnimateFloatingImage(imgObj));
        }

        // 连击数字
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

    GameObject CreateJudgeImage(string judgeResult)
    {
        if (judgeImagePrefab == null) return null;

        GameObject imgObj = Instantiate(judgeImagePrefab, canvasTransform);
        Image image = imgObj.GetComponent<Image>();

        if (image != null)
        {
            switch (judgeResult)
            {
                case "Perfect":
                    if (perfectSprite != null) image.sprite = perfectSprite;
                    break;
                case "Good":
                    if (goodSprite != null) image.sprite = goodSprite;
                    break;
                case "Miss":
                    if (missSprite != null) image.sprite = missSprite;
                    break;
            }
        }

        RectTransform rect = imgObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            float judgeLineY = judgeLine != null ? judgeLine.anchoredPosition.y : 0;
            rect.anchoredPosition = new Vector2(0, judgeLineY + 30);
            rect.sizeDelta = new Vector2(400, 200);
        }

        return imgObj;
    }

    System.Collections.IEnumerator AnimateFloatingImage(GameObject obj)
    {
        Image image = obj.GetComponent<Image>();
        float duration = 0.8f;
        float elapsed = 0f;
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector3 startPos = rect.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rect.anchoredPosition = startPos + Vector3.up * (t * 80f);

            if (image != null)
            {
                Color c = image.color;
                c.a = 1f - t;
                image.color = c;
            }

            yield return null;
        }

        Destroy(obj);
    }

    // ========== 相机震动协程 ==========
    System.Collections.IEnumerator CameraShake(float duration, float magnitude)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) yield break;

        Vector3 originalPos = mainCam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            mainCam.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCam.transform.localPosition = originalPos;
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
        }
    }

    void ResetDetector()
    {
        combo = 0;
        maxCombo = 0;
        lastBeatTime = 0;
        hasBufferedInput = false;
        lastClickTime = -1f;
        beatIndex = 0;
        lastProcessedBeatTime = -1f;
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