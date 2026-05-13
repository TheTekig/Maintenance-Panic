using UnityEngine;
using System;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }
    public bool IsMinigameActive { get; private set; }

    private Action<bool> onComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartMinigame(MinigameBase minigame, Action<bool> callBack)
    {
        if (IsMinigameActive) { Debug.LogWarning("Um minigame ja esta ativo"); return; }
        IsMinigameActive = true;
        PlayerState.SetBusy(true);
        onComplete = callBack;
        minigame.Begin(OnMinigameComplete);

        Time.timeScale = 1f;
    }

    private void OnMinigameComplete(bool success)
    {
        IsMinigameActive = false;
        PlayerState.SetBusy(false);
        onComplete?.Invoke(success);
        onComplete = null;
    }
}
