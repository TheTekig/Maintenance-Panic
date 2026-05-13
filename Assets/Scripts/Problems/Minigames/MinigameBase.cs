using UnityEngine;
using System;
using Unity.VisualScripting;

public abstract class MinigameBase : MonoBehaviour
{
    protected Action<bool> onComplete;

    public virtual void Begin(Action<bool> callBack)
    {
        onComplete = callBack;
        gameObject.SetActive(true);
        OnBegin();
    }

    protected abstract void OnBegin();

    protected void Complete(bool success)
    {
        gameObject.SetActive(false);
        onComplete?.Invoke(success);
    }
}
