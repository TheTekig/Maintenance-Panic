using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody2D rb;
    private Vector2 movement;

    private PlayerGrappler pg;

    private float slowMultiplier = 1f;

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pg = GetComponent<PlayerGrappler>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (PlayerState.IsBusy)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        else if (pg.ISgrapling)
        {
            return;
        }

            float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;
        
        if (movement.x > 0)
        {
            animator.SetBool("WalkLeft", false);
            animator.SetBool("WalkRight", true);
        }
        else if (movement.x < 0)
        {
            animator.SetBool("WalkRight", false);
            animator.SetBool("WalkLeft", true);
        }
        else
        {
            animator.SetBool("WalkRight", false);
            animator.SetBool("WalkLeft", false);
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FixedUpdate()
    {

        if (!PlayerState.IsBusy) rb.linearVelocity = movement * (speed * slowMultiplier);

    }

    public void SetSlowMultiplier(float value)
    {
        slowMultiplier = value;
    }
}
