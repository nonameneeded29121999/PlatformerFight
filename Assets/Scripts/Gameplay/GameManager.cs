using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameStateSO _gameState = default;

    private GameDifficulty currentDifficulty = default;

    [SerializeField]
    private long[] extraLifeThreshold;

    [SerializeField]
    private int currentThreshold = 0;

    [SerializeField]
    private bool maxThresholdReached = false;

    [Header("Event channels")]

    #region Listener

    [SerializeField]
    private GameDifficultyEventChannelSO OnRequestDifficulty = default;

    [SerializeField]
    private VoidEventChannelSO checkScoreThreshold = default;

    [SerializeField]
    private VoidEventChannelSO onPlayerDead = default;

    #endregion

    #region Responder

    [SerializeField]
    private IntEventChannelSO onAddLives = default;

    [SerializeField]
    private VoidEventChannelSO restartStage = default;

    [SerializeField]
    private VoidEventChannelSO gameOver = default;

    #endregion

    void Awake()
    {
        currentDifficulty = _gameState.CurrentDifficulty;

        if (extraLifeThreshold.Length <= 0)
            return;

        Array.Sort(extraLifeThreshold);

        currentThreshold = 0;

        for (int i = extraLifeThreshold.Length - 1; i >= 0; i--)
        {
            Debug.Log(i);
            if (_gameState.Score > extraLifeThreshold[i])
            {
                currentThreshold = i + 1;
                break;
            }
        }

        if (currentThreshold >= extraLifeThreshold.Length)
            maxThresholdReached = true;
    }

    private void OnEnable()
    {
        OnRequestDifficulty.OnRequestDifficultyAction += ReturnDifficulty;
        checkScoreThreshold.OnEventRaised += CheckScoreLives;
        onPlayerDead.OnEventRaised += OnPlayerDead;
    }

    private void OnDisable()
    {
        OnRequestDifficulty.OnRequestDifficultyAction -= ReturnDifficulty;
        checkScoreThreshold.OnEventRaised -= CheckScoreLives;
        onPlayerDead.OnEventRaised -= OnPlayerDead;
    }

    private void CheckScoreLives()
    {        
        if (extraLifeThreshold.Length <= 0)
            return;

        if (maxThresholdReached)
            return;

        for (int i = currentThreshold; i < extraLifeThreshold.Length; i++)
        {
            if (_gameState.Score >= extraLifeThreshold[i])
            {
                currentThreshold++;
                onAddLives.RaiseEvent(1);
            }
        }

        if (currentThreshold >= extraLifeThreshold.Length)
            maxThresholdReached = true;
    }

    private void OnPlayerDead()
    {
        if (_gameState.LifeCount > 0)
        {
            Debug.Log("Still live");
            onAddLives.RaiseEvent(-1);
            restartStage.RaiseEvent();
        }
        else
        {
            //StartCoroutine(ShowGameOver());
            gameOver.RaiseEvent();
        }    
    }

    private IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(3f);

        gameOver.RaiseEvent();
    }    

    private GameDifficulty ReturnDifficulty()
    {
        return currentDifficulty;
    }
}
