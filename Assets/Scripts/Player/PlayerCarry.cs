using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    private Box carriedBox;
    public bool HasItem => carriedBox != null;

    public void setCarried(Box box)
    {
        carriedBox = box;
    }

    public void ClearCarried()
    {
               carriedBox = null;
    }
}
