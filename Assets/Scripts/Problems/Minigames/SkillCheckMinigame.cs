using UnityEngine;

public class SkillCheckMinigame : MinigameBase
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform needle;
    [SerializeField] private RectTransform target;
    [SerializeField] private float needleSpeed = 180f;

    private bool running = false;
    private float needleAngle = 0;
    private float targetStart;
    private float targetSize = 40f;


    protected override void OnBegin()
    {
        panel.SetActive(true);
        running = true;
        needleAngle = 0f;
        targetStart = UnityEngine.Random.Range(30f,300f);

        if (target != null)
        {
            target.localEulerAngles = new Vector3(0, 0, -targetStart);
        }
    }

    private void Update()
    {
        if (!running) return;

        needleAngle = (needleAngle + needleSpeed * Time.deltaTime) % 360f;
        if (needle != null)
        {
            needle.localEulerAngles = new Vector3(0, 0, -needleAngle);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            running = false;
            panel.SetActive(false);

            float adjustedNeedle = (360f - needleAngle) % 360f;

            float diff = Mathf.Abs(
                Mathf.DeltaAngle(adjustedNeedle, targetStart)
            );

            bool inZone = diff <= targetSize * 0.5f;

            Debug.Log(
                $"needle={adjustedNeedle:F1} " +
                $"target={targetStart:F1} " +
                $"diff={diff:F1} " +
                $"inZone={inZone}"
            );

            Complete(inZone);
        }
    }
}
