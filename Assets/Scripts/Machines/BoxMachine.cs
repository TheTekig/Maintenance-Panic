using UnityEngine;

public class BoxMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private BoxSpawner spawner;
    [SerializeField] private Conveyor conveyor;

    [SerializeField] private SkillCheckMinigame minigameSkillCheck;
    [SerializeField] private KeySequenceMinigame minigameKeySequence;

    public bool HasProblem => activeProblem != null && !activeProblem.IsFixed;

    private IProblem activeProblem;

    public void ApplyProblem(IProblem problem)
    {
        if (HasProblem) return;

        activeProblem = problem;
        activeProblem.Activate();

        if (problem is MachineBreakProblem)
        {
            spawner?.SetActive(false);
        }
        if (problem is ConveyorJamProblem || problem is PowerOutageProblem)
        {
            conveyor?.SetActive(false);
            spawner?.SetActive(false);
        }
        if (problem is StuckBoxProblem)
        {
            spawner?.SetActive(false);
        }
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

        if (activeProblem is MachineBreakProblem mb)
        {
            mb.TryFix(player, ClearProblem);
        }
        else if (activeProblem is StuckBoxProblem sb)
        {
            sb.TryFix(player, ClearProblem);
        }
        else if (activeProblem is ConveyorJamProblem cj)
        {
            cj.TryFix(player, ClearProblem);
        }
    }

    public void ClearProblem()
    {
        activeProblem = null;
        spawner?.SetActive(true);
        conveyor?.SetActive(true);
    }

    public Sprite GetToolSprite() => activeProblem.ToolSprite;
    public Sprite GetProblemSprite() => activeProblem.ProblemSprite;

    public SkillCheckMinigame GetSkillCheckMinigame() => minigameSkillCheck;
    public KeySequenceMinigame GetKeySequenceMinigame() => minigameKeySequence;
}
