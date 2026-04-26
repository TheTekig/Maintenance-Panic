using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConveyorPanel : MonoBehaviour
{
    [Header("Reference Settings")]
    [SerializeField] private Conveyor conveyor;

    [Header("UI Settings")]
    [SerializeField] private GameObject panelUI;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TextMeshProUGUI speedLabel;
    [SerializeField] private TextMeshProUGUI warningLabel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button stopButton;

    private bool isOpen = false;


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
        if (isOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            ClosePanel();
        }
    }

    public void Interact(PlayerCarry player)
    {
        if (isOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void GrappleInteract()
    {         // No grapple interaction needed for the panel
    }

    private void OpenPanel()
    {
        if(panelUI == null || conveyor == null) return;

        isOpen = true;
        panelUI.SetActive(true);

        if (speedSlider != null) speedSlider.value = conveyor.GetSpeed();

        UpdateUI(conveyor.GetSpeed());
    }

    private void ClosePanel()
    {
        isOpen = false;
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
            if(speedSlider != null) speedSlider.value = defaultSpeed;
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
            }
            else if (speed >= danger)
            {
                warningLabel.text = "Warning: Conveyor is at dangerous speed!";
            }
            else if (speed >= danger * 0.75f)
            {
                warningLabel.text = "Caution: Conveyor is approaching dangerous speed.";
            }
            else
            {
                warningLabel.text = "Conveyor at regular speed";
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}
