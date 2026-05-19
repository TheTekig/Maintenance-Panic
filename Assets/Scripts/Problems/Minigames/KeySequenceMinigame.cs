using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class KeySequenceMinigame : MinigameBase
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI sequenceText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float timeLimit = 5f;

    private KeyCode[] possibleKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G };
    private List<KeyCode> sequence = new List<KeyCode>();
    private int currentIndex = 0;
    private float timer = 0f;
    private bool running = false;
    private const int SEQUENCE_LENGTH = 4;

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
        currentIndex = 0;
        timer = timeLimit;

        sequence.Clear();
        for(int i = 0; i < SEQUENCE_LENGTH; i++)
        {
            sequence.Add(possibleKeys[UnityEngine.Random.Range(0, possibleKeys.Length)]);
        }

        UpdateUI();
    }

    private void Update()
    {
        if (!running) return;

        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            running = false;
            PlayerState.SetBusy(false);
            panel.SetActive(false);
            Complete(false);
            return;
        }

        foreach(KeyCode key in possibleKeys)
        {
            if (Input.GetKeyDown(key))
            {
                if (key == sequence[currentIndex])
                {
                    currentIndex++;
                    if (feedbackText) feedbackText.text = "?";

                    if (currentIndex >= sequence.Count)
                    {
                        running = false ;
                        PlayerState.SetBusy(false);
                        panel.SetActive(false);
                        Complete(true);
                        return;
                    }
                }
                else
                {
                    currentIndex = 0;
                    if (feedbackText) feedbackText.text = "?";
                }
                UpdateUI();
                break;
            }
        }
    }

    private void UpdateUI()
    {
        if (sequenceText == null) return;

        string display = "";
        for(int i = 0; i < sequence.Count; i++)
        {
            string key = sequence[i].ToString();
            if (i < currentIndex)
            {
                display += $"<color=green>[{key}]</color>";
            }
            else if (i == currentIndex)
            {
                display += $"<color=yellow>[{key}]</color>";
            }
            else
            {
                display += $"[{key}]";
            }
        }
        sequenceText.text = display;
    }
}
