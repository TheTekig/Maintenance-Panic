using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private LayerMask interactLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactLayer);

            if (hit != null)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();

                if (interactable != null )
                {
                    interactable.Interact();
                }
            }
        }
    }
}
