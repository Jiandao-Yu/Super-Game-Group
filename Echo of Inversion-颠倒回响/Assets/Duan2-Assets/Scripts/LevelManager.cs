using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("UI References")]
    public Text objectiveText;
    public GameObject bossHealthBar;
    public GameObject warningText;
    public GameObject attackHint;
    public GameObject rhythmRuleHint;
    public Text controlHint;

    [Header("Level Settings")]
    public int currentLevel = 1; // 1 = 第一关, 2 = 第二关

    void Start()
    {
        SetLevel(currentLevel);
    }

    // 设置关卡
    public void SetLevel(int level)
    {
        currentLevel = level;

        if (level == 1)
        {
            SetLevel1();
        }
        else if (level == 2)
        {
            SetLevel2();
        }
    }

    // 第一关设置（教学关）
    void SetLevel1()
    {
        // 目标文字
        objectiveText.text = "Objective: Close the dimensional rift entrance";

        // 隐藏Boss相关UI
        bossHealthBar.SetActive(false);
        warningText.SetActive(false);
        attackHint.SetActive(false);
        rhythmRuleHint.SetActive(false);

        // 操作提示
        controlHint.text = "← →  Move | SPACE Jump | Click to Flip";
    }

    // 第二关设置（Boss战）
    void SetLevel2()
    {
        // 目标文字
        objectiveText.text = "Objective: Defeat the pollution source boss";

        // 显示Boss相关UI
        bossHealthBar.SetActive(true);
        attackHint.SetActive(true);

        // 操作提示（强调节拍）
        controlHint.text = "← →  Move | SPACE Jump | Flip on the BEAT!";
    }

    // 显示Boss出现警告
    public void ShowBossWarning()
    {
        warningText.SetActive(true);
        Invoke("HideWarning", 3f); // 3秒后隐藏
    }

    void HideWarning()
    {
        warningText.SetActive(false);
    }

    // 显示节拍规则教学
    public void ShowRhythmRule()
    {
        rhythmRuleHint.SetActive(true);
        Invoke("HideRhythmRule", 4f);
    }

    void HideRhythmRule()
    {
        rhythmRuleHint.SetActive(false);
    }

    // 更新Boss血量（0-1之间）
    public void UpdateBossHealth(float healthPercent)
    {
        if (bossHealthBar != null)
        {
            Slider slider = bossHealthBar.GetComponent<Slider>();
            if (slider != null)
            {
                slider.value = healthPercent;
            }
        }
    }

    // 显示可攻击提示
    public void ShowAttackHint(bool show)
    {
        attackHint.SetActive(show);
    }

    // 切换到第二关（外部调用）
    public void SwitchToLevel2()
    {
        SetLevel(2);
        ShowRhythmRule();
    }
}