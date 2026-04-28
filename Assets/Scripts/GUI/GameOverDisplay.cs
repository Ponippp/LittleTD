using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private string gameOverText = "GAME OVER";

    private void Awake()
    {
        if (tmp == null) tmp = GetComponent<TMP_Text>(); //was the field populated in the inspector
        if (tmp == null) tmp = GetComponentInChildren<TMP_Text>(); //was there a TMP_Text component on a child object
        if (tmp == null) //did getComponent actually find one
        {
            Debug.LogError($"[CoinDisplay] No TMP_Text component found on {gameObject.name} or its children.");
        }
    }

    private void OnEnable()
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnToggleGameOverText += ToggleText;
    }

    private void OnDisable()
    {
        if (EventsManager.instance != null)
            EventsManager.instance.gameEvents.OnToggleGameOverText -= ToggleText;
    }

    private void OnDestroy() //just in case the object is destroyed without being disabled first, we want to make sure we unsubscribe from events to avoid memory leaks and null reference exceptions
    {
        if (EventsManager.instance != null) EventsManager.instance.gameEvents.OnToggleGameOverText -= ToggleText;
    }

    private void Start()
    {
        tmp.text = "";
    }

    private void ToggleText()
    {
        if (gameOverText == null) return;
        if (tmp.text == gameOverText) tmp.text = "";
        else tmp.text = gameOverText;
    }
}