using UnityEngine;
using System.Collections.Generic;

public class Fire : MonoBehaviour
{
    [SerializeField] private float health = 10f;

    [SerializeField] private float slowMultiplier = 0.5f;

    [SerializeField] private Fire firePrefab;
    [SerializeField] private float spreadInterval = 5f;
    [SerializeField] private float spreadChance = 0.4f;

    private FireNode ownerNode;
    private FireVolumeEffect volumeEffect;


    public void Setup(FireNode node)
    {
        ownerNode = node;
        volumeEffect = GetComponent<FireVolumeEffect>();

        FireManager.Instance.RegisterFire();
        InvokeRepeating(nameof(Spread), spreadInterval, spreadInterval);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f )
        {
            Extinguish();
        }
    }

    private void Extinguish()
    {
        FireManager.Instance.UnregisterFire();
        ownerNode.ClearFire();
        Destroy(gameObject);
    }

    private void Spread()
    {
        if (Random.value > spreadChance) return;

        List<FireNode> nearby = ownerNode.GetNearbyNodes();

        if (nearby.Count == 0) return;

        List<FireNode> avaliable = nearby.FindAll(n => !n.HasFire);

        if (avaliable.Count == 0) return;

        FireNode target = nearby[Random.Range(0, nearby.Count)];

        if (target.HasFire) return;

        target.Ignite(firePrefab);
        
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!collision.CompareTag("Player")) return;

        PlayerMovement move = collision.GetComponent<PlayerMovement>();
        if (move != null)
        {
            move.SetSlowMultiplier(slowMultiplier);
        }

        PlayerGrappler grapple = collision.GetComponent<PlayerGrappler>();
        if (grapple != null)
        {
            grapple.SetBlocked(true);
        }

        volumeEffect?.OnPlayerEnter(collision.transform);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Burnable burnable = collision.GetComponent<Burnable>();
        if (burnable != null)
        {
            if (!burnable.IsBurning)
            {
                burnable.StartBurn();
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement move =
            collision.GetComponent<PlayerMovement>();

        if (move != null)
        {
            move.SetSlowMultiplier(1f);
        }

        PlayerGrappler grapple =
            collision.GetComponent<PlayerGrappler>();

        if (grapple != null)
        {
            grapple.SetBlocked(false);
        }

        volumeEffect?.OnPlayerExit();
    }
}
