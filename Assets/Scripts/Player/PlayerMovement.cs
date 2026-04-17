using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody2D rb;
    private Vector2 movement;

    private PlayerGrappler pg;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pg = GetComponent<PlayerGrappler>();
    }


    void Update()
    {
        if (!pg.ISgrapling)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            movement = new Vector2(moveX, moveY).normalized;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FixedUpdate()
    {
        if (!pg.ISgrapling) rb.linearVelocity = movement * speed;
    }
}
