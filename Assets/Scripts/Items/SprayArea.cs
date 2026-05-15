using UnityEngine;

public class SprayArea : MonoBehaviour
{
    //[SerializeField] private float duration = 0.3f;
    [SerializeField] private float damage = 1f;

    [SerializeField] private SpriteRenderer coneVisual;
    [SerializeField] private ParticleSystem particles;

    private ItemType sprayType;

    public void Setup(ItemType type)
    {
        sprayType = type;

        ApplyVisual();
    }

    private void ApplyVisual()
    {
        var main = particles.main;

        if(sprayType == ItemType.Extinguisher)
        {
            Color color = new Color(0.8f, 0.95f, 1f, 0.6f);
            
            coneVisual.color = color;
            main.startColor = color;
        }

        if (sprayType == ItemType.Poison)
        {
            Color color = new Color(0.5f, 1f, 0.5f, 0.6f);

            coneVisual.color = color;
            main.startColor = color;
        }
    }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * 20f) * 0.03f;

        coneVisual.transform.localScale = new Vector3(2.5f * pulse, 1.2f * pulse, 1f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (sprayType == ItemType.Extinguisher)
        {
            Fire fire = other.GetComponent<Fire>();
            if (fire != null)
            {
                fire.TakeDamage(damage * Time.deltaTime);
            }
        }

        if (sprayType == ItemType.Poison)
        {
            Rat rat = other.GetComponent<Rat>();
            if (rat != null)
            {
                rat.TakeDamage(damage * Time.deltaTime);
            }
        }
    }
}
