using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2BossTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Level2BossTrigger] 有物体进入 Boss 触发器: " + other.name);

        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("[Level2BossTrigger] 玩家碰到 Boss，第二关成功");
            triggered = true;

            Time.timeScale = 1f;
            StoryManager.pendingChapter = 12;
            SceneManager.LoadScene("Duan-start");
        }
    }
}
