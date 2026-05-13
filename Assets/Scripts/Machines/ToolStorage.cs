using UnityEngine;
using UnityEngine.Rendering;

public class ToolStorage : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemType itemtype;
    [SerializeField] private GameObject ToolPrefab;
    [SerializeField] private Vector2 SpawnOffset = new Vector2(0.5f, 0f);

    [SerializeField] private float cooldownDuration = 3f;

    private float cooldownTimer = 0f;
    private bool onCooldown = false;

    void Update()
    {
        if (onCooldown)
        {
            cooldownTimer -= Time.time;
            if (cooldownTimer < 0f)
            {
                onCooldown = false;
            }
        }
    }

    public void Interact(PlayerCarry player)
    {
        Debug.Log("Interacting With Tool Storage");
        player = player.GetComponent<PlayerCarry>();

        if (player == null ) return;

        if (!CanDispense(player)) return;

        if (player.HasItem)
        {
            player.Drop();
        }

        Vector2 spawnPos = (Vector2)transform.position + SpawnOffset;
        GameObject obj = Instantiate(ToolPrefab, spawnPos, Quaternion.identity);
        Item item = obj.GetComponent<Item>();

        if (item != null & player.TryPickup(item))
        {
            item.StartCarrying(player.transform);
            StartCoolDown();
        }
    }

    private bool CanDispense(PlayerCarry player)
    {
        if (onCooldown)
        {
            return false;
        }
        if (ToolPrefab == null)
        {
            return false;
        }
        return true;
    }

    private void StartCoolDown()
    {
        onCooldown = true;
        cooldownTimer = cooldownDuration;
    }
}
