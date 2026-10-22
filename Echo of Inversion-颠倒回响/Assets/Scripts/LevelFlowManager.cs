using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFlowManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameplayPanel;
    public GameObject victoryPanel;
    public GameObject failPanel;
    public GameObject dialoguePanel;

    [Header("Scene Flow")]
    public bool isLevel1 = false;
    public string nextSceneName = "";

    private bool levelEnded = false;

    void Start()
    {
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        Time.timeScale = 1f;
        levelEnded = false;
    }

    public void ShowFail()
    {
        if (levelEnded) return;
        levelEnded = true;

        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ShowVictory()
    {
        if (levelEnded) return;
        levelEnded = true;

        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoNextStepAfterVictory()
    {
        Time.timeScale = 1f;

        if (isLevel1)
        {
            // 第一关胜利后，不直接进第二关，先显示剧情页面
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (dialoguePanel != null) dialoguePanel.SetActive(true);
        }
        else
        {
            // 第二关胜利后，直接停留在胜利界面，或者你后面也可以改去最终结算场景
        }
    }

    public void StartNextLevelFromDialogue()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}
