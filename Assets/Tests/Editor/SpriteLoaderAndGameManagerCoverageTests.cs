using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class SpriteLoaderAndGameManagerCoverageTests
{
    private static void SetPrivateField<T>(object instance, string fieldName, T value)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found");
        field.SetValue(instance, value);
    }

    private static void SetStaticGameManagerInstance(GameManager manager)
    {
        FieldInfo instanceField = typeof(GameManager).GetField("<instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(instanceField, "Could not find GameManager singleton backing field");
        instanceField.SetValue(null, manager);
    }

    private static void EnsureEventsManagerReady()
    {
        if (EventsManager.instance != null) return;
        GameObject go = new GameObject("EventsManager_For_GameManager_Tests");
        EventsManager manager = go.AddComponent<EventsManager>();
        MethodInfo awake = typeof(EventsManager).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
        awake.Invoke(manager, null);
    }

    [Test]
    public void SpriteLoader_LoadMethods_HandleMissingResources()
    {
        SpriteLoader loader = new GameObject("SpriteLoader_Test").AddComponent<SpriteLoader>();

        var towerSprites = loader.LoadTowerSprites("DefinitelyMissingTower", "Idle");
        var clips = loader.LoadEnemyRunClips("DefinitelyMissingEnemy");

        Assert.IsNotNull(towerSprites);
        Assert.AreEqual(0, towerSprites.Count);
        Assert.IsNull(clips.runDown);
        Assert.IsNull(clips.runUp);
        Assert.IsNull(clips.runRight);
        Assert.IsNull(clips.AnyNonNull());
    }

    [Test]
    public void SpriteLoader_ExtractTrailingNumber_WorksForBothCases()
    {
        SpriteLoader loader = new GameObject("SpriteLoader_Number_Test").AddComponent<SpriteLoader>();
        MethodInfo extract = typeof(SpriteLoader).GetMethod("ExtractTrailingNumber", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(extract);

        int withDigits = (int)extract.Invoke(loader, new object[] { "Tower_123" });
        int withoutDigits = (int)extract.Invoke(loader, new object[] { "Tower_NoNumber" });

        Assert.AreEqual(123, withDigits);
        Assert.AreEqual(0, withoutDigits);
    }

    [Test]
    public void GameManager_CoinAndLifeMethods_UpdateStateAndGuards()
    {
        EnsureEventsManagerReady();
        GameManager manager = new GameObject("GameManager_Test").AddComponent<GameManager>();
        SetStaticGameManagerInstance(manager);

        SetPrivateField(manager, "currentCoins", 100);
        SetPrivateField(manager, "currentLives", 2);
        SetPrivateField(manager, "gameActive", true);

        bool spent = manager.TrySpendCoins(30);
        bool spendTooMuch = manager.TrySpendCoins(999);
        manager.AddCoins(5);
        manager.TrySubtractLife(1);
        manager.TrySubtractLife(1);

        Assert.IsTrue(spent);
        Assert.IsFalse(spendTooMuch);
        Assert.AreEqual(75, manager.GetCurrentCoins());
        Assert.AreEqual(0, manager.GetCurrentLives());
        Assert.IsFalse(manager.GetGameActive());
    }
}
