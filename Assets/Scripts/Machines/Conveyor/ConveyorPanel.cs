using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConveyorPanel : MonoBehaviour, IInteractable
{
    [Header("Reference Settings")]
    [SerializeField] private Conveyor conveyor;

    [Header("UI Settings")]
    [SerializeField] private GameObject panelUI;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TextMeshProUGUI speedLabel;
    [SerializeField] private TextMeshProUGUI warningLabel;
    [SerializeField] private Image warningColor;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button stopButton;

    private bool isOpen = false;
    private bool justOpened = false;
    [SerializeField] public bool isStopped => conveyor != null && conveyor.GetSpeed() <= 0f;


    void Start()
    {

        if (panelUI != null) panelUI.SetActive(false);

        if (speedSlider != null && conveyor != null)
        {
            speedSlider.minValue = conveyor.MinSpeed;
            speedSlider.maxValue = conveyor.MaxSpeed;
            speedSlider.value = conveyor.GetSpeed();
            speedSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);
        if (stopButton != null) stopButton.onClick.AddListener(ToggleStop);
    }

    void Update()
    {
        if (!isOpen) return;

        if (justOpened) { justOpened = false; return; }

        if (isOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            ClosePanel();
        }
    }

    public void Interact(PlayerCarry player)
    {
        Debug.Log("Interacted with Conveyor Panel");
        if (isOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void GrappleInteract(PlayerCarry player)
    {         // No grapple interaction needed for the panel
    }

    private void OpenPanel()
    {
        PlayerState.SetBusy(true);
        Debug.Log($"OpenPanel chamado! panelUI={panelUI} | conveyor={conveyor} | panelUI.activeSelf={panelUI?.activeSelf}");
        if (panelUI == null || conveyor == null)
        {
            Debug.LogError("panelUI ou conveyor está NULO! Verifique o Inspector.");
            return;
        }

        isOpen = true;
        justOpened = true;
        panelUI.SetActive(true);

        Debug.Log($"activeSelf={panelUI.activeSelf} | activeInHierarchy={panelUI.activeInHierarchy} | pai={panelUI.transform.parent?.gameObject.activeInHierarchy}");
        Debug.Log($"panelUI ativado! activeSelf agora = {panelUI.activeSelf}");

        if (speedSlider != null) speedSlider.value = conveyor.GetSpeed();

        UpdateUI(conveyor.GetSpeed());
    }

    private void ClosePanel()
    {
        PlayerState.SetBusy(false);
        isOpen = false;
        justOpened = false;
        if (panelUI != null) panelUI.SetActive(false);
    }

    private void OnSliderChanged(float value)
    {
        if (conveyor == null) return;
        conveyor.SetSpeed(value);
        UpdateUI(value);
    }

    private void ToggleStop()
    {
        if (conveyor == null) return;

        bool currentlyActive = conveyor.GetSpeed() > 0f;

        if (currentlyActive)
        {
            conveyor.SetSpeed(0f);
            if (speedSlider != null) speedSlider.value = 0f;
            if (stopButton != null) stopButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        }
        else
        {
            float defaultSpeed = conveyor.MaxSpeed * 0.4f;
            conveyor.SetSpeed(defaultSpeed);
            if (speedSlider != null) speedSlider.value = defaultSpeed;
            if (stopButton != null)
            {
                stopButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            }
        }

        UpdateUI(conveyor.GetSpeed());


    }

    private void UpdateUI(float speed)
    {
        if (speedLabel != null) speedLabel.text = $"Speed: {speed:F1}";

        if (warningLabel != null)
        {
            float danger = conveyor.GetDangerSpeed();

            if (speed <= 0f)
            {
                warningLabel.text = "Conveyor is stopped.";
                if (warningColor != null) warningColor.color = Color.gray;
            }
            else if (speed >= danger)
            {
                warningLabel.text = "Warning: Conveyor is at dangerous speed!";
                if (warningColor != null) warningColor.color = Color.red;
            }
            else if (speed >= danger * 0.75f)
            {
                warningLabel.text = "Caution: Conveyor is approaching dangerous speed.";
                if (warningColor != null) warningColor.color = Color.yellow;
            }
            else
            {
                warningLabel.text = "Conveyor at regular speed";
                if (warningColor != null) warningColor.color = Color.green;
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}
