using UnityEngine;

public class Level1GoalTrigger : MonoBehaviour
{
    public Level1GameManager level1GameManager;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Level1GoalTrigger] 有物体进入触发器: " + other.name);

        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("[Level1GoalTrigger] 玩家碰到 zhong1");
            triggered = true;

            if (level1GameManager != null)
            {
                level1GameManager.ReachGoal();
            }
            else
            {
                Debug.LogError("[Level1GoalTrigger] level1GameManager 没有拖进去");
            }
        }
    }
}