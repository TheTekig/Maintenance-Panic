using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ProblemAlertUI : MonoBehaviour
{
    [SerializeField] private Image problemIcon;
    [SerializeField] private Image toolIcon;

    [SerializeField] private Vector2 iconSize = new Vector2(32, 32);

    [SerializeField] private float lifeTime = 4f;

    private RectTransform rect;

    private Vector2 exitPos;
    private Vector2 targetPos;
    private bool moving = false;
    private bool exiting = false;

    private float timer = 0;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Setup(Sprite problem, Sprite tool)
    {
        problemIcon.sprite = problem;
        toolIcon.sprite = tool;

        ResizeImage(toolIcon);
        ResizeImage(problemIcon);
    }

    private void ResizeImage(Image image)
    {
        RectTransform rect = image.GetComponent<RectTransform>();
        rect.sizeDelta = iconSize;
        image.preserveAspect = true;
    }

    public void PlaySpawnAnimation()
    {
        rect = GetComponent<RectTransform>();

        targetPos = Vector2.zero;

        exitPos = targetPos + new Vector2(0, -100);

        rect.anchoredPosition = exitPos;

        moving = true;

        timer = lifeTime;
    }

    private void Update()
    {
        if (moving)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * 10f);

            if (Vector2.Distance(rect.anchoredPosition, targetPos) < 1f)
            {
                rect.anchoredPosition = targetPos;
                moving = false;
            }
        }

        if (!moving && !exiting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                exiting = true;
            }
        }
        

        if (exiting)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, exitPos, Time.deltaTime * 10f);

            if (Vector2.Distance(rect.anchoredPosition, exitPos) < 1f)
            {
                Destroy(gameObject);
            }
        }
      
        

    }
}
