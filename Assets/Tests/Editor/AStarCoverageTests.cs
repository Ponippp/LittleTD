using NUnit.Framework;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarCoverageTests
{
    private static Tilemap CreateFilledTilemap(int width, int height)
    {
        GameObject gridObject = new GameObject("TestGrid", typeof(Grid));
        GameObject tilemapObject = new GameObject("TestTilemap", typeof(Tilemap), typeof(TilemapRenderer));
        tilemapObject.transform.SetParent(gridObject.transform, false);

        Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();
        Tile tile = ScriptableObject.CreateInstance<Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        return tilemap;
    }

    private static void EnsureEventsManagerExists()
    {
        if (EventsManager.instance == null)
        {
            new GameObject("EventsManager_Test").AddComponent<EventsManager>();
        }
    }

    private static void SetRunAStarInstantly(AStar aStar, bool value)
    {
        FieldInfo field = typeof(AStar).GetField("_runAStarInstantly", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, "Could not find private _runAStarInstantly field on AStar");
        field.SetValue(aStar, value);
    }

    [Test]
    public void GetGridNode_InBounds_ReturnsNode()
    {
        EnsureEventsManagerExists();
        Tilemap tilemap = CreateFilledTilemap(3, 3);

        AStar aStar = new GameObject("AStar_Test").AddComponent<AStar>();
        aStar.SetupAStar(3, 3, Vector3.zero, tilemap);

        AStarNode node = aStar.GetGridNode(1, 1);

        Assert.IsNotNull(node);
        Assert.AreEqual(1, node.x);
        Assert.AreEqual(1, node.y);
    }

    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    [TestCase(99, 0)]
    [TestCase(0, 99)]
    public void GetGridNode_OutOfBounds_ReturnsNull(int x, int y)
    {
        EnsureEventsManagerExists();
        Tilemap tilemap = CreateFilledTilemap(2, 2);

        AStar aStar = new GameObject("AStar_OutOfBounds_Test").AddComponent<AStar>();
        aStar.SetupAStar(2, 2, Vector3.zero, tilemap);

        Assert.IsNull(aStar.GetGridNode(x, y));
    }

    [Test]
    public void TryRunAStar_NullTarget_ReturnsNull()
    {
        EnsureEventsManagerExists();
        Tilemap tilemap = CreateFilledTilemap(2, 2);

        AStar aStar = new GameObject("AStar_NullTarget_Test").AddComponent<AStar>();
        SetRunAStarInstantly(aStar, true);
        aStar.SetupAStar(2, 2, Vector3.zero, tilemap);

        AStarNode start = aStar.GetGridNode(0, 0);
        var result = aStar.TryRunAStar(start, null);

        Assert.IsNull(result);
    }

    [Test]
    public void TryRunAStar_NullStart_ReturnsNull()
    {
        EnsureEventsManagerExists();
        Tilemap tilemap = CreateFilledTilemap(2, 2);

        AStar aStar = new GameObject("AStar_NullStart_Test").AddComponent<AStar>();
        SetRunAStarInstantly(aStar, true);
        aStar.SetupAStar(2, 2, Vector3.zero, tilemap);

        AStarNode target = aStar.GetGridNode(0, 0);
        var result = aStar.TryRunAStar(null, target);

        Assert.IsNull(result);
    }

    [Test]
    public void TryRunAStar_StartEqualsTarget_ReturnsSinglePointPath()
    {
        EnsureEventsManagerExists();
        Tilemap tilemap = CreateFilledTilemap(2, 2);

        AStar aStar = new GameObject("AStar_SinglePoint_Test").AddComponent<AStar>();
        SetRunAStarInstantly(aStar, true);
        aStar.SetupAStar(2, 2, Vector3.zero, tilemap);

        AStarNode node = aStar.GetGridNode(1, 1);
        var result = aStar.TryRunAStar(node, node);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(new Vector3(1.5f, 1.5f, 0f), result[0]);
    }
}
