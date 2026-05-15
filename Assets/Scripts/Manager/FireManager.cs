using UnityEngine;
using System.Collections.Generic;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance;

    [SerializeField] List<FireNode> fireNodes = new();

    [SerializeField] private Fire firePrefab;

    [SerializeField] private int maxActiveFires = 20;

    [SerializeField] private float fireSpawnInterval = 25f;

    [SerializeField] private float fireSpawnChance = 0.5f;

    private FireProblem currentProblem;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InvokeRepeating(nameof(TryStartFire), fireSpawnInterval, fireSpawnInterval);
    }

    public void RegisterFire()
    {
        ActiveFires++;
    }

    public void UnregisterFire()
    {
        ActiveFires--;
        if (ActiveFires <= 0)
        {
            currentProblem?.Fix();
            currentProblem = null;
        }
    }

    public static int ActiveFires = 0;

    private void TryStartFire()
    {
        if (Random.value > fireSpawnChance) return;

        if (ActiveFires >= maxActiveFires) return;

        List<FireNode> available = fireNodes.FindAll(n => !n.HasFire);

        if (available.Count > 0) return;

        FireNode target = available[Random.Range(0, available.Count)];

        target.Ignite(firePrefab);
    }

    public void StartRandomFire()
    {
        if (ActiveFires >= maxActiveFires) return;

        List<FireNode> available =fireNodes.FindAll(n => !n.HasFire);

        if (available.Count == 0) return;

        FireNode target = available[ Random.Range( 0, available.Count ) ];

        target.Ignite(firePrefab);
    }

    public void StartProblem(FireProblem problem)
    {
        currentProblem = problem;
        StartRandomFire();
    }
}
