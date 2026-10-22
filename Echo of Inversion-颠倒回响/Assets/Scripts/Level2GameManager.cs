using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2GameManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource level2Bgm;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip victoryClip;
    public AudioClip failClip;

    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject victoryPanel;
    public GameObject failPanel;

    private bool levelEnded = false;
    private bool bossReached = false;
    private bool musicStarted = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
    }

    void Update()
    {
        if (levelEnded) return;

        if (level2Bgm != null && level2Bgm.isPlaying)
        {
            musicStarted = true;
        }

        // 音乐结束且还没碰到 Boss = 失败
        if (level2Bgm != null && musicStarted && !level2Bgm.isPlaying && !bossReached)
        {
            ShowFail();
        }
    }

    public void ReachBoss()
    {
        if (levelEnded) return;

        bossReached = true;
        levelEnded = true;

        // 🎵 播放胜利音效
        if (sfxSource != null && victoryClip != null)
        {
            sfxSource.PlayOneShot(victoryClip);
        }

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (level2Bgm != null)
            level2Bgm.Pause();

        Time.timeScale = 0f;
    }

    public void ShowFail()
    {
        if (levelEnded) return;

        levelEnded = true;

        // 🎵 播放失败音效
        if (sfxSource != null && failClip != null)
        {
            sfxSource.PlayOneShot(failClip);
        }

        if (failPanel != null)
            failPanel.SetActive(true);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (level2Bgm != null)
            level2Bgm.Pause();

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ContinueAfterVictory()
    {
        Time.timeScale = 1f;
        StoryManager.pendingChapter = 12;
        SceneManager.LoadScene("Duan-start");
    }
}