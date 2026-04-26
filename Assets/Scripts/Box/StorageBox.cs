using UnityEngine;

public class StorageBox : MonoBehaviour, IInteractable
{
    [Header("Storage Box Settings")]
    [SerializeField] private bool isSecondaryStorage = false;

    public void Interact(PlayerCarry player)
    {
        if(!player.HasItem) return;

        DeliverBoxFromPlayer(player);

    }

    public void GrappleInteract(PlayerCarry player)
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Box box = other.GetComponent<Box>();
        if(box == null) return;

        if (IsBeingCarried(box)) return;

        RegisterDelivery();
        Destroy(other.gameObject);
    }

    private void DeliverBoxFromPlayer(PlayerCarry player)
    {
        Box box = player.GetCarried();

        box.Drop(player);
        Destroy(box.gameObject);

        RegisterDelivery();
    }

    public void DeliverFromConveyor(Box box)
    {
        if (box == null) return;

        RegisterDelivery();
        Destroy(box.gameObject);
    }

    private void RegisterDelivery()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.AddScore();
        }
        else
        {
            Debug.LogWarning("GameManager instance not found. Score will not be updated.");
        }
    }

    private bool IsBeingCarried(Box box)
    {
        PlayerCarry[] players = FindObjectsByType<PlayerCarry>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.GetCarried() == box)
            {
                return true;
            }
        }
        return false;
    }
}
