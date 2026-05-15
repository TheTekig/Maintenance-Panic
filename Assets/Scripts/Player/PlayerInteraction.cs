using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    private LayerMask interactionLayer;
    private PlayerCarry playerCarry;
    
    void Awake()
    {
        interactionLayer = LayerMask.GetMask("Interact");
        playerCarry = GetComponent<PlayerCarry>();
    }

    void Update()
    {
        if (PlayerState.IsBusy) return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            HandleInteract();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            HandleDrop();
        }
    }

    private void HandleInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactionLayer);

        string[] priority = { "Item", "Machine", "ToolStorage", "BoxStorage" };

        foreach (string tag in priority)
        {
            foreach (var hit in hits)
            {
                if (!hit.CompareTag(tag)) continue;

                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable == null)
                {
                    interactable = hit.GetComponentInParent<IInteractable>();
                }
                if (interactable == null) continue;

                interactable.Interact(playerCarry);
                return;
            }
        }
    }

    private void HandleDrop()
    {
        if (playerCarry != null && playerCarry.HasItem)
        {
            playerCarry.GetCarried().Drop(playerCarry);
        }
    }
}
