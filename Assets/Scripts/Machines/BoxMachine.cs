using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class BoxMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private BoxSpawner spawner;
    [SerializeField] private Conveyor conveyor;

    [SerializeField] private SkillCheckMinigame minigameSkillCheck;
    [SerializeField] private KeySequenceMinigame minigameKeySequence;

    [SerializeField] private ParticleSystem explosionFX;
    [SerializeField] private ParticleSystem sparksFX;
    [SerializeField] private ParticleSystem smokeFX;

    [SerializeField] private SpriteRenderer spriterer;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    [SerializeField] private float pulseSpeed = 1f;

    [SerializeField] private CinemachineImpulseSource impulseSource;

    private Animator animator;
    public bool HasProblem => activeProblem != null && !activeProblem.IsFixed;

    private IProblem activeProblem;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (HasProblem)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);

            spriterer.color = Color.Lerp(normalColor, warningColor, t);
        }
    }

    public void ApplyProblem(IProblem problem)
    {
        if (HasProblem) return;

        activeProblem = problem;
        activeProblem.Activate();

        StartCoroutine(BreakSequence());
        animator.SetBool("isBroken", true);

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
        sparksFX.Stop();
        explosionFX.Stop();
        smokeFX.Stop();
        animator.SetBool("isBroken", false);
        activeProblem = null;
        spawner?.SetActive(true);
        conveyor?.SetActive(true);
        spriterer.color = normalColor;
    }

    private IEnumerator BreakSequence()
    {
        sparksFX.Play();
        yield return new WaitForSeconds(0.5f);
        impulseSource.GenerateImpulse();
        explosionFX.Play();
        ScreenFlash.Instance.Flash(Color.red, 0.8f, 0.5f);
        smokeFX.Play();
    }

    public Sprite GetToolSprite() => activeProblem.ToolSprite;
    public Sprite GetProblemSprite() => activeProblem.ProblemSprite;

    public SkillCheckMinigame GetSkillCheckMinigame() => minigameSkillCheck;
    public KeySequenceMinigame GetKeySequenceMinigame() => minigameKeySequence;
}
