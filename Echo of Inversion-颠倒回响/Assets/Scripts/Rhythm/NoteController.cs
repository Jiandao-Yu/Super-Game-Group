using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NoteController : MonoBehaviour
{
    [Header("=== 引用 ===")]
    public RhythmGameCore rhythmCore;
    public GameObject notePrefab;
    public RectTransform judgeLine;
    public Transform noteParent;

    [Header("=== 移动设置 ===")]
    public float spawnOffset = 3f;          // 提前生成时间（秒）
    public float trackLength = 800f;        // 轨道长度（从起点到判定线的像素距离）

    [Header("=== 视觉设置 ===")]
    public Color noteColor = Color.white;
    public float noteSize = 50f;

    // 内部数据
    private List<MovingNote> activeNotes = new List<MovingNote>();
    private float judgeLineX;
    private float startX;
    private bool isSubscribed = false;

    void Start()
    {
        if (rhythmCore == null)
            rhythmCore = GetComponent<RhythmGameCore>();

        if (judgeLine != null)
            judgeLineX = judgeLine.anchoredPosition.x;

        startX = judgeLineX - trackLength;

        // 订阅判定结果事件
        if (!isSubscribed)
        {
            RhythmGameCore.OnJudgeResult += OnJudgeResult;
            isSubscribed = true;
        }
    }

    void Update()
    {
        if (rhythmCore == null) return;

        float currentTime = rhythmCore.GetCurrentMusicTime();
        float[] beats = rhythmCore.GetAllBeatTimes();

        if (beats != null)
        {
            foreach (float beatTime in beats)
            {
                if (beatTime > currentTime && beatTime <= currentTime + spawnOffset)
                {
                    if (!IsNoteAlreadyScheduled(beatTime))
                    {
                        SpawnNote(beatTime);
                    }
                }
            }
        }

        // 更新所有音符位置
        UpdateNotesPosition();
    }

    void UpdateNotesPosition()
    {
        float currentTime = rhythmCore.GetCurrentMusicTime();

        foreach (var note in activeNotes)
        {
            float timeToBeat = note.beatTime - currentTime;
            float progress = timeToBeat / spawnOffset;
            float x = Mathf.Lerp(judgeLineX, startX, progress);
            note.rectTransform.anchoredPosition = new Vector2(x, note.rectTransform.anchoredPosition.y);
        }
    }

    void SpawnNote(float beatTime)
    {
        if (notePrefab == null) return;

        GameObject noteObj = Instantiate(notePrefab, noteParent);
        RectTransform rect = noteObj.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(startX, 0);
        rect.sizeDelta = new Vector2(noteSize, noteSize);

        Image img = noteObj.GetComponent<Image>();
        if (img != null) img.color = noteColor;

        activeNotes.Add(new MovingNote
        {
            gameObject = noteObj,
            rectTransform = rect,
            beatTime = beatTime
        });
    }

    bool IsNoteAlreadyScheduled(float beatTime)
    {
        foreach (var note in activeNotes)
        {
            if (Mathf.Abs(note.beatTime - beatTime) < 0.05f)
                return true;
        }
        return false;
    }

    void OnJudgeResult(string judgeResult, int combo)
    {
        if (activeNotes.Count == 0) return;

        // 找到最接近判定线的音符
        MovingNote closest = null;
        float minDist = float.MaxValue;

        foreach (var note in activeNotes)
        {
            float dist = Mathf.Abs(note.rectTransform.anchoredPosition.x - judgeLineX);
            if (dist < minDist)
            {
                minDist = dist;
                closest = note;
            }
        }

        if (closest != null)
        {
            StartCoroutine(AnimateNoteHit(closest, judgeResult));
        }
    }

    System.Collections.IEnumerator AnimateNoteHit(MovingNote note, string judgeResult)
    {
        Image img = note.gameObject.GetComponent<Image>();
        if (img != null)
        {
            switch (judgeResult)
            {
                case "Perfect": img.color = Color.yellow; break;
                case "Good": img.color = Color.cyan; break;
                default: img.color = Color.red; break;
            }
        }

        float elapsed = 0;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            if (note.rectTransform != null)
            {
                float scale = 1f + elapsed * 3f;
                note.rectTransform.localScale = Vector3.one * scale;
            }
            yield return null;
        }

        activeNotes.Remove(note);
        Destroy(note.gameObject);
    }

    void OnDestroy()
    {
        if (isSubscribed)
        {
            RhythmGameCore.OnJudgeResult -= OnJudgeResult;
        }
    }

    class MovingNote
    {
        public GameObject gameObject;
        public RectTransform rectTransform;
        public float beatTime;
    }
}