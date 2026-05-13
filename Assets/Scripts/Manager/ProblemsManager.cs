using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class ProblemsManager : MonoBehaviour
{
    [SerializeField] private BoxMachine[] machines;
    [SerializeField] private FuseBox fuseBox;
    [SerializeField] private Light2D globalLight;

    [SerializeField] private WireMinigame minigameWire;

    [SerializeField] private float initialInterval = 20f;
    [SerializeField] private float minimumInterval = 6f;
    [SerializeField] private float escalationRate = 0.5f;

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
        if (machines == null || machines.Length == 0) return;

        List<BoxMachine> available = new List<BoxMachine>();
        foreach (var m in machines)
        {
            if (!m.HasProblem) available.Add(m);
        }

        if (available.Count == 0) return;

        BoxMachine target = available[Random.Range(0, available.Count)];
        IProblem problem = CreateRandomProblem(target);

        if (problem != null)
        {
            target.ApplyProblem(problem);
        }
    }

    private IProblem CreateRandomProblem(BoxMachine machine)
    {
        int[] weights = { 25, 20, 20, 10, 15, 10 };
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
                    case 0: return new MachineBreakProblem(machine.GetSkillCheckMinigame());
                    case 1: return new StuckBoxProblem(machine.GetKeySequenceMinigame());
                    case 2: return new ConveyorJamProblem(machine.GetKeySequenceMinigame());
                    case 3:
                        var outage = new PowerOutageProblem(globalLight, minigameWire);
                        fuseBox?.RegisterOutage(outage);
                        return outage;
                }
            }
        }
        return new MachineBreakProblem(machine.GetSkillCheckMinigame());
    }
}
