using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1GameManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource level1Bgm;

    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject failPanel;

    private bool levelEnded = false;
    private bool goalReached = false;
    private bool musicStarted = false;

    void Start()
    {
        Time.timeScale = 1f;

        Debug.Log("[Level1GameManager] Start");

        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);
        else
            Debug.LogWarning("[Level1GameManager] gameplayPanel 没有拖进去");

        if (failPanel != null)
            failPanel.SetActive(false);
        else
            Debug.LogWarning("[Level1GameManager] failPanel 没有拖进去");

        if (level1Bgm == null)
            Debug.LogWarning("[Level1GameManager] level1Bgm 没有拖进去");
    }

    void Update()
    {
        if (levelEnded) return;

        if (level1Bgm != null && level1Bgm.isPlaying)
        {
            musicStarted = true;
        }

        // 音乐播完且还没碰到 zhong1 = 失败
        if (level1Bgm != null && musicStarted && !level1Bgm.isPlaying && !goalReached)
        {
            Debug.Log("[Level1GameManager] 音乐结束，第一关失败");
            ShowFail();
        }
    }

    public void ReachGoal()
    {
        if (levelEnded) return;

        Debug.Log("[Level1GameManager] ReachGoal 被触发，第一关成功");

        goalReached = true;
        levelEnded = true;

        if (level1Bgm != null)
        {
            level1Bgm.Pause();
            Debug.Log("[Level1GameManager] 音乐已暂停");
        }

        Time.timeScale = 1f;

        // 直接回 Duan-start，并进入第6章
        StoryManager.pendingChapter = 6;
        SceneManager.LoadScene("Duan-start");
    }

    public void ShowFail()
    {
        if (levelEnded) return;

        Debug.Log("[Level1GameManager] ShowFail 被触发");

        levelEnded = true;

        if (gameplayPanel != null)
        {
            gameplayPanel.SetActive(false);
            Debug.Log("[Level1GameManager] gameplayPanel 已关闭");
        }

        if (failPanel != null)
        {
            failPanel.SetActive(true);
            Debug.Log("[Level1GameManager] failPanel 已打开");
        }

        if (level1Bgm != null)
        {
            level1Bgm.Pause();
            Debug.Log("[Level1GameManager] 音乐已暂停");
        }

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Debug.Log("[Level1GameManager] RestartLevel");

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}