using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour, IInteractable
{
    private bool isBeingPulled = false;
    private bool isCarried = false;

    private Transform carrier;
    private Rigidbody2D rb;
    private PlayerCarry player;

    [SerializeField] private float followSpeed = 12f;
    [SerializeField] private Vector2 carryOffset = new Vector2(0.6f, 0);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<PlayerCarry>();
    }
 
    void Update()
    {
        // If the box is being pulled by the grappling hook, move it towards the carrier
        if (isBeingPulled && carrier != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, carrier.position, followSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, carrier.position) < 0.3f)
            {
                isBeingPulled = false;
                StartCarrying(carrier);
            }
        }

        // If the box is being carried, keep it at the offset position from the carrier
        if (isCarried && carrier != null)
        {
            transform.position = (Vector2)carrier.position + carryOffset;
        }
    }

    public void Interact()
    {
        if (isCarried)
        {
            Drop();
        }
        else
        {
            if (player != null && !player.HasItem)
            {
                StartCarrying(player.transform);
                player.setCarried(this);
            }
        }
    }

    public void GrappleInteract()
    {
        if (player != null && !player.HasItem)
        {
            carrier = player.transform;
            isBeingPulled = true;

            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void StartCarrying(Transform who)
    {
        carrier = who;
        isCarried = true;
        isBeingPulled = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Drop()
    {
        isCarried = false;
        isBeingPulled = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        carrier = null;
        
        if (player != null)
        {
            player.ClearCarried();
        }
    }
}
