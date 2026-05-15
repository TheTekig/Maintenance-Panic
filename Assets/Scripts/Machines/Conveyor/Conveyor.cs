using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Conveyor : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;


    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minSpeed = 0f;
    [SerializeField] private float maxSpeed = 8f;

    private float defaultSpeed;
    private float speedMultiplier = 1f;


    [SerializeField] private float dangerSpeed = 6f;
    [SerializeField] private float fallChancePerSecond = 0.3f;

    private bool isActive = true;

    public float DangerSpeed => dangerSpeed;

    private Dictionary<Item, int> boxesOnBelt = new Dictionary<Item, int>();

    public float MoveSpeed => moveSpeed;
    public float MinSpeed => minSpeed;
    public float MaxSpeed => maxSpeed;

    private IProblem activeProblem;
    public bool HasProblem => activeProblem != null && !activeProblem.IsFixed;

    private void Start()
    {
        defaultSpeed = moveSpeed;
    }

    void Update()
    {
        if (!isActive || waypoints == null || waypoints.Length == 0) return;

        List<Item> toRemove = new List<Item>();

        foreach (var pair in new Dictionary<Item, int>(boxesOnBelt))
        {
            Item item = pair.Key;
            int waypointIndex = pair.Value;

            if (item == null)
            {
                toRemove.Add(item);
                continue;
            }

            if (IsBeingCarried(item))
            {
                toRemove.Add(item);
                continue;
            }

            if (moveSpeed >= dangerSpeed)
            {
                float chance = fallChancePerSecond * Time.deltaTime;
                if (Random.value < chance)
                {
                    EjectBox(item);
                    toRemove.Add(item);
                    continue;
                }
            }

            Transform target = waypoints[waypointIndex];
            item.transform.position = Vector2.MoveTowards(
                item.transform.position, target.position, (defaultSpeed * speedMultiplier) * Time.deltaTime);

            if (Vector2.Distance(item.transform.position, target.position) < 0.1f)
            {
                if (waypointIndex + 1 < waypoints.Length)
                {
                    boxesOnBelt[item] = waypointIndex + 1;
                }
                else
                {
                    toRemove.Add(item);
                    OnReachEnd(item);
                }
            }
        }
        foreach (var item in toRemove)
        {
            boxesOnBelt.Remove(item);
        }
    }

    private void LateUpdate()
    {
        if(!HasProblem) return;
        activeProblem.CheckFixed(ClearProblem);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if(item == null) return;
        if(!boxesOnBelt.ContainsKey(item))
        {
            AddBox(item);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if(item == null) return;

        if (boxesOnBelt.ContainsKey(item) && !IsBeingCarried(item))
        {
            boxesOnBelt.Remove(item);
            ReleaseBoxPhysics(item);
        }
    }

    public void AddBox(Item item)
    {
        if (waypoints == null || waypoints.Length == 0)  return;
        
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
        boxesOnBelt[item] = 0;
    }

    private void EjectBox(Item item)
    {
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            Vector2 ejectDir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
            rb.AddForce(ejectDir * moveSpeed * 1.5f, ForceMode2D.Impulse);
        }
    }

    private void OnReachEnd(Item item)
    {
        if (item == null) return;

        BoxStorage storage = FindNearestStorage(item.transform.position);

        if (storage != null)
        {
            storage.DeliverFromConveyor(item);
        }
        else
        {
            ReleaseBoxPhysics(item);
        }

    }

    private BoxStorage FindNearestStorage(Vector2 position)
    {
        BoxStorage[] storages = FindObjectsByType<BoxStorage>(FindObjectsSortMode.None);
        BoxStorage storage = null;
        float minDist = float.MaxValue;

        foreach (var s in storages)
        {
            float dist = Vector2.Distance(position, s.transform.position);
            if (dist < minDist) 
            {
                minDist = dist;
                storage = s;
            } 
        }
        return storage;
    }

    private void ReleaseBoxPhysics(Item item)
    {
        if (item == null) return;
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private bool IsBeingCarried(Item item)
    {
        PlayerCarry[] players = FindObjectsByType<PlayerCarry>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            if (player.GetCarried() == item) return true;
        }

        Rat[] rats = FindObjectsByType<Rat>(FindObjectsSortMode.None);
        foreach (var rat in rats)
        {
            if (rat.GetBox() == item) return true;
        }

        return false;

    }

    public void SetSpeed(float speed)
    {
        moveSpeed = Mathf.Clamp(speed, minSpeed, maxSpeed);
    }

    public void SetSpeedMultplier(float value)
    {
        speedMultiplier = value;
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if(!isActive)
        {
            foreach(var pair in boxesOnBelt)
            {
                if (pair.Key == null) continue;
                Rigidbody2D rb = pair.Key.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }

    public void ApplyProblem(IProblem problem)
    {
        if (HasProblem) return;

        activeProblem = problem;
        activeProblem.Activate();
    }

    public void ClearProblem()
    {
        activeProblem = null;
    }

    public bool HasBoxes => boxesOnBelt.Count > 0;
    public float GetSpeed() => moveSpeed;
    public float GetDangerSpeed() => dangerSpeed;
}
