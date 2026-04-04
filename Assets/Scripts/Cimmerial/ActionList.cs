using System;
using System.Collections.Generic;

[System.Serializable]
public class ActionList : IBaseReboost
{
    private Dictionary<string, Delegate> actions = new Dictionary<string, Delegate>();
    private List<string> actionsToRemove = new List<string>();
    private bool isExecuting;

    public void Add(string name, Action action)
    {
        if (!actions.ContainsKey(name))
        {
            actions[name] = action;
        }
    }

    public void Add(string name, Action<object> contextAction)
    {
        if (!actions.ContainsKey(name))
        {
            actions[name] = contextAction;
        }
    }

    public void Remove(string name)
    {
        if (isExecuting)
        {
            actionsToRemove.Add(name);
        }
        else if (actions.ContainsKey(name))
        {
            actions.Remove(name);
        }
    }

    public bool HasAction(string name)
    {
        return actions.ContainsKey(name);
    }

    public void ExecuteAll(object context = null)
    {
        isExecuting = true;

        foreach (var pair in new Dictionary<string, Delegate>(actions))
        {
            if (!actionsToRemove.Contains(pair.Key))
            {
                if (pair.Value is Action<object> contextAction && context != null)
                {
                    contextAction(context);
                }
                else if (pair.Value is Action regularAction)
                {
                    regularAction();
                }
            }
        }

        isExecuting = false;

        foreach (var name in actionsToRemove)
        {
            actions.Remove(name);
        }
        actionsToRemove.Clear();
    }

    public void ResetReboostiplier() => Clear();

    public void Clear()
    {
        if (isExecuting) actionsToRemove.AddRange(actions.Keys);
        else actions.Clear();
    }
}
