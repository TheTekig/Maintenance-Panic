using NUnit.Framework;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MachineBreakProblem : IProblem
{
    public string ProblemName => "Maquina Quebrada";

    public bool IsFixed {  get; private set; }

    public ItemType RequiredTool => ItemType.Screwdriver;

    private SkillCheckMinigame minigame;

    public MachineBreakProblem(SkillCheckMinigame minigame)
    {
        this.minigame = minigame;
        Debug.Log("Maquina Travada - Chave Fenda");
    }

    public void Activate()
    {
        IsFixed = false;
    }

    public void TryFix(PlayerCarry player, System.Action onFixed)
    { 
        if(player.GetCarried().itemType() != RequiredTool)
        {
            Debug.Log("Ferramenta Errada - Use Screwdriver");
            return;
        }

        MinigameManager.Instance.StartMinigame(minigame, Success =>
        {
            if (Success)
            {
                IsFixed = true;
                player.Drop();
                onFixed?.Invoke();
                Debug.Log("Maquina Concertada");
            }
            else
            {
                Debug.Log("Minigame falhou");
            }
        });
    }

    public void Fix() => IsFixed = true;
}

public class StuckBoxProblem : IProblem
{
    public string ProblemName => "Caixa Emperrada";
    public bool IsFixed { get; private set; }
    public ItemType? RequiredTool => null;

    private KeySequenceMinigame minigame;

    public StuckBoxProblem(KeySequenceMinigame minigame)
    {
        this.minigame = minigame;
    }

    public void Activate()
    {
        IsFixed = false;
    }

    public void TryFix(PlayerCarry player, System.Action onFixed)
    {
        MinigameManager.Instance.StartMinigame(minigame, success =>
        {
            if (success)
            {
                IsFixed = true;
                onFixed?.Invoke();
                Debug.Log("Caixa Desengasgada");
            }
            else
            {
                Debug.Log("Nao conseguiu resolver");
            }
        });
    }

    public void Fix() => IsFixed = true;
}

public class ConveyorJamProblem : IProblem
{
    public string ProblemName => "Esteira Travada";
    public bool IsFixed { get; private set; }
    public ItemType RequiredTool => ItemType.Oil;

    private KeySequenceMinigame minigame;

    public ConveyorJamProblem(KeySequenceMinigame minigame)
    {
        this.minigame = minigame;
    }

    public void Activate()
    {
        IsFixed = false;
        Debug.Log("Esteira Travada - Oleo");
    }

    public void TryFix(PlayerCarry player, System.Action onFixed)
    {
        if(player.GetCarried().itemType() != RequiredTool)
        {
            Debug.Log("Ferramenta Errada - Oleo");
            return;
        }

        MinigameManager.Instance.StartMinigame(minigame, success =>
        {
            if (success)
            {
                IsFixed = true;
                player.Drop();
                onFixed?.Invoke();
                Debug.Log("Esteira Destravada");
            }
        });
    }

    public void Fix() => IsFixed = true;
}

public class PowerOutageProblem : IProblem
{
    public string ProblemName => "Queda de Energia";
    public bool IsFixed { get; private set; }
    public ItemType RequiredTool => ItemType.Wire;

    private Light2D globalLight;
    private float originalIntensity;
    private WireMinigame minigame;

    public PowerOutageProblem(Light2D globalLight, WireMinigame minigame)
    {
        this.globalLight = globalLight;
        this.minigame = minigame;
    }

    public void Activate()
    {
        IsFixed = false;
        if (globalLight != null)
        {
            originalIntensity = globalLight.intensity;
            globalLight.intensity = 0.15f;
        }
        Debug.Log("?? Queda de energia! Vá ao quadro de luz com o Fio Elétrico.");
    }

    public void TryFix(PlayerCarry player, System.Action onFixed)
    {
        if (player.GetCarried().itemType() != ItemType.Wire)
        {
            Debug.Log("Ferramenta Errada - Wire");
            return;
        }

        MinigameManager.Instance.StartMinigame(minigame, success =>
        {
            if (success)
            {
                IsFixed = true;
                player.Drop();
                if (globalLight != null) globalLight.intensity = originalIntensity;
                onFixed?.Invoke();
                Debug.Log("Energia Restaurada");
            }
        });
    }

    public void Fix()
    {
        IsFixed = true;
        if (globalLight != null) globalLight.intensity = originalIntensity;
    }
}