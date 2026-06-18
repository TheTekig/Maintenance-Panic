using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance;

    [SerializeField] private Image flashImage;

    private void Awake()
    {
        Instance = this;
    }

    public void Flash(Color color, float maxAlpha, float duration)
    {
        StartCoroutine(FlashRoutine(color, maxAlpha, duration));
    }

    private IEnumerator FlashRoutine(Color color, float maxAlpha, float duration)
    {
        float timer = 0f;
        color.a = maxAlpha;

        flashImage.color = color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            Color current = color;

            current.a = Mathf.Lerp(maxAlpha, 0f, t);
            flashImage.color = current;
            yield return null;
        }

        color.a = 0f;
        flashImage.color = color;
    }
}
