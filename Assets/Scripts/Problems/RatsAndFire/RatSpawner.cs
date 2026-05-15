using UnityEngine;
using System.Collections.Generic;

public class RatSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ratPrefab;
 

    [SerializeField] private float SpawnInterval = 3f;
    [SerializeField] private int SpawnLimit = 10;

    private bool isActive = false;
    private float timer = 0f;

    private List<GameObject> rats = new List<GameObject>();


    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= SpawnInterval)
        {
            timer = 0f;
            SpawnRat();
        }
    }

    private void SpawnRat()
    {
        if (ratPrefab == null) return;

        rats.RemoveAll(r => r == null);

        if (rats.Count < SpawnLimit)
        {
            GameObject rat = Instantiate(ratPrefab, transform.position, Quaternion.identity);
            rats.Add(rat);
        }

    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (active) timer = 0f;
    }
}
