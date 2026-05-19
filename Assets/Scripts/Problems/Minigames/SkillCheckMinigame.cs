using UnityEngine;
using UnityEngine.UI;

public class SkillCheckMinigame : MinigameBase
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform needle;
    [SerializeField] private RectTransform target;
    [SerializeField] private float needleSpeed = 180f;
    [SerializeField] private float targetSize = 40f;

    private bool running = false;
    private float needleAngle = 0f;
    private float targetAngle = 0f;

    private const float NeedleOffset = -90f;
    private const float TargetOffset = -90f;


    protected override void OnBegin()
    {
        PlayerGrappler grappler = GetComponent<PlayerGrappler>();
        if (grappler != null)
        {
            grappler.CancelGrapple();
        }
        PlayerState.SetBusy(true);

        panel.SetActive(true);
        running = true;
        needleAngle = 0f;

        targetAngle = Random.Range(30f, 300f);

        Image targetImage = target.GetComponent<Image>();
        if (targetImage != null )
        {
            targetImage.fillAmount = Random.Range(0.08f, 0.18f);
            targetSize = targetImage.fillAmount * 360f;
        }

        if (target != null)
        {
            target.localEulerAngles = new Vector3(0, 0, -(targetAngle + TargetOffset));
        }

        if (needle != null)
        {
            needle.localEulerAngles = new Vector3(0f, 0f, NeedleOffset);
        }


    }

    private void Update()
    {
        if (!running) return;

        needleAngle = (needleAngle + needleSpeed * Time.deltaTime) % 360f;

        if (needle != null)
        {
            needle.localEulerAngles = new Vector3(0, 0, -(needleAngle) + NeedleOffset);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            running = false;
            panel.SetActive(false);
            PlayerState.SetBusy(false);

            float diff = Mathf.Abs(Mathf.DeltaAngle(needleAngle, targetAngle));
            bool inZone = diff <= targetSize * 0.5f;

            Debug.Log($"needle={needleAngle:F1} target={targetAngle:F1} diff={diff:F1} inZone={inZone}");

            Complete(inZone);
        }
    }
}
