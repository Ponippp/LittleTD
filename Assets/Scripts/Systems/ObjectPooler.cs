using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public static class ObjectPooler
{
    public static Dictionary<string, Component> poolLookup = new Dictionary<string, Component>();
    public static Dictionary<string, Queue<Component>> poolDictionary = new Dictionary<string, Queue<Component>>();
    private static Dictionary<string, int> poolMaxSizeDictionary = new Dictionary<string, int>();
    private static Dictionary<string, GameObject> poolParentObjects = new Dictionary<string, GameObject>();
    private static GameObject OBJECTPOOLS_ROOT;

    public static void EnqueueObject<T>(T item, string name) where T : Component
    {
        if (!item.gameObject.activeSelf) { return; }

        item.transform.position = Vector2.zero;
        poolDictionary[name].Enqueue(item);
        item.gameObject.SetActive(false);

        UpdateNames();
    }

    public static T DequeueObject<T>(string key) where T : Component
    {
        T itemToReturn;
        if (poolDictionary[key].TryDequeue(out var item))
        {
            itemToReturn = (T)item;
        }
        else
        {
            itemToReturn = (T)EnqueueNewInstance(poolLookup[key], key, false);
        }

        UpdateNames();
        return itemToReturn;
    }

    public static T EnqueueNewInstance<T>(T item, string key, bool enqueue = true) where T : Component
    {
        T newInstance = Object.Instantiate(item);
        newInstance.gameObject.SetActive(false);
        newInstance.transform.position = Vector2.zero;

        // Set parent to keep hierarchy organized
        if (poolParentObjects.TryGetValue(key, out GameObject parent))
        {
            newInstance.transform.SetParent(parent.transform);
        }

        poolMaxSizeDictionary[key]++;
        if (enqueue)
        {
            poolDictionary[key].Enqueue(newInstance);
        }

        UpdateNames();
        return newInstance;
    }

    public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string DictionaryEntry) where T : Component
    {
        if (OBJECTPOOLS_ROOT == null)
        {
            OBJECTPOOLS_ROOT = GameObject.Find(Utility.OBJECTPOOLS_PARENT_NAME);
            if (OBJECTPOOLS_ROOT == null) OBJECTPOOLS_ROOT = new GameObject(Utility.OBJECTPOOLS_PARENT_NAME);
        }

        // SETUP OBJECTPOOL PARENT OBJECT
        GameObject poolParent = new GameObject(DictionaryEntry);
        poolParent.transform.SetParent(OBJECTPOOLS_ROOT.transform);
        poolParentObjects[DictionaryEntry] = poolParent;

        // ADD POOL TO POOL DICTIONARY
        poolDictionary.Add(DictionaryEntry, new Queue<Component>());
        poolLookup.Add(DictionaryEntry, pooledItemPrefab);
        poolMaxSizeDictionary[DictionaryEntry] = 0; // Will be incremented by EnqueueNewInstance

        for (int i = 0; i < poolSize; i++)
        {
            EnqueueNewInstance(pooledItemPrefab, DictionaryEntry, true);
        }

        UpdateNames();
    }

    private static void UpdateNames()
    {
        int totalActive = 0;
        int totalObjects = 0;

        foreach (var key in poolDictionary.Keys)
        {
            int inactiveCount = poolDictionary[key].Count;
            int totalInPool = poolMaxSizeDictionary[key];
            int activeInPool = totalInPool - inactiveCount;

            totalActive += activeInPool;
            totalObjects += totalInPool;

            if (poolParentObjects.TryGetValue(key, out GameObject parent))
            {
                parent.name = $"{key} ({activeInPool}/{totalInPool})";
            }
        }

        if (OBJECTPOOLS_ROOT != null)
        {
            OBJECTPOOLS_ROOT.name = $"{Utility.OBJECTPOOLS_PARENT_NAME} ({totalActive}/{totalObjects})";
        }
    }

    public static int GetCurrentActivePoolSize(string key)
    {
        return poolMaxSizeDictionary[key] - poolDictionary[key].Count;
    }

    // New methods for pool destruction
    public static void DestroyPool(string key)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"Attempted to destroy non-existent pool: {key}");
            return;
        }

        // Destroy all pooled objects
        while (poolDictionary[key].Count > 0)
        {
            var obj = poolDictionary[key].Dequeue();
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }

        // Destroy the parent object
        if (poolParentObjects.TryGetValue(key, out GameObject parent))
        {
            Object.Destroy(parent);
            poolParentObjects.Remove(key);
        }

        // Clean up dictionaries
        poolDictionary.Remove(key);
        poolLookup.Remove(key);
        poolMaxSizeDictionary.Remove(key);

        UpdateNames();
    }

    public static void DestroyAllPools()
    {
        List<string> poolKeys = new List<string>(poolDictionary.Keys);
        foreach (string key in poolKeys)
        {
            DestroyPool(key);
        }

        if (OBJECTPOOLS_ROOT != null)
        {
            Object.Destroy(OBJECTPOOLS_ROOT);
        }

        // Clear all dictionaries
        poolDictionary.Clear();
        poolLookup.Clear();
        poolMaxSizeDictionary.Clear();
        poolParentObjects.Clear();

        UpdateNames();
    }

    public static bool PoolExists(string key)
    {
        return poolDictionary.ContainsKey(key);
    }
}
