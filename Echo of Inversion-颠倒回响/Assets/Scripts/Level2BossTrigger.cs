using UnityEngine;

public class Level2BossTrigger : MonoBehaviour
{
    public Level2GameManager level2GameManager;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (level2GameManager != null)
            {
                level2GameManager.ReachBoss();
            }
        }
    }
}