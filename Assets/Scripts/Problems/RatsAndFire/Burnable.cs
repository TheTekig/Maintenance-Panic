using UnityEngine;

public class Burnable : MonoBehaviour
{
    [SerializeField] private float burnDuration = 5f;

    private float burnTimer;
    public bool IsBurning { get; private set; }

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!IsBurning) return;

        burnTimer += Time.deltaTime;
        sr.color = Color.red;

        if (burnTimer >= burnDuration)
        {
            DestroyObject();
        }
    }

    public void StartBurn()
    {
        if (IsBurning) return;

        IsBurning = true;
        burnTimer = 0f;
        Debug.Log(gameObject.name + "Start Burning");
    }

    public void StopBurn()
    {
        if (!IsBurning) return;
        IsBurning = false;
        sr.color = Color.white;
    }

    private void DestroyObject()
    {
        Debug.Log(gameObject.name + "was destroyed by the fire");
        Destroy(gameObject);
    }
}
