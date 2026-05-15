using UnityEngine;

public class RatNest : MonoBehaviour, IInteractable
{
    [SerializeField] private RatSpawner spawner;
    [SerializeField] private LeverMinigame minigameLever;

    public bool HasProblem => activeProblem != null && !activeProblem.IsFixed;
    private IProblem activeProblem;

    public void ApplyProblem(IProblem problem)
    {
        if (HasProblem) return;

        activeProblem = problem;
        activeProblem.Activate();
    }

    public void Interact(PlayerCarry player)
    {
        if (!HasProblem)
        {
            return;
        }

        if (player.GetCarried() == null) return;
        TryFixWithTool(player);
    }

    private void TryFixWithTool(PlayerCarry player)
    {
        if (MinigameManager.Instance.IsMinigameActive) return;

        if (activeProblem is RatInfestation ri)
        {
            ri.TryFix(player, ClearProblem);
        }
    }

    public void ClearProblem()
    {
        activeProblem = null;
    }

    public LeverMinigame GetLeverMinigame() => minigameLever;
    public RatSpawner GetSpawner() => spawner;

    public Sprite GetProblemSprite() => activeProblem.ProblemSprite;
    public Sprite GetToolSprite() => activeProblem.ToolSprite;
}
