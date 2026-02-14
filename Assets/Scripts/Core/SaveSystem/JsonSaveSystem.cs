using System.IO;
using UnityEngine;

public class JsonSaveSystem : ISaveSystem
{
    private readonly string savePath;

    public JsonSaveSystem(string path = null)
    {
        savePath = path ?? Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void SaveGame(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public GameSaveData LoadGame()
    {
        if (!HasSaveFile()) return null;
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
    }
}
