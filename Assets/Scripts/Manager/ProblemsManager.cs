using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class ProblemsManager : MonoBehaviour
{
    [SerializeField] private BoxMachine[] machines;
    [SerializeField] private RatNest[] spawns;
    [SerializeField] private Conveyor[] conveyors;
    [SerializeField] private FuseBox fuseBox;
    [SerializeField] private Light2D globalLight;

    [SerializeField] private WireMinigame minigameWire;

    [SerializeField] private float initialInterval = 20f;
    [SerializeField] private float minimumInterval = 6f;
    [SerializeField] private float escalationRate = 0.5f;

    [Header("Problem Sprites")]
    [SerializeField] private Sprite machineSprite;
    [SerializeField] private Sprite fuseBoxSprite;
    [SerializeField] private Sprite ratSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite ConveyorPanelSprite;

    [Header("Tool Sprites")]
    [SerializeField] private Sprite wrenchSprite;
    [SerializeField] private Sprite ScrewdriverSprite;
    [SerializeField] private Sprite OilSprite;
    [SerializeField] private Sprite WireSprite;
    [SerializeField] private Sprite extinguisherSprite;
    [SerializeField] private Sprite poisonSprite;
    [SerializeField] private Sprite InteractSprite;

    private float timer = 0f;
    private float currentInterval;
    private float elapseTime = 0f;

    void Start()
    {
        currentInterval = initialInterval;
    }

    // Update is called once per frame
    void Update()
    {
        elapseTime += Time.deltaTime;
        timer += Time.deltaTime;

        currentInterval = Mathf.Max(
            minimumInterval,
            initialInterval - (elapseTime / 60f) * escalationRate * initialInterval);

        if (timer >= currentInterval)
        {
            timer = 0f;
            TriggerRandomProblem();
        }
    }

    private void TriggerRandomProblem()
    {
        int roll = Random.Range(0, 100);

        if (roll < 70)
        {
            TriggerMachineProblem();
        }
        else
        {
            TriggerGlobalProblem();
        }
    }

    private void TriggerMachineProblem()
    {
        int roll = Random.Range(0, 100);
        if (roll < 70)
        {
            TriggerBoxMachineProblem();
        }
        else
        {
            TriggerConveyorProblem();
        }
    }

    private void TriggerConveyorProblem()
    {
        if (conveyors == null || conveyors.Length == 0) return;

        List<Conveyor> available = new();

        foreach (var c in conveyors)
        {
            if (!c.HasProblem)
            {
                available.Add(c);
            }
        }

        if (available.Count == 0) return;

        Conveyor target = available[Random.Range(0, available.Count)];

        IProblem problem = CreatConveyorProblem(target);

        if (problem != null)
        {
            target.ApplyProblem(problem);
        }

    }

    private void TriggerBoxMachineProblem()
    {
        List<BoxMachine> available = new();

        foreach (var m in machines)
        {
            if(!m.HasProblem) available.Add(m);
        }

        if (available.Count == 0) return;

        BoxMachine target = available[Random.Range(0,available.Count)];
        IProblem problem = CreateRandomProblem(target);

        if(problem != null)
        {
            target.ApplyProblem(problem);
            Debug.Log("Tentando criar card UI");
            ProblemUIManager.Instance.ShowProblem(problem.ToolSprite, problem.ProblemSprite);
        }
    }

    private void TriggerGlobalProblem()
    {
        int roll = Random.Range(0, 3);

        switch(roll)
        {
            case 0:
                TriggerPowerOutage();
                break;

            case 1:
                TriggerRatInfestation();
                break;

            case 2:
                TriggerFireProblem();
                break;
        }
    }


    private void TriggerFireProblem()
    {
        FireProblem fire = new FireProblem(fireSprite, extinguisherSprite);

        fire.Activate();
    }

    private void TriggerRatInfestation()
    {
        if (spawns == null || spawns.Length == 0) return;

        List<RatNest> nests = new();

        foreach(var n in spawns)
        {
            if(!n.HasProblem) nests.Add(n);
        }

        if (nests.Count == 0) return;

        RatNest target = nests[Random.Range(0,nests.Count)];
        IProblem problem = new RatInfestation(target.GetSpawner(), target.GetLeverMinigame(), ratSprite, wrenchSprite);

        target.ApplyProblem(problem);
        Debug.Log("Tentando criar card UI");
        ProblemUIManager.Instance.ShowProblem(problem.ToolSprite, problem.ProblemSprite);
        Debug.Log("Ifestacao de Ratos Iniciada");

    }

    private void TriggerPowerOutage()
    {
        var outage = new PowerOutageProblem(globalLight, minigameWire, fuseBoxSprite, WireSprite);
        fuseBox?.RegisterOutage(outage);
        Debug.Log("Tentando criar card UI");
        ProblemUIManager.Instance.ShowProblem(outage.ToolSprite, outage.ProblemSprite);
        Debug.Log("Apagao Iniciado");
    }

    private IProblem CreatConveyorProblem(Conveyor conveyor)
    {
        Debug.Log("Chamando Conveyor Problem");
        int roll = Random.Range(0, 2);
            
        switch (roll)
        {
            case 0: return new ConveyorOverclockProblem(conveyor, InteractSprite, ConveyorPanelSprite);
            
            case 1: return new ConveyorStoppedProblem(conveyor, InteractSprite, ConveyorPanelSprite);
        }

        return null;
    }

    private IProblem CreateRandomProblem(BoxMachine machine)
    {
        const int BREAK_WEIGHT = 25;
        const int STUCK_WEIGHT = 20;
        const int JAM_WEIGHT = 20;

        int[] weights = { BREAK_WEIGHT, STUCK_WEIGHT, JAM_WEIGHT };
        int total = 0;

        foreach (int w in weights) total += w;

        int roll = Random.Range(0, total);
        int acc = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            acc += weights[i];
            if (roll < acc)
            {
                switch (i)
                {
                    case 0: return new MachineBreakProblem(machine.GetSkillCheckMinigame(), machineSprite, ScrewdriverSprite);
                    case 1: return new StuckBoxProblem(machine.GetKeySequenceMinigame(), machineSprite, ScrewdriverSprite);
                    case 2: return new ConveyorJamProblem(machine.GetKeySequenceMinigame(), machineSprite, OilSprite);
                }
            }
        }
        return new MachineBreakProblem(machine.GetSkillCheckMinigame(), machineSprite, ScrewdriverSprite);
    }
}
