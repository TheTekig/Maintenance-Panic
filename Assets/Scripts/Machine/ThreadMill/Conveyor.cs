using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Conveyor : MonoBehaviour
{
    [Header("Waypoints - Arrange in the order the box should follow")]
    [SerializeField] private Transform[] waypoints;

    [Header("Speed")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minSpeed = 0f;
    [SerializeField] private float maxSpeed = 8f;

    [SerializeField] private float dangerSpeed = 6f;
    [SerializeField] private float fallChancePerSecond = 0.3f;

    private bool isActive = true;

    private Dictionary<Box, int> boxesOnBelt = new Dictionary<Box, int>();

    public float MoveSpeed => moveSpeed;
    public float MinSpeed => minSpeed;
    public float MaxSpeed => maxSpeed;

    private void Update()
    {
        if (!isActive || waypoints == null || waypoints.Length == 0) return;

        List<Box> toRemove = new List<Box>();

        foreach ( var pair in new Dictionary<Box, int>(boxesOnBelt))
        {
            Box box = pair.Key;
            int waypointIndex = pair.Value;
            
            if (box == null)
            {
                toRemove.Add(box);
                continue;
            }

            if (IsBeingCarried(box))
            {
                toRemove.Add(box);
                continue;
            }

            if (moveSpeed >= dangerSpeed)
            {
               float chance = fallChancePerSecond * Time.deltaTime;
                if (Random.value < chance)
                {
                    EjectBox(box);
                    toRemove.Add(box);
                    continue;
                }
            }

            Transform target = waypoints[waypointIndex];
            box.transform.position = Vector2.MoveTowards(box.transform.position, target.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(box.transform.position, target.position) < 0.1f)
            {
                if (waypointIndex + 1 < waypoints.Length)
                {
                    boxesOnBelt[box] = waypointIndex + 1;
                }
                else
                {
                    toRemove.Add(box);
                    OnReachedEnd(box);
                }
            }
        }

        foreach (var box in toRemove)
        {
            boxesOnBelt.Remove(box);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Box box = collision.GetComponent<Box>();
        if (box == null) return;
        if (boxesOnBelt.ContainsKey(box)) return;

        AddBox(box);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Box box = collision.GetComponent<Box>();
        if (box == null) return;

        if (boxesOnBelt.ContainsKey(box) && !IsBeingCarried(box))
        {
            boxesOnBelt.Remove(box);
            ReleaseBoxPhysics(box);
        }
    }

    public void AddBox(Box box)
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Rigidbody2D rb = box.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }

        boxesOnBelt[box] = 0;
    }

    private void EjectBox(Box box)
    {
        Rigidbody2D rb = box.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            Vector2 ejectDir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
            rb.AddForce(ejectDir * moveSpeed * 1.5f, ForceMode2D.Impulse);
        }
        Debug.Log($"Box {box.name} ejected from conveyor due to high speed!");
    }

    private void OnReachedEnd(Box box)
    {
        if (box == null) return;

        StorageBox storage = FindNearestStorage(box.transform.position);

        if (storage != null)
        {
            storage.DeliverFromConveyor(box);
        }
        else
        {
            ReleaseBoxPhysics(box);
        }

    }

    private StorageBox FindNearestStorage(Vector2 position)
    {
        StorageBox[] storages = FindObjectsByType<StorageBox>(FindObjectsSortMode.None);
        StorageBox storage = null;
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

    private void ReleaseBoxPhysics(Box box)
    {
        if (box == null) return;
        Rigidbody2D rb = box.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private bool IsBeingCarried(Box box)
    {
        PlayerCarry[] players = FindObjectsByType<PlayerCarry>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.GetCarried() == box)
            {
                return true;
            }
        }

        return false;
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = Mathf.Clamp(speed, minSpeed, maxSpeed);
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (!isActive)
        {
            foreach (var pair in boxesOnBelt)
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

    public bool HasBoxes => boxesOnBelt.Count > 0;
    public float GetSpeed() => moveSpeed;
    public float GetDangerSpeed() => dangerSpeed;


}
