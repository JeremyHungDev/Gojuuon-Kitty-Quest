public interface ISaveSystem
{
    void SaveGame(GameSaveData data);
    GameSaveData LoadGame();
    bool HasSaveFile();
    void DeleteSave();
}
