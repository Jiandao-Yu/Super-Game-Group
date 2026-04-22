using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject dialoguePanel;
    public GameObject dialogTitle;
    public GameObject dialogText;
    public GameObject buttonText;
    public Button continueButton;

    [Header("Story Data")]
    public List<StoryChapter> chapters = new List<StoryChapter>();

    [Header("Game Manager References")]
    public GameObject gameplayPanel;
    public GameObject startPanel;

    // 用于跨场景回到 Duan-start 后，自动跳到指定章节
    public static int pendingChapter = -1;

    private int currentChapterIndex = 0;
    private bool isPlayingStory = false;
    private System.Action onStoryComplete;

    [System.Serializable]
    public class StoryChapter
    {
        public string title;
        [TextArea(5, 10)]
        public string content;
        public string buttonText;
    }

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueButtonClick);
        }

        if (chapters.Count == 0)
            SetupFullStory();

        // 如果是从第一关/第二关返回这个场景，自动续到指定章节
        if (pendingChapter >= 0)
        {
            int chapterToStart = pendingChapter;
            pendingChapter = -1;

            if (startPanel != null)
                startPanel.SetActive(false);

            if (gameplayPanel != null)
                gameplayPanel.SetActive(false);

            StartStoryInternal(chapterToStart);
        }
        else
        {
            // 正常进入开始场景时，显示开始面板
            if (startPanel != null)
                startPanel.SetActive(true);

            if (gameplayPanel != null)
                gameplayPanel.SetActive(false);
        }
    }

    void SetupFullStory()
    {
        chapters.Clear();

        // Chapter 0: Opening Story
        StoryChapter chapter0 = new StoryChapter();
        chapter0.title = "The Sequence Thicket";
        chapter0.content = "A space at the edge of the universe maintained by rhythmic energy.\n\n" +
                  "Originally harmonious, residents lived by rhythm resonance.\n\n" +
                  "However, a being from another dimension tore through the dimensional membrane,\n\n" +
                  "creating the \"Upside Down World\" mirroring the normal world,\n\n" +
                  "and releasing pollution into the normal world.\n\n" +
                  "You - the \"Inversion Walker\" who can sense rhythm and control gravity,\n\n" +
                  "must close the dimensional rift and save the forest.";
        chapter0.buttonText = "[Start Level 1]";
        chapters.Add(chapter0);

        // Chapter 1: Level 1 Goal
        StoryChapter chapter1 = new StoryChapter();
        chapter1.title = "Level 1 - Tutorial";
        chapter1.content = "Goal: Go and close the dimensional rift entrance\n\n" +
                  "Tutorial:\n" +
                  "- Left/Right Arrows or A/D : Move\n" +
                  "- Space : Jump\n" +
                  "- Mouse Left Click : Flip Gravity (No rhythm required)";
        chapter1.buttonText = "[Start Challenge]";
        chapters.Add(chapter1);

        // Chapter 2: Movement Tutorial
        StoryChapter chapter2 = new StoryChapter();
        chapter2.title = "Movement Tutorial";
        chapter2.content = "Use Left/Right Arrows or A/D to move\n\nExplore the level and find the dimensional rift!";
        chapter2.buttonText = "[Got it]";
        chapters.Add(chapter2);

        // Chapter 3: Jump Tutorial
        StoryChapter chapter3 = new StoryChapter();
        chapter3.title = "Jump Tutorial";
        chapter3.content = "Press Space to jump!\n\nJump over obstacles and keep moving forward.";
        chapter3.buttonText = "[Got it]";
        chapters.Add(chapter3);

        // Chapter 4: Gravity Flip Tutorial
        StoryChapter chapter4 = new StoryChapter();
        chapter4.title = "Gravity Flip Tutorial";
        chapter4.content = "Press Mouse Left Click to flip gravity!\n\nNo rhythm required in this level.\n\nAfter flipping, you can walk on the ceiling.";
        chapter4.buttonText = "[Try Flip]";
        chapters.Add(chapter4);

        // Chapter 5: Flip Success
        StoryChapter chapter5 = new StoryChapter();
        chapter5.title = "Flip Success";
        chapter5.content = "Flip successful!\n\nYou can now walk on the ceiling.\n\nContinue forward and close the rift entrance.";
        chapter5.buttonText = "[Continue]";
        chapters.Add(chapter5);

        // Chapter 6: Level 1 Complete
        StoryChapter chapter6 = new StoryChapter();
        chapter6.title = "Rift Closed";
        chapter6.content = "The rift entrance has been closed.\n\nBut the pollution has not disappeared...\n\nThe real pollution source comes from deep within the Upside Down World.\n\nYou must enter the other dimension and face the pollution source.";
        chapter6.buttonText = "[Enter Level 2]";
        chapters.Add(chapter6);

        // Chapter 7: Level 2 Goal
        StoryChapter chapter7 = new StoryChapter();
        chapter7.title = "Level 2 - Upside Down World";
        chapter7.content = "Goal: Enter the Upside Down World and defeat the pollution source Boss\n\n!!! Rhythm Rule Activated !!!\n\nGravity flip must hit the music beat!\n\nWatch the beat ring and click when the circle shrinks completely.";
        chapter7.buttonText = "[Start Challenge]";
        chapters.Add(chapter7);

        // Chapter 8: Rhythm Tutorial
        StoryChapter chapter8 = new StoryChapter();
        chapter8.title = "Rhythm Tutorial";
        chapter8.content = "How to hit the beat?\n\n1. Watch the circular beat ring on screen\n2. Wait for the outer ring to shrink inward\n3. Click when the rings overlap\n\nHit shows 'Perfect', Miss shows 'Miss'";
        chapter8.buttonText = "[Got it]";
        chapters.Add(chapter8);

        // Chapter 9: Boss Appears
        StoryChapter chapter9 = new StoryChapter();
        chapter9.title = "!!! WARNING !!!";
        chapter9.content = "The pollution source appears!\n\nAvoid attacks and wait for the flip timing.\n\nDuring the attack window, hit the beat to deal damage!";
        chapter9.buttonText = "[Fight Boss]";
        chapters.Add(chapter9);

        // Chapter 10: Attack Window
        StoryChapter chapter10 = new StoryChapter();
        chapter10.title = "Attack Timing";
        chapter10.content = "Flip now!\n\nHit the beat to deal damage!\n\nConsecutive hits deal higher damage!";
        chapter10.buttonText = "[Attack]";
        chapters.Add(chapter10);

        // Chapter 11: Miss Tutorial
        StoryChapter chapter11 = new StoryChapter();
        chapter11.title = "Missed Beat";
        chapter11.content = "Missed the beat!\n\nWait for the next beat, click when the ring shrinks completely.\n\nKeep the rhythm, you can do it!";
        chapter11.buttonText = "[Try Again]";
        chapters.Add(chapter11);

        // Chapter 12: Victory
        StoryChapter chapter12 = new StoryChapter();
        chapter12.title = "Victory";
        chapter12.content = "The pollution source has been defeated!\n\nThe Sequence Thicket is recovering...\n\nThe forest residents feel the rhythm resonance again.\n\nYou saved this space, Inversion Walker.\n\n[Game Complete!]";
        chapter12.buttonText = "[Back to Menu]";
        chapters.Add(chapter12);
    }

    // ===== UI 按钮入口 =====

    public void StartOpeningStory()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        StartStoryInternal(0);
    }

    public void StartLevelCompleteStory()
    {
        StartStoryInternal(6);
    }

    public void StartVictoryStory()
    {
        StartStoryInternal(12);
    }

    private void StartStoryInternal(int chapterIndex, System.Action onComplete = null)
    {
        if (chapterIndex < 0 || chapterIndex >= chapters.Count)
        {
            Debug.LogWarning("Story chapter index out of range: " + chapterIndex);
            return;
        }

        currentChapterIndex = chapterIndex;
        onStoryComplete = onComplete;
        ShowCurrentChapter();
        isPlayingStory = true;
        Time.timeScale = 0f;
    }

    public void StartStory(int chapterIndex, System.Action onComplete = null)
    {
        StartStoryInternal(chapterIndex, onComplete);
    }

    void ShowCurrentChapter()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        StoryChapter chapter = chapters[currentChapterIndex];

        if (dialogTitle != null)
        {
            TextMeshProUGUI tmp = dialogTitle.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = chapter.title;
            else Debug.LogError("DialogTitle does not have TextMeshProUGUI component");
        }

        if (dialogText != null)
        {
            TextMeshProUGUI tmp = dialogText.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = chapter.content;
            else Debug.LogError("DialogText does not have TextMeshProUGUI component");
        }

        if (buttonText != null)
        {
            TextMeshProUGUI tmp = buttonText.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = chapter.buttonText;
            else Debug.LogError("ButtonText does not have TextMeshProUGUI component");
        }
    }

    void OnContinueButtonClick()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Time.timeScale = 1f;

        bool wasPlayingStory = isPlayingStory;
        isPlayingStory = false;

        if (onStoryComplete != null)
        {
            var callback = onStoryComplete;
            onStoryComplete = null;
            callback?.Invoke();
        }

        if (wasPlayingStory)
        {
            HandleChapterAction(currentChapterIndex);
        }
    }

    void HandleChapterAction(int chapterIndex)
    {
        switch (chapterIndex)
        {
            case 0:
                Debug.Log("[Story] Opening story ended, showing level 1 goal");
                StartStoryInternal(1);
                break;

            case 1:
                Debug.Log("[Story] Level 1 goal shown, loading level 1");
                StartGameLevel(1);
                break;

            case 2:
                Debug.Log("[Story] Movement tutorial ended");
                break;

            case 3:
                Debug.Log("[Story] Jump tutorial ended");
                break;

            case 4:
                Debug.Log("[Story] Gravity flip tutorial ended");
                break;

            case 5:
                Debug.Log("[Story] Flip success tutorial ended");
                break;

            case 6:
                Debug.Log("[Story] Level 1 complete, showing level 2 goal");
                StartStoryInternal(7);
                break;

            case 7:
                Debug.Log("[Story] Level 2 goal shown, loading level 2");
                StartGameLevel(2);
                EnableRhythmMode();
                break;

            case 8:
                Debug.Log("[Story] Rhythm tutorial ended");
                break;

            case 9:
                Debug.Log("[Story] Boss appears, starting battle");
                StartBossBattle();
                break;

            case 10:
                Debug.Log("[Story] Attack window opened");
                EnableAttackWindow();
                break;

            case 11:
                Debug.Log("[Story] Miss tutorial ended");
                break;

            case 12:
                Debug.Log("[Story] Game complete, returning to menu");
                ReturnToMainMenu();
                break;

            default:
                Debug.Log("[Story] Unknown chapter index: " + chapterIndex);
                break;
        }
    }

    void StartGameLevel(int level)
    {
        Time.timeScale = 1f;

        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        if (level == 1)
        {
            Debug.Log("Loading Level 1...");
            SceneManager.LoadScene("SCN_Level01_Test");
        }
        else if (level == 2)
        {
            Debug.Log("Loading Level 2...");
            SceneManager.LoadScene("scene2");
        }
    }

    void StartLevelGameplay()
    {
        Debug.Log("Starting level gameplay");
    }

    void EnableRhythmMode()
    {
        Debug.Log("Rhythm mode enabled");
    }

    void StartBossBattle()
    {
        Debug.Log("Boss battle started");
    }

    void EnableAttackWindow()
    {
        Debug.Log("Boss attack window opened");
    }

    void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Duan-start");
    }

    public void ShowMovementTutorial()
    {
        if (!HasShownTutorial("Movement"))
        {
            StartStoryInternal(2);
            MarkTutorialShown("Movement");
        }
    }

    public void ShowJumpTutorial()
    {
        if (!HasShownTutorial("Jump"))
        {
            StartStoryInternal(3);
            MarkTutorialShown("Jump");
        }
    }

    public void ShowGravityTutorial()
    {
        if (!HasShownTutorial("Gravity"))
        {
            StartStoryInternal(4);
            MarkTutorialShown("Gravity");
        }
    }

    public void ShowFlipSuccess()
    {
        if (!HasShownTutorial("FlipSuccess"))
        {
            StartStoryInternal(5);
            MarkTutorialShown("FlipSuccess");
        }
    }

    public void OnFirstLevelComplete()
    {
        StartStoryInternal(6);
    }

    public void ShowRhythmTutorial()
    {
        if (!HasShownTutorial("Rhythm"))
        {
            StartStoryInternal(8);
            MarkTutorialShown("Rhythm");
        }
    }

    public void ShowMissTutorial()
    {
        if (!HasShownTutorial("Miss"))
        {
            StartStoryInternal(11);
            MarkTutorialShown("Miss");
        }
    }

    public void ShowBossAppear()
    {
        if (!HasShownTutorial("BossAppear"))
        {
            StartStoryInternal(9);
            MarkTutorialShown("BossAppear");
        }
    }

    public void ShowAttackWindowTutorial()
    {
        if (!HasShownTutorial("AttackWindow"))
        {
            StartStoryInternal(10);
            MarkTutorialShown("AttackWindow");
        }
    }

    public void OnGameComplete()
    {
        StartStoryInternal(12);
    }

    private bool HasShownTutorial(string tutorialName)
    {
        return PlayerPrefs.GetInt("Tutorial_" + tutorialName, 0) == 1;
    }

    private void MarkTutorialShown(string tutorialName)
    {
        PlayerPrefs.SetInt("Tutorial_" + tutorialName, 1);
        PlayerPrefs.Save();
    }

    public void ResetAllTutorials()
    {
        string[] tutorials = { "Movement", "Jump", "Gravity", "FlipSuccess", "Rhythm", "Miss", "BossAppear", "AttackWindow" };
        foreach (string tutorial in tutorials)
        {
            PlayerPrefs.DeleteKey("Tutorial_" + tutorial);
        }
        PlayerPrefs.Save();
        Debug.Log("All tutorials reset");
    }
}