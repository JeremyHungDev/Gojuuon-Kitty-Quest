using NUnit.Framework;

public class GameManagerTests
{
    [Test]
    public void GameState_DefaultsToMainMenu()
    {
        var stateManager = new GameStateManager();

        Assert.AreEqual(GameState.MainMenu, stateManager.CurrentState);
    }

    [Test]
    public void GameState_TransitionsCorrectly()
    {
        var stateManager = new GameStateManager();

        stateManager.ChangeState(GameState.StageSelect);
        Assert.AreEqual(GameState.StageSelect, stateManager.CurrentState);

        stateManager.ChangeState(GameState.Playing);
        Assert.AreEqual(GameState.Playing, stateManager.CurrentState);
    }

    [Test]
    public void PlayerManager_DefaultsToPlayer1()
    {
        var playerManager = new PlayerManager();

        Assert.AreEqual(0, playerManager.ActivePlayerIndex);
    }

    [Test]
    public void PlayerManager_SwitchPlayer_TogglesIndex()
    {
        var playerManager = new PlayerManager();
        playerManager.SetPlayerCount(2);

        playerManager.SwitchPlayer();
        Assert.AreEqual(1, playerManager.ActivePlayerIndex);

        playerManager.SwitchPlayer();
        Assert.AreEqual(0, playerManager.ActivePlayerIndex);
    }

    [Test]
    public void PlayerManager_SinglePlayer_DoesNotSwitch()
    {
        var playerManager = new PlayerManager();
        playerManager.SetPlayerCount(1);

        playerManager.SwitchPlayer();
        Assert.AreEqual(0, playerManager.ActivePlayerIndex);
    }

    [Test]
    public void PlayerManager_IsTwoPlayer_ReturnsCorrectly()
    {
        var playerManager = new PlayerManager();

        Assert.IsFalse(playerManager.IsTwoPlayer);

        playerManager.SetPlayerCount(2);
        Assert.IsTrue(playerManager.IsTwoPlayer);
    }
}
