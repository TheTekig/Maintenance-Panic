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
    }

    public void Interact(PlayerCarry player)
    {
        if (!HasProblem)
        {
            Debug.Log("Maquina Sem Problemas");
            return;
        }

        if (player.GetCarried() == null) return;

        TryFixWithTool(player);
    }

    private void TryFixWithTool(PlayerCarry player)
    {
        if (MinigameManager.Instance.IsMinigameActive) return;

        void OnFixed()
        {
            activeProblem = null;
            spawner?.SetActive(true);
            conveyor?.SetActive(true);
        }

        if (activeProblem is MachineBreakProblem mb)
        {
            mb.TryFix(player, OnFixed);
        }
        else if (activeProblem is StuckBoxProblem sb)
        {
            sb.TryFix(player, OnFixed);
        }
        else if (activeProblem is ConveyorJamProblem cj)
        {
            cj.TryFix(player, OnFixed);
        }
    }

    public SkillCheckMinigame GetSkillCheckMinigame() => minigameSkillCheck;
    public KeySequenceMinigame GetKeySequenceMinigame() => minigameKeySequence;
}
