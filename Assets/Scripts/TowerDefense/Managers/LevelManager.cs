using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private int lives = 10;

    public int TotalLives { get; set; }
    public int CurrentWave { get; set; }
    public HeartController heartController;
    
    private void Start()
    {
        TotalLives = lives;
        CurrentWave = 1;
    }

    private void ReduceLives(Enemy enemy)
    {
        TotalLives--;
        StartCoroutine(PlayHeartReduce());
        if (TotalLives <= 0)
        {
            AudioManager.Instance.musicSource.Stop();
            TotalLives = 0;
            AudioManager.Instance.PlaySound("GameOver");
            GameOver();
        }
    }

    private IEnumerator PlayHeartReduce()
    {
        heartController.TriggerHeartShaker();
        AudioManager.Instance.PlaySound("LoseHeart");
        yield return new WaitForSeconds(0.1f);
    }

    private void GameOver()
    {
        UIManager.Instance.ShowGameOverPanel();
        UIManager.Instance.PauseTime();
    }

    private void YouWin()
    {
        AudioManager.Instance.musicSource.Stop();
        AudioManager.Instance.PlaySound("Win");
        StartCoroutine(ExecuteAfterTime(4));
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        UIManager.Instance.ShowWinPanel();
        UIManager.Instance.PauseTime();
    }

    private void WaveCompleted()
    {
        CurrentWave++;
        if (CurrentWave > Spawner.totalWave)
            YouWin();
    }
    
    private void OnEnable()
    {
        Enemy.OnEndReached += ReduceLives;
        Spawner.OnWaveCompleted += WaveCompleted;
    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= ReduceLives;
        Spawner.OnWaveCompleted -= WaveCompleted;
    }
}
