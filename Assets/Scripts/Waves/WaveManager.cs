using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// TODO: stub
public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private WaveData currentWave;
    [SerializeField] private int enemiesSentInCurrentWave = 0;
    [SerializeField] private float enemySendCountdownTimer = 0f;
    [SerializeField] private bool outOfWaves = false;
    [SerializeField] private float timeBetweenWavesTimer = 0f;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private bool betweenWaves = true;

    private void Start()
    {

        GameManager.instance.SetCurrentWaveIndex(0);
        enemiesSentInCurrentWave = 0;
        betweenWaves = true;
        timeBetweenWavesTimer = timeBetweenWaves;
        currentWave = waves[GameManager.instance.GetCurrentWaveIndex()];
        EventsManager.instance.gameEvents.WaveUpdated(GameManager.instance.GetCurrentWaveIndex());
    }

    private void Update()
    {
        if (!GameManager.instance.GetGameActive() || outOfWaves) return;
        if (betweenWaves) DelayBetweenWaves();
        else
        {
            enemySendCountdownTimer -= Time.deltaTime;
            if (enemySendCountdownTimer <= 0)
            {
                enemySendCountdownTimer = currentWave.sendInterval;
                EnemyFactory.Instance.CreateEnemy(currentWave.enemyType);
                enemiesSentInCurrentWave += 1;
            }
            // reseet and bump up wave num
            if (enemiesSentInCurrentWave >= currentWave.enemiesInWave)
            {
                GameManager.instance.SetCurrentWaveIndex(GameManager.instance.GetCurrentWaveIndex() + 1);
                if (GameManager.instance.GetCurrentWaveIndex() < waves.Count)
                {
                    betweenWaves = true;
                    currentWave = waves[GameManager.instance.GetCurrentWaveIndex()];
                    enemySendCountdownTimer = currentWave.sendInterval;
                    enemiesSentInCurrentWave = 0;
                }
                else
                {
                    outOfWaves = true;
                    GameManager.instance.SetCurrentWaveIndex(-1);
                }
            }
        }
    }

    private void DelayBetweenWaves()
    {
        timeBetweenWavesTimer -= Time.deltaTime;
        if (timeBetweenWavesTimer <= 0)
        {
            betweenWaves = false;
            timeBetweenWavesTimer = timeBetweenWaves;
            EventsManager.instance.gameEvents.WaveUpdated(GameManager.instance.GetCurrentWaveIndex() + 1);
        }
    }

}