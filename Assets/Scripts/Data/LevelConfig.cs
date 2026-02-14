using System;
using System.Collections.Generic;

[Serializable]
public class LevelConfig
{
    public int levelId;
    public string name;
    public string sceneName;
    public string description;
    public List<string> kanaGroups;
    public string reward;
    public string rewardSprite;
    public string unlockCondition;
    public List<string> minigames;
}

[Serializable]
public class LevelConfigWrapper
{
    public List<LevelConfig> levels;
}
