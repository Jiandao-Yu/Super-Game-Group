using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryData", menuName = "游戏/剧情数据")]
public class StoryData : ScriptableObject
{
    [Serializable]
    public class StoryChapter
    {
        public string title;      // 章节标题
        [TextArea(5, 10)]
        public string content;    // 剧情内容
        public string buttonText; // 按钮文字
    }

    public List<StoryChapter> chapters = new List<StoryChapter>();
}