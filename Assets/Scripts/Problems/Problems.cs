using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MachineBreakProblem : IProblem
{
    public string ProblemName => "Maquina Quebrada";

    public Sprite ProblemSprite => problemSprite;
    public Sprite ToolSprite => toolSprite;

    private Sprite problemSprite;
    private Sprite toolSprite;

    public bool IsFixed {  get; private set; }

    public ItemType RequiredTool => ItemType.Screwdriver;

    private SkillCheckMinigame minigame;

    public MachineBreakProblem(SkillCheckMinigame minigame, Sprite problemSprite, Sprite toolSprite)
    {
        this.minigame = minigame;

        this.problemSprite = problemSprite;
        this.toolSprite = toolSprite;

        Debug.Log("Maquina Travada - Chave Fenda");
    }

    public void Activate()
    {
        IsFixed = false;
    }

    public void TryFix(PlayerCarry player, System.Action onFixed)
    {
        Item carried = player.GetCarried();
        if(carried == null ||carried.itemType() != RequiredTool)
        {
            Debug.Log("Ferramenta Errada - Use Screwdriver");
            return;
        }

        MinigameManager.Instance.StartMinigame(minigame, Success =>
        {
            if (Success)
            {
                IsFixed = true;
                player.GetCarried()?.Drop(player);
                onFixed?.Invoke();
                Debug.Log("Maquina Concertada");
            }
            else
            {
                Debug.Log("Minigame falhou");
            }
        });
    }

    public void CheckFixed(System.Action onFixed)
    {
    }
    public void Fix() => IsFixed = true;
}

public class StuckBoxProblem : IProblem
{
    public string ProblemName => "Caixa Emperrada";

    public Sprite ProblemSprite => problemSprite;
    public Sprite ToolSprite => toolSprite;

    private Sprite problemSprite;
    private Sprite toolSprite;

    public bool IsFixed { get; private set; }
    public ItemType? RequiredTool => null;

    private KeySequenceMinigame minigame;

    public StuckBoxProblem(KeySequenceMinigame minigame, Sprite problemSprite, Sprite toolSprite)
    {
        this.minigame = minigame;

        this.problemSprite = problemSprite;
        this.toolSprite = toolSprite;
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

    public void CheckFixed(System.Action onFixed)
    {
    }

    public void Fix() => IsFixed = true;
}

public class ConveyorJamProblem : IProblem
{
    public string ProblemName => "Esteira Travada";
    public Sprite ProblemSprite => problemSprite;
    public Sprite ToolSprite => toolSprite;

    private Sprite problemSprite;
    private Sprite toolSprite;

    public bool IsFixed { get; private set; }
    public ItemType RequiredTool => ItemType.Oil;

    private KeySequenceMinigame minigame;

    public ConveyorJamProblem(KeySequenceMinigame minigame, Sprite problemSprite, Sprite toolSprite)
    {
        this.minigame = minigame;
        this.problemSprite = problemSprite;
        this.toolSprite = toolSprite;
    }

    public void Activate()
    {
        IsFixed = false;
        Debug.Log("Esteira Travada - Oleo");
    }

    public void TryFix(PlayerCarry player, System.Action onFixed)
    {
        Item carried = player.GetCarried();
        if (carried == null || carried.itemType() != RequiredTool)
        {
            Debug.Log("Ferramenta Errada - Use Screwdriver");
            return;
        }

        MinigameManager.Instance.StartMinigame(minigame, success =>
        {
            if (success)
            {
                IsFixed = true;
                player.GetCarried()?.Drop(player);
                onFixed?.Invoke();
                Debug.Log("Esteira Destravada");
            }
        });
    }

    public void CheckFixed(System.Action onFixed)
    {
    }

    public void Fix() => IsFixed = true;
}

public class PowerOutageProblem : IProblem
{
    public string ProblemName => "Queda de Energia";

    public Sprite ProblemSprite => problemSprite;
    public Sprite ToolSprite => toolSprite;

    private Sprite problemSprite;
    private Sprite toolSprite;

    public bool IsFixed { get; private set; }
    public ItemType RequiredTool => ItemType.Wire;

    private Light2D globalLight;
    private float originalIntensity;
    private WireMinigame minigame;

    public PowerOutageProblem(Light2D globalLight, WireMinigame minigame, Sprite problemSprite, Sprite toolSprite)
    {
        this.globalLight = globalLight;
        this.minigame = minigame;

        this.problemSprite = problemSprite;
        this.toolSprite = toolSprite;
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
        Item carried = player.GetCarried();
        if (carried == null || carried.itemType() != RequiredTool)
        {
            Debug.Log("Ferramenta Errada - Use Screwdriver");
            return;
        }

        MinigameManager.Instance.StartMinigame(minigame, success =>
        {
            if (success)
            {
                IsFixed = true;
                player.GetCarried()?.Drop(player);
                if (globalLight != null) globalLight.intensity = originalIntensity;
                onFixed?.Invoke();
                Debug.Log("Energia Restaurada");
            }
        });
    }

    public void CheckFixed(System.Action onFixed)
    {
    }

    public void Fix()
    {
        IsFixed = true;
        if (globalLight != null) globalLight.intensity = originalIntensity;
    }
}

public class RatInfestation : IProblem
{
    public string ProblemName => "Infestacao";
    public Sprite ProblemSprite => problemSprite;
    public Sprite ToolSprite => toolSprite;

    private Sprite problemSprite;
    private Sprite toolSprite;
    public bool IsFixed { get; private set; }
    public ItemType RequiredTool => ItemType.Wrench;

    private LeverMinigame minigame;
    private RatSpawner spawner;

    public RatInfestation(RatSpawner spawner, LeverMinigame minigame, Sprite problemSprite, Sprite toolSprite)
    {
        this.spawner = spawner;
        this.minigame = minigame;

        this.problemSprite = problemSprite;
        this.toolSprite = toolSprite;
    }

    public void Activate()
    {
        IsFixed = false;
        spawner.SetActive(true);
        Debug.Log("Infestacao de Ratos Feche o Spawn com uma Wrench");
    }

    public void TryFix(PlayerCarry player, System.Action onFixed)
    {
        Item carried = player.GetCarried();
        if (carried == null || carried.itemType() != RequiredTool)
        {
            Debug.Log("Ferramenta Errada use uma Wrench");
            return;
        }

        MinigameManager.Instance.StartMinigame(minigame, success =>
        {
            if (success)
            {
                IsFixed = true;
                spawner.SetActive(false);
                player.GetCarried()?.Drop(player);
                onFixed?.Invoke();
                Debug.Log("Rat Spawner Fechado");
            }
        });
    }

    public void CheckFixed(System.Action onFixed)
    { 
    }

    public void Fix()
    {
        IsFixed = true;
    }

}

public class ConveyorOverclockProblem : IProblem
{
    public string ProblemName => "Overclock";
    public Sprite ProblemSprite => problemSprite;
    public Sprite ToolSprite => toolSprite;

    private Sprite problemSprite;
    private Sprite toolSprite;
    public bool IsFixed { get; private set; }

    private Conveyor conveyor;

    public ConveyorOverclockProblem(Conveyor conveyor, Sprite toolSprite, Sprite problemSprite)
    {
        this.conveyor = conveyor;
    }

    public void Activate()
    {
        IsFixed = false;
        conveyor.SetSpeed(conveyor.GetDangerSpeed() + 2f);
    }

    public void TryFix(PlayerCarry player, System.Action onFixed)
    {
      
    }

    public void CheckFixed(System.Action onFixed)
    {
        if (IsFixed) return;

        if (conveyor.GetSpeed() <= conveyor.GetDangerSpeed())
        {
            Fix();
            onFixed?.Invoke();
        }
    }

    public void Fix()
    {
        IsFixed = true;
    }
}

public class ConveyorStoppedProblem : IProblem
{
    public string ProblemName => "Stopped";

    public bool IsFixed { get; private set; }

    public Sprite ProblemSprite => problemSprite;
    public Sprite ToolSprite => toolSprite;

    private Sprite problemSprite;
    private Sprite toolSprite;

    private Conveyor conveyor;

    public ConveyorStoppedProblem(Conveyor conveyor, Sprite toolSprite, Sprite problemSprite)
    {
        this.conveyor = conveyor;
    }

    public void Activate()
    {
        IsFixed = false;

        conveyor.SetSpeed(0f);

        Debug.Log(
            "Conveyor parada!"
        );
    }

    public void TryFix(
        PlayerCarry player,
        System.Action onFixed)
    {
        if (IsFixed)
            return;

        if (conveyor.GetSpeed() > 0f)
        {
            Fix();

            onFixed?.Invoke();
        }
    }

    public void CheckFixed(System.Action onFixed)
    {
        if (IsFixed)
            return;

        if (conveyor.GetSpeed() > 0f)
        {
            Fix();

            onFixed?.Invoke();
        }
    }

    public void Fix()
    {
        IsFixed = true;

        Debug.Log(
            "Conveyor religada!"
        );
    }
}