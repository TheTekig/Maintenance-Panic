using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    private Item carriedItem;

    public bool HasItem => carriedItem != null;

    public bool TryPickup(Item item)
    {
        if (HasItem) return false;
        carriedItem = item;
        return true;
    }

    public void Drop()
    {
        carriedItem = null;
    }

    public Item GetCarried()
    {
        return carriedItem;
    }
}
