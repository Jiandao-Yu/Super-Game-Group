using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject gameplayPanel;
    public GameObject victoryPanel;
    public GameObject failPanel;

    void Start()
    {
        ShowStart();
    }

    public void ShowStart()
    {
        startPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        victoryPanel.SetActive(false);
        failPanel.SetActive(false);
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }

    public void ShowFail()
    {
        failPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }
}