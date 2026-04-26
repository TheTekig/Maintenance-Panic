using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerGrappler : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private LineRenderer line;
 
    public LineRenderer Line => line;

    private bool isGrappling = false;
    public bool ISgrapling => isGrappling;

    private bool isShowingInteractLine = false;
    private float interactLineTimer = 0f;
    private const float INTERACT_LINE_DURATION = 0.3f;

    private Vector2 target;
    private PlayerCarry playerCarry;

    private void Awake()
    {
        playerCarry = GetComponent<PlayerCarry>();
    }

    void Update()
    {
        if (isShowingInteractLine)
        {
            interactLineTimer -= Time.deltaTime;
            if (interactLineTimer <= 0f)
            {
                isShowingInteractLine = false;
                line.enabled = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {

            if (playerCarry.HasItem) return;

            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos = new Vector2(mouse.x, mouse.y);

            Vector2 playerPos = transform.position;
            Vector2 direction = mousePos - playerPos;

            float distance = direction.magnitude;

            if (distance < minDistance) return;

            distance = Mathf.Min(distance, maxDistance);
            direction = direction.normalized;

            LayerMask mask  = LayerMask.GetMask("Interactable");
            RaycastHit2D hit = Physics2D.Raycast(playerPos, direction, distance,mask);

            if (hit.collider != null)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable == null)
                {
                    interactable = hit.collider.GetComponentInParent<IInteractable>();
                }

                if (interactable != null)
                {
                    interactable.GrappleInteract(GetComponent<PlayerCarry>());

                    if (!(interactable is Box))
                    { 
                    line.enabled = true;
                    line.SetPosition(0, playerPos);
                    line.SetPosition(1, hit.point);

                    isShowingInteractLine = true;
                    interactLineTimer = INTERACT_LINE_DURATION;
                    }   
                    return;
                }
            }

            // If no interactable object is hit, grapple to the maximum distance in the direction of the mouse
            target = playerPos + direction * distance;

            isGrappling = true;
            isShowingInteractLine = false;
            line.enabled = true;
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
            }
       }
         
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}
