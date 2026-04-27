using TMPro;
using UnityEngine;
/*
How it works:
1. Player places a tower → TowerPlacer.PlaceTower() calls:
GameManager.instance.TrySpendCoins(cost);
2. TrySpendCoins in GameManager deducts the coins, then calls:
currentCoins -= amount;
EventsManager.instance.gameEvents.CoinsUpdated(currentCoins);
3. CoinsUpdated in GameEvents fires the event:
public void CoinsUpdated(int coins) { OnCoinsUpdated?.Invoke(coins); }
This broadcasts to every subscriber, passing that int along.
4. CoinDisplay is subscribed because of this line in OnEnable:
EventsManager.instance.gameEvents.OnCoinsUpdated += UpdateCoinText;
So when the event fires, UpdateCoinText gets called automatically with the coin value.
5. UpdateCoinText updates the UI:
coinText.text = $"Coins: {coins}";
*/
public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;

    private void Awake()
    {
        if (coinText == null) coinText = GetComponent<TMP_Text>(); //was the field populated in the inspector
        if (coinText == null) coinText = GetComponentInChildren<TMP_Text>(); //was there a TMP_Text component on a child object
        if (coinText == null) //did getComponent actually find one
        {
            Debug.LogError($"[CoinDisplay] No TMP_Text component found on {gameObject.name} or its children.");
        }
    }

    private void OnEnable()
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnCoinsUpdated += UpdateCoinText;
    }

    private void OnDisable()
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnCoinsUpdated -= UpdateCoinText;
    }

    private void OnDestroy() //just in case the object is destroyed without being disabled first, we want to make sure we unsubscribe from events to avoid memory leaks and null reference exceptions
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnCoinsUpdated -= UpdateCoinText;
    }

    private void Start()
    {
        if (coinText != null && GameManager.instance != null)
        {
            UpdateCoinText(GameManager.instance.GetCurrentCoins());
        }
    }

    private void UpdateCoinText(int coins)
    {
        if (coinText == null) return;
        coinText.text = $"Coins: {coins}"; //updates UI
    }
}