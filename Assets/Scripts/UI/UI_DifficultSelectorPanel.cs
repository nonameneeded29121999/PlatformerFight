using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_DifficultSelectorPanel : MonoBehaviour
{
    public UnityAction OnChooseDifficulty;

    public UnityAction OnCloseDifficultyPanel;

    [SerializeField]
    private InputReader _inputReader = default;

    [SerializeField]
    private GameStateSO gameStateSO = default;

    private void OnEnable()
    {
        _inputReader.UICancelEvent += CancelDifficultyPanel;
    }

    private void OnDisable()
    {
        _inputReader.UICancelEvent -= CancelDifficultyPanel;
    }

    public void DifficultySelected(GameDifficulty gameDifficulty)
    {
        gameStateSO.SetDifficulty(gameDifficulty);
        OnChooseDifficulty.Invoke();
    }

    public void EasyBtn()
    {
        gameStateSO.SetDifficulty(GameDifficulty.EASY);
        OnChooseDifficulty.Invoke();
    }

    public void NormalBtn()
    {
        gameStateSO.SetDifficulty(GameDifficulty.NORMAL);
        OnChooseDifficulty.Invoke();
    }

    public void HardBtn()
    {
        gameStateSO.SetDifficulty(GameDifficulty.HARD);
        OnChooseDifficulty.Invoke();
    }

    public void LunaticBtn()
    {
        gameStateSO.SetDifficulty(GameDifficulty.LUNATIC);
        OnChooseDifficulty.Invoke();
    }

    private void CancelDifficultyPanel()
    {
        OnCloseDifficultyPanel.Invoke();
    }
}
