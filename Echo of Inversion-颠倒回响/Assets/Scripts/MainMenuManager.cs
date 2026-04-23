using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("StartGame button clicked!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("SCN_Level01_Test");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}