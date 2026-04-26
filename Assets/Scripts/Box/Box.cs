using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour, IInteractable
{
    private bool isBeingPulled = false;
    private bool isCarried = false;

    private Transform carrier;
    private Rigidbody2D rb;
    private Collider2D col;

    [SerializeField] private float followSpeed = 12f;
    [SerializeField] private Vector2 carryOffset = new Vector2(0.6f, 0);

    private Collider2D playerCollider;

    private LineRenderer grapplerLine;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
 
    void Update()
    {
        // If the box is being pulled by the grappling hook, move it towards the carrier
        if (isBeingPulled && carrier != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, carrier.position, followSpeed * Time.deltaTime);

            if (grapplerLine != null && grapplerLine.enabled )
            {
                grapplerLine.SetPosition(0, carrier.position);
                grapplerLine.SetPosition(1, transform.position);
            }


            // If the box is close enough to the carrier, switch to being carried
            if (Vector2.Distance(transform.position, carrier.position) < 0.3f)
            {
                PlayerCarry player = carrier.GetComponent<PlayerCarry>();

                if (player != null && player.TryPickup(this))
                {
                    StartCarrying(carrier);
                }
                else
                {
                    CancelPull();
                }
            }
        }

        // If the box is being carried, keep it at the offset position from the carrier
        if (isCarried && carrier != null)
        {
            transform.position = (Vector2)carrier.position + (Vector2)(carrier.right * carryOffset.x + carrier.up * carryOffset.y);
        }
    }

    public void Interact(PlayerCarry player)
    {
        if (isCarried)
        {
            Drop(player);
        }
        else
        {
            if (player.TryPickup(this))
            {
                StartCarrying(player.transform);
            }
        }
    }

    public void GrappleInteract(PlayerCarry player)
    {
        Debug.Log("Grapple Interact with Box");
        if (player.HasItem) return;
        
        carrier = player.transform;
        playerCollider = player.GetComponent<Collider2D>();

        isBeingPulled = true;
        isCarried = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        col.enabled = false;

        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(col, playerCollider, true);
        }

        PlayerGrappler grappler = player.GetComponent<PlayerGrappler>();
        if (grappler != null)
        {
            grapplerLine = grappler.Line;
            if (grapplerLine != null)
            {
                grapplerLine.SetPosition(0, carrier.position);
                grapplerLine.SetPosition(1, transform.position);
                grapplerLine.enabled = true;
            }
        }

    }

    public void StartCarrying(Transform who)
    {
        carrier = who;

        isCarried = true;
        isBeingPulled = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        col.enabled = false;
    }

    public void Drop(PlayerCarry player)
    {
        isCarried = false;
        isBeingPulled = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = true;

        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(col, playerCollider, false);
            playerCollider = null;
        }

        HideLine();
        carrier = null;
        player.Drop();
    }

    private void CancelPull()
    {
        isBeingPulled = false;
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = true;

        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(col, playerCollider, false);
            playerCollider = null;
        }
        HideLine();
        carrier = null;
    }

    private void HideLine()
    {
        if (grapplerLine != null)
        {
            grapplerLine.enabled = false;
            grapplerLine = null;
        }
    }
}
