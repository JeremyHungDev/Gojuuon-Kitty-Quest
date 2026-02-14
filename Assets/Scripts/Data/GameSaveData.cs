using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public List<PlayerData> players;
    public string lastPlayDate;
    public int totalPlayTimeSeconds;
}

[Serializable]
public class PlayerData
{
    public string playerName;
    public int currentLevel;
    public List<string> learnedKana;
    public List<string> collectedRewards;
    public List<MinigameScore> scores;

    public PlayerData()
    {
        learnedKana = new List<string>();
        collectedRewards = new List<string>();
        scores = new List<MinigameScore>();
    }
}

[Serializable]
public class MinigameScore
{
    public string minigameId;
    public string kanaId;
    public int bestScore;
    public int attempts;
}
