using TMPro;
using UnityEngine;
/*
How it works:
1. Player places a tower → TowerPlacer.PlaceTower() calls:
GameManager.instance.TrySpendLives(cost);
2. TrySpendLives in GameManager deducts the lives, then calls:
currentLives -= amount;
EventsManager.instance.gameEvents.LivesUpdated(currentLives);
3. LivesUpdated in GameEvents fires the event:
public void LivesUpdated(int lives) { OnLivesUpdated?.Invoke(lives); }
This broadcasts to every subscriber, passing that int along.
4. LivesDisplay is subscribed because of this line in OnEnable:
EventsManager.instance.gameEvents.OnLivesUpdated += UpdateLiveText;
So when the event fires, UpdateLiveText gets called automatically with the live value.
5. UpdateLiveText updates the UI:
liveText.text = $"Lives: {lives}";
*/
public class LivesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;

    private void Awake()
    {
        if (livesText == null) livesText = GetComponent<TMP_Text>(); //was the field populated in the inspector
        if (livesText == null) livesText = GetComponentInChildren<TMP_Text>(); //was there a TMP_Text component on a child object
        if (livesText == null) //did getComponent actually find one
        {
            Debug.LogError($"[LivesDisplay] No TMP_Text component found on {gameObject.name} or its children.");
        }
    }

    private void OnEnable()
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnLivesUpdated += UpdateLivesText;
    }

    private void OnDisable()
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnLivesUpdated -= UpdateLivesText;
    }

    private void OnDestroy() //just in case the object is destroyed without being disabled first, we want to make sure we unsubscribe from events to avoid memory leaks and null reference exceptions
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnLivesUpdated -= UpdateLivesText;
    }

    private void Start()
    {
        if (livesText != null && GameManager.instance != null)
        {
            UpdateLivesText(GameManager.instance.GetCurrentLives());
        }
    }

    private void UpdateLivesText(int lives)
    {
        if (livesText == null) return;
        livesText.text = $"Lives: {lives}"; //updates UI
    }
}