using UnityEngine;
using TMPro;


public class CountdownPulse : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [Header("Scale")]
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float pulseAmount = 0.15f;

    [Header("Color")]
    [SerializeField] private Color safeColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Animate(float remainingTime)
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * pulse;

        float t = 1f - (remainingTime / 10f);

        text.color = Color.Lerp(safeColor, dangerColor, t);

        pulseSpeed = Mathf.Lerp(1.5f, 4f, t);
    }

    public void ResetVisual()
    {
        transform.localScale = originalScale;
        text.color = safeColor;
    }
}
