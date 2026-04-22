using UnityEngine;

public class InputTester : MonoBehaviour
{
    public BeatCircleAnimator beatCircle;
    public LevelManager levelManager;

    void Update()
    {
        // 鼠标左键 - 翻转
        if (Input.GetMouseButtonDown(0))
        {
            if (beatCircle != null)
            {
                beatCircle.OnFlip();
            }
        }

        // 数字键1 - 切换到第一关
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (levelManager != null)
                levelManager.SetLevel(1);
        }

        // 数字键2 - 切换到第二关
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (levelManager != null)
                levelManager.SwitchToLevel2();
        }

        // 数字键3 - 显示Boss警告
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (levelManager != null)
                levelManager.ShowBossWarning();
        }

        // 数字键4 - 显示可攻击提示
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (levelManager != null)
                levelManager.ShowAttackHint(true);
        }

        // 数字键5 - 隐藏可攻击提示
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (levelManager != null)
                levelManager.ShowAttackHint(false);
        }
    }
}