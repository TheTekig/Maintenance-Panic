using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    private LayerMask interactLayer;

    private void Awake()
    {
        interactLayer = LayerMask.GetMask("Interactable");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactLayer);

            foreach (var hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();

                if (interactable == null)
                {
                    interactable = hit.GetComponentInParent<IInteractable>(); // Check parent if not found on the object itself
                }

                if (interactable != null)
                {
                    interactable.Interact(GetComponent<PlayerCarry>());
                    break; // Interact with the first valid object only
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerCarry playerCarry = GetComponent<PlayerCarry>();
            if (playerCarry.HasItem)
            {
                Box box = playerCarry.GetCarried();
                box.Drop(playerCarry);
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
