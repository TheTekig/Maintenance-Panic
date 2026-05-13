using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class PlayerGrappler : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private LineRenderer line;

    [SerializeField] private LayerMask grappleMask;
 
    private PlayerCarry playerCarry;
    private bool isGrappling = false;
    private Item grabbedItem;
    public bool ISgrapling => isGrappling;

    private Vector2 target;

    private void Awake()
    {
        playerCarry = GetComponent<PlayerCarry>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayerState.SetBusy(true);

            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos = new Vector2(mouse.x, mouse.y);

            Vector2 playerPos = transform.position;
            Vector2 direction = mousePos - playerPos;
            if (direction.magnitude < minDistance) return;

            if (direction.magnitude > maxDistance)
            {
                direction = direction.normalized * maxDistance;
            }

            target = playerPos + direction;

            RaycastHit2D hit = Physics2D.Raycast(playerPos, direction.normalized, direction.magnitude, grappleMask);
            if(hit.collider != null)
            {
                if (hit.collider.CompareTag("Collisor") || hit.collider.CompareTag("Machine") || hit.collider.CompareTag("ToolStorage"))
                {
                    target = hit.point + hit.normal * 0.5f;
                }
                
                if (hit.collider.CompareTag("Item"))
                {
                    Debug.Log("Colidiu com uma tag 'Item'!");

                    Item item = hit.collider.GetComponent<Item>();
                    
                    if (item != null)
                    {
                       grabbedItem = item;
                       line.enabled = true;

                       return;
                    }
                }
            }

            Debug.DrawRay(playerPos, direction.normalized * direction.magnitude, Color.yellow, 2f);

            isGrappling = true;
            line.enabled = true;
        }
        if (grabbedItem != null)
        {
            grabbedItem.transform.position = Vector2.MoveTowards(
                grabbedItem.transform.position, transform.position, speed * Time.deltaTime);

            line.SetPosition(0, grabbedItem.transform.position);
            line.SetPosition(1, transform.position);

            if(Vector2.Distance(grabbedItem.transform.position, transform.position) < 0.8f)
            {
                IInteractable interactable = grabbedItem.GetComponent<IInteractable>();
                grabbedItem.Interact(playerCarry);
                grabbedItem = null;
                line.enabled = false;
                PlayerState.SetBusy(false);
                return;
            }
        }

        if (isGrappling)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, target);

            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                isGrappling = false;
                line.enabled = false;
                PlayerState.SetBusy(false);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
