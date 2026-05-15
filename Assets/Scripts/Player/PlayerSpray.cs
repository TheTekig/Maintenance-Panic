using UnityEngine;

public class PlayerSpray : MonoBehaviour
{
    [SerializeField] private SprayArea sprayPrefab;

    private PlayerCarry carry;
    private SprayArea currentSpray;

    private void Awake()
    {
        carry = GetComponent<PlayerCarry>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !PlayerState.IsBusy)
        {
            StartSpray();
        }
        if (Input.GetMouseButtonUp(1))
        {
            StopSpray();
        }
        UpdateSprayPosition();
    }

    private void StartSpray()
    {
        if (!carry.HasItem) return;

        Item item = carry.GetCarried();

        if (item == null) return;

        ItemType type = item.itemType();

        if (type != ItemType.Extinguisher && type != ItemType.Poison) return;

        Transform sprayPoint = item.GetSprayPoint();

        currentSpray = Instantiate(sprayPrefab, sprayPoint.position, sprayPoint.rotation);

        currentSpray.transform.SetParent(sprayPoint);

        currentSpray.Setup(type);
    }

    private void StopSpray()
    {
        if (currentSpray != null)
        {
            Destroy(currentSpray.gameObject);

            currentSpray = null;
        }
    }

    private void UpdateSprayPosition()
    {
        if (currentSpray == null) return;

        Item item = carry.GetCarried();

        if(item == null) return;

        Transform sprayPoint = item.GetSprayPoint();

        currentSpray.transform.position = sprayPoint.position;

        currentSpray.transform.rotation = sprayPoint.rotation;
    }
}
