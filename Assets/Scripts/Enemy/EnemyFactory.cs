using UnityEngine;

// TODO STUB
public static class EnemyFactory
{
    public static GameObject CreateEnemy(EnemyType type, Vector3 spawnPos)
    {
        GameObject prefab = GetPrefabByType(type);
        
        //instaniate enemy prefab at given pos w/ default rotation
        GameObject enemyObj = Object.Instantiate(prefab, spawnPos, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        // 3. Inject the right Strategy based  on the type!
        if (type == EnemyType.SCRUB) enemy.SetMovementStrategy(new GroundStrategy(enemy, Object.FindAnyObjectByType<AStar>()));
        else if (type == EnemyType.BALLER) enemy.SetMovementStrategy(new FlyingStrategy(enemy)); //don't need astar b/c it just flies from A to B
        else Debug.LogError("Unknown EnemyType: " + type);

        return enemyObj;
    }

    // TODO STUB
    private static GameObject GetPrefabByType(EnemyType type)
    {
        return null;
    }
}
