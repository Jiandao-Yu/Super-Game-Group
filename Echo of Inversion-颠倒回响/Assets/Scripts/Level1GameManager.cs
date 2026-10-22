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

        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        if (failPanel != null) failPanel.SetActive(false);
    }

    void Update()
    {
        if (levelEnded) return;

        if (level1Bgm != null && level1Bgm.isPlaying)
        {
            musicStarted = true;
        }

        // 音乐结束且还没到终点 = 失败
        if (level1Bgm != null && musicStarted && !level1Bgm.isPlaying && !goalReached)
        {
            ShowFail();
        }
    }

    public void ReachGoal()
    {
        if (levelEnded) return;

        goalReached = true;
        levelEnded = true;

        if (level1Bgm != null)
            level1Bgm.Pause();

        Time.timeScale = 1f;
        StoryManager.pendingChapter = 6;
        SceneManager.LoadScene("Duan-start");
    }

    public void ShowFail()
    {
        if (levelEnded) return;

        levelEnded = true;

        if (failPanel != null)
            failPanel.SetActive(true);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (level1Bgm != null)
            level1Bgm.Pause();

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}