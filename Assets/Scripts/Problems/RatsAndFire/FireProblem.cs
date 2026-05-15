using UnityEngine;

public class FireProblem : IProblem
{
    public string ProblemName => "Incendio";

    public bool IsFixed {  get; private set; }

    public ItemType RequiredTool => ItemType.Extinguisher;

    public Sprite ProblemSprite => problemSprite;
    public Sprite ToolSprite => toolSprite;

    private Sprite problemSprite;
    private Sprite toolSprite;

    public FireProblem( Sprite fireSprite, Sprite extinguisherSprite)
    {
        problemSprite = fireSprite;
        toolSprite = extinguisherSprite;

    }

    public void Activate()
    {
        IsFixed = false;
        FireManager.Instance.StartProblem(this);

    }

    public void TryFix( PlayerCarry player, System.Action onFixed )
    {

    }

    public void CheckFixed(System.Action onFixed)
    {
    }

    public void Fix()
    {
        if (IsFixed) return;

        IsFixed = true;
    }
}

