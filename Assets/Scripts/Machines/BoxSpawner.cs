using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Conveyor conveyor;
    [SerializeField] private ConveyorPanel conveyorPanel;

    [SerializeField] private float spawnInterval = 3f;

    private bool isActive = true;
    private float timer = 0f;

    private void Awake()
    {
        conveyorPanel = GetComponent<ConveyorPanel>();
    }

    void Update()
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
        if (boxPrefab == null || conveyor == null || (conveyorPanel != null && conveyorPanel.isStopped)) return;

        GameObject boxObj = Instantiate(boxPrefab, transform.position, Quaternion.identity);
        Item item = boxObj.GetComponent<Item>();

        if (item != null)
        {
            conveyor.AddBox(item);
        }
    }

    public void SetActive(bool Active)
    {
        isActive = Active;
        if (Active) timer = 0f;
    }
}
