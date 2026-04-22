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
    public float noteSpeed = 300f;           // 移动速度（像素/秒）
    public float spawnOffset = 3f;           // 提前生成时间（秒）
    public float missDistance = 150f;        // 超出判定线多少像素后算 Miss

    [Header("=== 视觉设置 ===")]
    public Color noteColor = Color.white;
    public float noteSize = 50f;

    private List<MovingNote> activeNotes = new List<MovingNote>();
    private float judgeLineX;
    private bool isSubscribed = false;

    void Start()
    {
        if (rhythmCore == null)
            rhythmCore = GetComponent<RhythmGameCore>();

        if (judgeLine != null)
            judgeLineX = judgeLine.anchoredPosition.x;

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

        // 生成新音符
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

        // 更新音符位置（用速度移动）
        float deltaTime = Time.deltaTime;
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            MovingNote note = activeNotes[i];

            // 向右移动
            float newX = note.rectTransform.anchoredPosition.x + noteSpeed * deltaTime;
            note.rectTransform.anchoredPosition = new Vector2(newX, note.rectTransform.anchoredPosition.y);

            // 超出判定线右侧 missDistance，且未被判定，算 Miss
            if (newX > judgeLineX + missDistance && !note.wasJudged)
            {
                Debug.Log($"音符 {note.beatTime:F2}s 未击中，Miss");
                activeNotes.RemoveAt(i);
                Destroy(note.gameObject);
            }
            // 超出屏幕右侧太远也销毁
            else if (newX > judgeLineX + 500f)
            {
                activeNotes.RemoveAt(i);
                Destroy(note.gameObject);
            }
        }
    }

    void SpawnNote(float beatTime)
    {
        if (notePrefab == null) return;

        // 计算初始 X 位置：根据剩余时间算出应该在的位置
        float currentTime = rhythmCore.GetCurrentMusicTime();
        float timeToBeat = beatTime - currentTime;
        float startX = judgeLineX - noteSpeed * timeToBeat;

        // 限制最大初始位置（不要太靠左）
        startX = Mathf.Max(startX, judgeLineX - 1200f);

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
            beatTime = beatTime,
            wasJudged = false
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
            if (note.wasJudged) continue;

            float dist = Mathf.Abs(note.rectTransform.anchoredPosition.x - judgeLineX);
            if (dist < minDist)
            {
                minDist = dist;
                closest = note;
            }
        }

        if (closest != null)
        {
            closest.wasJudged = true;
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
        public bool wasJudged = false;
    }
}