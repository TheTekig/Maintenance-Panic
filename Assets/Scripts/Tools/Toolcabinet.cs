using UnityEngine;

public class Toolcabinet : MonoBehaviour, IInteractable
{
    [Header("Configuration")]
    [SerializeField] private Tooltype tooltype;
    [SerializeField] private GameObject toolPrefab;
    [SerializeField] private Vector2 spawnOffset = new Vector2(0.5f, 0f);

    [Header("Cooldown")]
    [SerializeField] private float cooldownDuration = 3f;

    private float cooldownTimer = 0f;
    private bool onCooldown = false;

    void Update()
    {
        if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                onCooldown = false;
            }
        }
    }

    public void  Interact (PlayerCarry player)
    {
        Playertoolcarry toolCarry = player.GetComponent<Playertoolcarry>();

        if (toolCarry == null) return;

        if (!CanDispense(toolCarry)) return;

        if (toolCarry.HasTool) toolCarry.DropTool();
       
        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;
        GameObject obj = Instantiate(toolPrefab, spawnPos, Quaternion.identity);
        Tool tool = obj.GetComponent<Tool>();

        if (tool != null && toolCarry.TryPickup(tool))
        {
            tool.StartCarrying(player.transform);

            StartCoolDown();
        }

    }

    public void GrappleInteract(PlayerCarry player)
    {
        Playertoolcarry toolCarry = player.GetComponent<Playertoolcarry>();
        if (toolCarry == null) return;

        if (!CanDispense(toolCarry)) return;

        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;
        GameObject obj = Instantiate(toolPrefab, spawnPos, Quaternion.identity);
        Tool tool = obj.GetComponent<Tool>();

        if (tool != null)
        {
            tool.GrappleInteract(player);
            StartCoolDown();
        }


    }

    private bool CanDispense(Playertoolcarry toolCarry)
    {
        if (onCooldown)
        {
            Debug.Log("Toolcabinet is on cooldown.");
            return false;
        }
        if (toolPrefab == null)
        {
            Debug.LogWarning("Prefab Ferramenta nao atribuida");
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
