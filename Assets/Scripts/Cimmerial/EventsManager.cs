using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance { get; private set; }
    public GameEvents gameEvents;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        gameEvents = new GameEvents();
    }
    
}
