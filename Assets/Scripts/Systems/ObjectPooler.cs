using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Imported from BlackWinter. It is unchanged except for 'UpdateNames' method which I added for ease of use in the editor. 
/// I am confident it works. Look to Projectile to see example usage of it.
/// </summary>
public static class ObjectPooler
{
    public static Dictionary<string, Component> poolLookup = new Dictionary<string, Component>(); //string is component name, component is the type that that pool is comprised of like projectile or tower
    public static Dictionary<string, Queue<Component>> poolDictionary = new Dictionary<string, Queue<Component>>(); //queue is where objects go when inactive
    private static Dictionary<string, int> poolMaxSizeDictionary = new Dictionary<string, int>(); //using dictionaries b/c we have multuiple object pools so we need to specifiy which one using the key
    private static Dictionary<string, GameObject> poolParentObjects = new Dictionary<string, GameObject>();
    private static GameObject OBJECTPOOLS_ROOT; //empty field where we can place an empty game object, otherwise we create one during run time

    //make objects inactive and enqueue them when we're done using them temporarily; if obj is alr inactive, no need to do anything, else add to queue and set to inactive
    public static void EnqueueObject<T>(T item, string name) where T : Component
    {
        if (!item.gameObject.activeSelf) { return; } 

        item.transform.position = Vector2.zero;
        poolDictionary[name].Enqueue(item);
        item.gameObject.SetActive(false);

        UpdateNames(); //updates "ObjectPools (x/100)" in inspector
    }

    public static T DequeueObject<T>(string key) where T : Component
    {
        T itemToReturn;
        if (poolDictionary[key].TryDequeue(out var item)) //try to access obj from queue
        {
            itemToReturn = (T)item;
        }
        else //if queue is empty, enqueue a new obj instance
        {
            itemToReturn = (T)EnqueueNewInstance(poolLookup[key], key, false);
        }

        UpdateNames(); //updates "ObjectPools (x/100)" in inspector
        return itemToReturn;
    }

    //run this when queue is empty and you need another object, like a projectile
    public static T EnqueueNewInstance<T>(T item, string key, bool enqueue = true) where T : Component
    {
        T newInstance = Object.Instantiate(item);
        newInstance.gameObject.SetActive(false);
        newInstance.transform.position = Vector2.zero; //put all objs at same palce for easy debugging

        //Set parent to keep hierarchy organized
        if (poolParentObjects.TryGetValue(key, out GameObject parent))
        {
            newInstance.transform.SetParent(parent.transform); //puts object beneath its respective obj pool in inspector hierarchy
        }

        poolMaxSizeDictionary[key]++; //increment max pool size of the pool specified by the key
        if (enqueue)
        {
            poolDictionary[key].Enqueue(newInstance);
        }

        UpdateNames();
        return newInstance;
    }

    public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string DictionaryEntry) where T : Component
    {
        if (OBJECTPOOLS_ROOT == null) //checks if field was populated in editor
        {
            OBJECTPOOLS_ROOT = GameObject.Find(Utility.OBJECTPOOLS_PARENT_NAME);
            //checks if .Find() found a pool
            if (OBJECTPOOLS_ROOT == null) OBJECTPOOLS_ROOT = new GameObject(Utility.OBJECTPOOLS_PARENT_NAME);
        }

        // SETUP OBJECTPOOL PARENT OBJECT
        GameObject poolParent = new GameObject(DictionaryEntry);
        poolParent.transform.SetParent(OBJECTPOOLS_ROOT.transform); //obj is just an identifier, in order to physically move it need to access .transform
        poolParentObjects[DictionaryEntry] = poolParent; //poolParentObjects is list of all pools

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

        foreach (var key in poolDictionary.Keys) //var is a generic data type, its string in our case, but it can be smth else if we want
        {
            int inactiveCount = poolDictionary[key].Count;
            int totalInPool = poolMaxSizeDictionary[key]; //specified by max pool size
            int activeInPool = totalInPool - inactiveCount; //num objs in pool minus numobjects in queue

            totalActive += activeInPool;
            totalObjects += totalInPool;

            if (poolParentObjects.TryGetValue(key, out GameObject parent))
            {
                parent.name = $"{key} ({activeInPool}/{totalInPool})"; //safeguard in case object pool root is null somehow
            }
        }

        if (OBJECTPOOLS_ROOT != null)
        {
            //see how many total objs in all pools from inspector easily at runtime
            //sum up all object pool amounts; creates "ObjectPools (0/100)"
            OBJECTPOOLS_ROOT.name = $"{Utility.OBJECTPOOLS_PARENT_NAME} ({totalActive}/{totalObjects})";
        }
    }

    // public static void DestroyPool(string key)
    // {
    //     if (!poolDictionary.ContainsKey(key))
    //     {
    //         Debug.LogWarning($"Attempted to destroy non-existent pool: {key}");
    //         return;
    //     }

    //     // Destroy all pooled objects
    //     while (poolDictionary[key].Count > 0)
    //     {
    //         var obj = poolDictionary[key].Dequeue();
    //         if (obj != null)
    //         {
    //             Object.Destroy(obj.gameObject);
    //         }
    //     }

    //     // Destroy the parent object
    //     if (poolParentObjects.TryGetValue(key, out GameObject parent))
    //     {
    //         Object.Destroy(parent);
    //         poolParentObjects.Remove(key);
    //     }

    //     // Clean up dictionaries
    //     poolDictionary.Remove(key);
    //     poolLookup.Remove(key);
    //     poolMaxSizeDictionary.Remove(key);

    //     UpdateNames();
    // }

}
