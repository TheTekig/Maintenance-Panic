using UnityEngine;

public class BoxStorage : MonoBehaviour, IInteractable
{
    public void Interact(PlayerCarry player)
    {
        Debug.Log("Interaction With Box Storage");
        DeliverBoxFromPlayer(player);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();

        if(item == null) return;
        
        if (IsBeingCarried(item)) return;

        if(item.itemType() != ItemType.Box) return;

        RegisterDelivery();

        Destroy(item.gameObject);


    }

    private void RegisterDelivery()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore();
        }
        else
        {
            Debug.LogWarning("GameManager instance not found. Score will not be updated.");
        }
    }

    public void DeliverFromConveyor(Item item)
    {
        if (item == null) return;

        if (item.itemType() == ItemType.Box)
        {
            Destroy(item.gameObject);

            RegisterDelivery();
        }
        else
        {
            Debug.Log("It's Not a Box");
        }

    }

    private void DeliverBoxFromPlayer(PlayerCarry player)
    {
        Item item = player.GetCarried();

        if (item == null) return;

        if (item.itemType() == ItemType.Box)
        {
            player.Drop();

            Destroy(item.gameObject);

            RegisterDelivery();
        }
        else
        {
            Debug.Log("It's Not a Box");
        }
    }

    private bool IsBeingCarried(Item item)
    {
        PlayerCarry[] players = FindObjectsByType<PlayerCarry>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.GetCarried() == item)
            {
                return true;
            }
        }
        return false;
    }
}
