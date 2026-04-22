using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BeatCircleAnimator : MonoBehaviour
{
    [Header("节拍圈设置")]
    public Image innerRing;
    public float beatInterval = 1f;      // 节拍间隔（秒）
    public float animationDuration = 0.8f; // 缩圈动画时长

    [Header("反馈文字")]
    public Text feedbackText;
    public Text perfectText;   // PERFECT 独立文字
    public Text goodText;      // GOOD 独立文字
    public Text missText;      // MISS 独立文字

    [Header("评级阈值")]
    [Tooltip("完美判定范围（内圈缩到最小时的误差范围）")]
    public float perfectThreshold = 0.1f;
    [Tooltip("良好判定范围")]
    public float goodThreshold = 0.2f;

    [Header("其他")]
    public LevelManager levelManager;

    private bool isAnimating = false;
    private float perfectTiming = 0f;  // 记录完美时机

    void Start()
    {
        if (innerRing == null)
            innerRing = GetComponentInChildren<Image>();

        // 初始隐藏所有反馈文字
        HideAllFeedback();

        StartCoroutine(BeatLoop());
    }

    IEnumerator BeatLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(beatInterval);
            StartCoroutine(ShrinkRing());
        }
    }

    IEnumerator ShrinkRing()
    {
        isAnimating = true;
        float elapsed = 0;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(0.2f, 0.2f, 1f);

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            innerRing.rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            // 记录完美时机（缩圈到最小时）
            if (t >= 0.95f && perfectTiming == 0)
            {
                perfectTiming = Time.time;
            }

            yield return null;
        }

        innerRing.rectTransform.localScale = endScale;

        yield return new WaitForSeconds(0.1f);
        innerRing.rectTransform.localScale = startScale;
        isAnimating = false;
        perfectTiming = 0;
    }

    // 检测节拍命中并返回评级
    public string CheckHit()
    {
        float currentScale = innerRing.rectTransform.localScale.x;

        // 计算命中时机与完美时机的差值
        float timeDiff = Mathf.Abs(Time.time - perfectTiming);

        if (currentScale < 0.3f && isAnimating)
        {
            if (timeDiff < perfectThreshold)
                return "PERFECT";
            else if (timeDiff < goodThreshold)
                return "GOOD";
            else
                return "GOOD";
        }

        return "MISS";
    }

    // 处理翻转点击
    public void OnFlip()
    {
        string result = CheckHit();

        switch (result)
        {
            case "PERFECT":
                ShowPerfect();
                if (levelManager != null && levelManager.currentLevel == 2)
                {
                    Debug.Log("PERFECT hit! - Big damage to boss!");
                }
                break;
            case "GOOD":
                ShowGood();
                if (levelManager != null && levelManager.currentLevel == 2)
                {
                    Debug.Log("GOOD hit! - Normal damage to boss!");
                }
                break;
            case "MISS":
                ShowMiss();
                if (levelManager != null && levelManager.currentLevel == 2)
                {
                    levelManager.ShowRhythmRule();
                    Debug.Log("MISS! - No damage, showing hint!");
                }
                break;
        }
    }

    void ShowPerfect()
    {
        HideAllFeedback();
        if (perfectText != null)
        {
            perfectText.gameObject.SetActive(true);
            perfectText.text = "PERFECT!";
            perfectText.color = Color.green;
            Invoke("HideAllFeedback", 0.8f);
        }
        else if (feedbackText != null)
        {
            feedbackText.text = "PERFECT!";
            feedbackText.color = Color.green;
            Invoke("ClearFeedback", 0.8f);
        }
    }

    void ShowGood()
    {
        HideAllFeedback();
        if (goodText != null)
        {
            goodText.gameObject.SetActive(true);
            goodText.text = "GOOD!";
            goodText.color = Color.yellow;
            Invoke("HideAllFeedback", 0.8f);
        }
        else if (feedbackText != null)
        {
            feedbackText.text = "GOOD!";
            feedbackText.color = Color.yellow;
            Invoke("ClearFeedback", 0.8f);
        }
    }

    void ShowMiss()
    {
        HideAllFeedback();
        if (missText != null)
        {
            missText.gameObject.SetActive(true);
            missText.text = "MISS!";
            missText.color = Color.red;
            Invoke("HideAllFeedback", 0.8f);
        }
        else if (feedbackText != null)
        {
            feedbackText.text = "MISS!";
            feedbackText.color = Color.red;
            Invoke("ClearFeedback", 0.8f);
        }
    }

    void HideAllFeedback()
    {
        if (perfectText != null) perfectText.gameObject.SetActive(false);
        if (goodText != null) goodText.gameObject.SetActive(false);
        if (missText != null) missText.gameObject.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    public void SetBeatInterval(float interval)
    {
        beatInterval = interval;
    }
}