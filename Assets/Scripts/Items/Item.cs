using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IInteractable
{
    //[SerializeField] private float followSpeed = 18f;
    [SerializeField] private Vector2 carryOffset = new Vector2(0f, 0.3f);
    [SerializeField] ItemType type;

    [SerializeField] private Transform sprayPoint;

    private Transform Carrier;

    private bool isCarried = false;

    private Rigidbody2D rb;
    private Collider2D col;
    private Collider2D playerCollider;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isCarried && Carrier != null)
        {
            transform.position = (Vector2)Carrier.position
                + (Vector2)(Carrier.right * carryOffset.x + Carrier.up * carryOffset.y);

            RotateToMouse();
        }
    }

    public void Interact(PlayerCarry player)
    {
        if (player.TryPickup(this))
        {
            StartCarrying(player.transform);
        }
    }

    public void StartCarrying(Transform who)
    {
        Carrier = who;
        isCarried = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;
    }

    public void Drop(PlayerCarry player)
    {
        isCarried = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = true;

        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(col, playerCollider, false);
            playerCollider = null;
        }

        Carrier = null;
        player.Drop();
    }

    private void RotateToMouse()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouse - transform.position;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0,0,angle);
    }

    public Transform GetSprayPoint()
    {
        return sprayPoint;
    }

    public ItemType itemType()
    {
        return type;
    }
}
