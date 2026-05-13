using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance {get; private set;}

    public static bool IsBusy {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return;}
        Instance = this;
    }

    public static void SetBusy(bool busy)
    {
        IsBusy = busy;
    }
}
