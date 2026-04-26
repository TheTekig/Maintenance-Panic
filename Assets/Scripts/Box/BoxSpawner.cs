using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [Header("Referece")]
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Conveyor conveyor;

    [Header("Settings")]
    [SerializeField] private float spawnInterval = 3f;

    private bool isActive = true;
    private float timer = 0f;

    private void Update()
    {
        if (!isActive) return;
        
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBox();
        }
    }

    private void SpawnBox()
    {
        if (boxPrefab == null || conveyor == null) return;

        GameObject boxObj = Instantiate(boxPrefab, transform.position, Quaternion.identity);
        Box box = boxObj.GetComponent<Box>();

        if (box != null)
        {
            conveyor.AddBox(box);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (active) timer = 0f; // Reset timer when reactivating
    }
}
