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
        Debug.Log("Handle Interaction");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactionLayer);

        foreach(var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
            {
                interactable = hit.GetComponentInParent<IInteractable>();
            }
            if (interactable == null) continue;


            if (hit.CompareTag("Machine"))
            {
                interactable.Interact(playerCarry);
                Debug.Log("Item Found : " + hit.name);
            }

            if (hit.CompareTag("ToolStorage"))
            {
                interactable.Interact(playerCarry);
            }

            if (hit.CompareTag("BoxStorage"))
            {
               interactable.Interact(playerCarry);
            }

            if (hit.CompareTag("Item"))
            {
                Debug.Log("Item Found : " + hit.name);
                interactable.Interact(playerCarry);
                return;
            }
        }

        if (playerCarry.HasItem)
        {
            Item carried = playerCarry.GetCarried();
            if (carried != null)
            {
                Debug.Log("Interacting with carried item");
                carried.Interact(playerCarry);
                return;
            }
        }
    }

    private void HandleDrop()
    {
        Debug.Log("Handle Drop");
        if (playerCarry != null && playerCarry.HasItem)
        {
            playerCarry.GetCarried().Drop(playerCarry);
        }
    }
}
