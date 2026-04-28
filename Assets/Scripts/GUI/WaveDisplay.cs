using TMPro;
using UnityEngine;
/*
How it works:
1. Player places a tower → TowerPlacer.PlaceTower() calls:
GameManager.instance.TrySpendWaves(cost);
2. TrySpendWaves in GameManager deducts the waves, then calls:
currentWaves -= amount;
EventsManager.instance.gameEvents.WavesUpdated(currentWaves);
3. WavesUpdated in GameEvents fires the event:
public void WavesUpdated(int waves) { OnWavesUpdated?.Invoke(waves); }
This broadcasts to every subscriber, passing that int along.
4. WaveDisplay is subscribed because of this line in OnEnable:
EventsManager.instance.gameEvents.OnWavesUpdated += UpdateWaveText;
So when the event fires, UpdateWaveText gets called automatically with the wave value.
5. UpdateWaveText updates the UI:
waveText.text = $"Waves: {waves}";
*/
public class WaveDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;

    private void Awake()
    {
        if (waveText == null) waveText = GetComponent<TMP_Text>(); //was the field populated in the inspector
        if (waveText == null) waveText = GetComponentInChildren<TMP_Text>(); //was there a TMP_Text component on a child object
        if (waveText == null) //did getComponent actually find one
        {
            Debug.LogError($"[WaveDisplay] No TMP_Text component found on {gameObject.name} or its children.");
        }
    }

    private void OnEnable()
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnWaveUpdated += UpdateWaveText;
    }

    private void OnDisable()
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnWaveUpdated -= UpdateWaveText;
    }

    private void OnDestroy() //just in case the object is destroyed without being disabled first, we want to make sure we unsubscribe from events to avoid memory leaks and null reference exceptions
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnWaveUpdated -= UpdateWaveText;
    }

    private void Start()
    {
        if (waveText != null && GameManager.instance != null)
        {
            UpdateWaveText(GameManager.instance.GetCurrentWaveIndex() + 1);
        }
    }

    private void UpdateWaveText(int wave)
    {
        if (waveText == null || wave == 0) return;
        waveText.text = $"Waves: {wave}"; //updates UI
    }
}