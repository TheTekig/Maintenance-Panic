using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    private Box carriedBox;
    public bool HasItem => carriedBox != null;

    public bool TryPickup(Box box)
    {
        if (HasItem) return false;

        carriedBox = box;
        return true;
    }

    public void Drop()
    {
        carriedBox = null;
    }

    public Box GetCarried()
    {
        return carriedBox;
    }
}
