using System.Collections;
using UnityEngine;

public class Rat : MonoBehaviour
{
    public enum RatState
    {
        SearchingBox,
        ChasingBox,
        Eating,
        Fleeing,
    }

    [SerializeField] private float health = 10f;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float eatTime = 4f;
    [SerializeField] private float searchInterval = 1f;
    [SerializeField] private float chaseRadius = 5f;

    [SerializeField] private float fleeSpeed = 3.5f;
    [SerializeField] private float fleeDistance = 3f;

    [SerializeField] private Transform player;
    [SerializeField] private PlayerCarry playerCarry;
    [SerializeField] private float detectDistance = 2.5f;

    private Animator animator;
    private Rigidbody2D rb;
    private RatState state = RatState.SearchingBox;

    private SpriteRenderer sprite;

    private Item targetBox;
    private float eatTimer = 0f;
    private float searchTimer = 0f;

    [SerializeField] private float wanderRadius = 2f;
    [SerializeField] private float wanderInterval = 2f;

    private Vector2 fleePos;

    private float wanderTimer = 0f;
    private Vector2 wanderTarget;

    public bool RatCarring = false;
    public Item carriedbox;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        switch (state)
        {
            case RatState.SearchingBox : UpdateSearching(); break;
            case RatState.ChasingBox : UpdateChasing(); break;
            case RatState.Fleeing : UpdateFleeing(); break;
            case RatState.Eating : UpdateEating(); break; 
        }
    }

    private void UpdateSearching()
    {
        Wander();

        searchTimer -= Time.deltaTime;

        if (searchTimer > 0f) { return; }

        searchTimer = searchInterval;

        targetBox = FindNearestBox();

        if (targetBox != null)
        {
            state = RatState.ChasingBox;            
        }

    }

    private void UpdateChasing()
    {
        Item nearest = FindNearestBox();
        if (nearest == null)
        {
            targetBox = null;
            state = RatState.SearchingBox;
            return;
        }

        targetBox = nearest;

        float dist = Vector2.Distance(transform.position, targetBox.transform.position);

        if(dist > chaseRadius)
        {
            targetBox = null;
            state = RatState.SearchingBox;
            return;
        }

        MoveTowards(targetBox.transform.position, speed);
        if (dist < 0.7f)
        {
            RatCarring = true;
            GrabBox();
        }
    }

    private void UpdateFleeing()
    {
        if (targetBox == null)
        {
            state = RatState.SearchingBox;
            return;
        }

        CheckPlayerNearby();

        targetBox.transform.position = (Vector2)transform.position + Vector2.up * 0.3f;
        MoveTowards(fleePos, fleeSpeed);

        if (Vector2.Distance(transform.position, fleePos) < 0.3f)
        {
            eatTimer = eatTime;
            state = RatState.Eating;
        }
    }

    private void UpdateEating()
    {
        eatTimer -= Time.deltaTime;

        if (eatTimer <= 0)
        {
            if (targetBox != null)
            {
                Item boxToDestroy = targetBox;

                boxToDestroy.gameObject.SetActive(false);

                targetBox = null;
                carriedbox = null;
                RatCarring = false;

                Destroy(boxToDestroy.gameObject);
            }

            searchTimer = searchInterval;
            state = RatState.SearchingBox;
        }
    }


    private void GrabBox()
    {

        carriedbox = targetBox;

        if (playerCarry != null && playerCarry.GetCarried() == targetBox)
        {
            targetBox = null;
            state = RatState.SearchingBox;
            return;
        }
  
        fleePos = (Vector2)transform.position + Random.insideUnitCircle.normalized * fleeDistance;


        Rigidbody2D boxRB = targetBox.GetComponent<Rigidbody2D>();
        Collider2D boxCol = targetBox.GetComponent<Collider2D>();

        if (boxRB != null) boxRB.bodyType = RigidbodyType2D.Kinematic;
        if (boxCol != null) boxCol.enabled = false;

        state = RatState.Fleeing;
    }

    private void MoveTowards(Vector2 target, float spd)
    {
        animator.SetBool("IsMoving", true);
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.MovePosition((Vector2)transform.position + dir * spd * Time.deltaTime);
    }

    private Item FindNearestBox()
    {

        Item[] boxes = FindObjectsByType<Item>(FindObjectsSortMode.None);
        Item nearest = null;
        float minDist = float.MaxValue;

        foreach (Item item in boxes)
        {
            if (item.itemType() == ItemType.Box)
            {

                bool carried = false;

                if (playerCarry != null && playerCarry.GetCarried() == item)
                {
                    carried = true;
                    break;
                } 

                if (carried) continue;

                float dist = Vector2.Distance(transform.position, item.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = item;
                }
            }
        }
        return nearest;
    }

    private void CheckPlayerNearby()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectDistance)
        {
            Vector2 directionAway = ((Vector2)transform.position - (Vector2)player.position).normalized;
            fleePos = (Vector2)transform.position + directionAway * fleeDistance;
        }
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer < 0f)
        {
            wanderTimer = wanderInterval;
            wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
        }
        MoveTowards(wanderTarget, speed);
    }

    public Item GetBox()
    {
        return carriedbox;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        StartCoroutine(TakingDamageEffect(2.0f));
        if (health < 0f)
        {
            Kill();
        }
    }

    private void Kill()
    {
        if (carriedbox != null)
        {
            Rigidbody2D boxRB = targetBox.GetComponent<Rigidbody2D>();
            Collider2D boxCol = targetBox.GetComponent<Collider2D>();

            if (boxRB != null) boxRB.bodyType = RigidbodyType2D.Dynamic;
            if (boxCol != null) boxCol.enabled = true;
        }

        Destroy(gameObject);
    }

    public IEnumerator TakingDamageEffect(float time)
    {
        sprite.color = Color.green;
        yield return new WaitForSeconds(time);
        sprite.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (targetBox != null)
        {
            Gizmos.DrawLine(transform.position, targetBox.transform.position);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        if (fleePos != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, fleePos.normalized);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position , 0.7f );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectDistance);
    }
}
