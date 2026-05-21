using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerGrappler : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform grapplePoint;

    [SerializeField] private float wallOffset = 0.5f;

    [SerializeField] private LayerMask grappleMask;
 
    private PlayerCarry playerCarry;
    private bool isGrappling = false;
    private Item grabbedItem;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private bool blocked = false;

    public bool ISgrapling => isGrappling;

    private Vector2 target;

    private Vector2 hitPoint;
    private bool hasHitPoint;

    private Vector2 debugCastOrigin;
    private Vector2 debugCastDirection;
    private float debugCastDistance;


    private void Awake()
    {
        playerCarry = GetComponent<PlayerCarry>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {   if (!blocked)
        {
            if (Input.GetMouseButtonDown(0) && !PlayerState.IsBusy)
            {
                StartGrapple();
            }
        }
        UpdateLine();
    }

    private void FixedUpdate()
    {

        HandleGrappleMovement();

        HandleGrappleItemPull();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGrappling) return;

        if (collision.collider.CompareTag("Collisor") || collision.collider.CompareTag("Machine") || collision.collider.CompareTag("ToolStorage"))
        {
            rb.linearVelocity = Vector2.zero;
            CancelGrapple();
        }
           


    }

    private void StartGrapple()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mouse.x, mouse.y);

        Vector2 playerPos = transform.position;
        Vector2 rawDirection = mousePos - playerPos;

        float distance = rawDirection.magnitude;

        Vector2 direction = rawDirection.normalized;

        debugCastOrigin = playerPos;
        debugCastDirection = direction;
        debugCastDistance = distance;

        if (distance < minDistance)
        {
            return;
        }
       distance = Mathf.Min(distance, maxDistance);

        target = playerPos + direction * distance;


        RaycastHit2D hit = Physics2D.BoxCast(playerPos, boxCollider.size, 0f, direction, distance, grappleMask);
        Debug.DrawRay(playerPos, direction * distance, Color.red, 1f);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Item"))
            {

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

    private void HandleGrappleMovement()
    {
        if (isGrappling)
        {

            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);


            rb.MovePosition(newPos);

            if (Vector2.Distance(rb.position, target) < 0.1f)
            {
                Debug.Log("Cancelando Grapple ");
                CancelGrapple();
            }
           
        }
    }

    private void HandleGrappleItemPull()
    {
        if (grabbedItem != null)
        {
            grabbedItem.transform.position = Vector2.MoveTowards(
                grabbedItem.transform.position, transform.position, speed * Time.fixedDeltaTime);


            if (Vector2.Distance(grabbedItem.transform.position, transform.position) < 0.8f)
            {
                IInteractable interactable = grabbedItem.GetComponent<IInteractable>();
                grabbedItem.Interact(playerCarry);
                grabbedItem = null;
                line.enabled = false;
                return;
            }
        }
    }

    private void UpdateLine()
    {
        if(!line.enabled) return;

        if (grabbedItem != null)
        {
            line.SetPosition(0, grabbedItem.transform.position);
            line.SetPosition(1, transform.position);
        }
        else if (isGrappling)
        {
            line.SetPosition(0, grapplePoint.position);
            line.SetPosition(1, target);
        }
    }

    public void CancelGrapple()
    {
        isGrappling = false;
        grabbedItem = null;
        line.enabled = false;
    }

    public void SetBlocked(bool value)
    {
        blocked = value;
    }


}
