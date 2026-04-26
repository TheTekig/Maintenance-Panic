using UnityEngine;

public class Tool : MonoBehaviour, IInteractable
{
    [SerializeField] public Tooltype tooltype;

    private bool isBeingPulled = false;
    private bool isCarried = false;

    private float followSpeed = 12f;

    private Transform carrier;
    private Rigidbody2D rb;
    private Collider2D col;

    private Collider2D playerCollider;
    private LineRenderer grapplerLine;

    [SerializeField] private Vector2 carryOffset = new Vector2(0f, 0.8f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }


    void Update()
    {
        // If the tool is being pulled by the grappling hook, move it towards the carrier
        if (isBeingPulled && carrier != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, carrier.position, followSpeed * Time.deltaTime);

            if (grapplerLine != null && grapplerLine.enabled)
            {
                grapplerLine.SetPosition(0, carrier.position);
                grapplerLine.SetPosition(1, transform.position);
            }


            // If the box is close enough to the carrier, switch to being carried
            if (Vector2.Distance(transform.position, carrier.position) < 0.3f)
            {
                Playertoolcarry player = carrier.GetComponent<Playertoolcarry>();

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

        if (isCarried && carrier != null)
        {
            transform.position = (Vector2)carrier.position + (Vector2)(carrier.right * carryOffset.x + carrier.up * carryOffset.y);
        }
    }

    public void Interact(PlayerCarry player)
    {
        Playertoolcarry toolCarry = player.GetComponent<Playertoolcarry>();

        if (toolCarry == null) return;

        if (toolCarry.HasTool)
        {
            toolCarry.DropTool();
        }

        if (toolCarry.TryPickup(this))
        {
            StartCarrying(player.transform);
        }

    }

    public void GrappleInteract(PlayerCarry player)
    {
        Playertoolcarry toolCarry = player.GetComponent<Playertoolcarry>();

        if (toolCarry == null || toolCarry.HasTool) return;

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

    public void Drop(Vector2 position)
    {
        isCarried = false;
        isBeingPulled = false;
        carrier = null;

        transform.position = position;
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = true;

        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(col, playerCollider, false);
            playerCollider = null;
        }
        HideLine();
    }

    public void StartCarrying(Transform who)
    {
        carrier = who;

        isCarried = true;
        isBeingPulled = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        col.enabled = false;

        HideLine();
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
