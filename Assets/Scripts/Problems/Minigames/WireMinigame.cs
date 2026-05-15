using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class WireMinigame : MinigameBase
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] leftWires;
    [SerializeField] private Button[] rightWires;
    [SerializeField] private Image[] leftColors;
    [SerializeField] private Image[] rightColors;

    private Color[] wireColors = { Color.red, Color.blue, Color.yellow, Color.green };
    private int[] correctMapping;
    private int selectedLeft = -1;
    private int connectedCount = 0;
    private bool[] connected;

    protected override void OnBegin()
    {
        panel.SetActive(true);
        PlayerState.SetBusy(true);
        connectedCount = 0;
        selectedLeft = -1;
        connected = new bool[leftWires.Length];

        correctMapping = new int[leftWires.Length];
        List<int> indices = new List<int>();

        for (int i = 0; i < leftWires.Length; i++) indices.Add(i);

        for (int i = 0; i < leftWires.Length; i++)
        {
            int pick = UnityEngine.Random.Range(0, indices.Count);
            correctMapping[i] = indices[pick];
            indices.RemoveAt(pick);
        }

        for (int i = 0; i < leftWires.Length; i++)
        {
            leftWires[i].interactable = true;
            rightWires[i].interactable = true;

            Color c = wireColors[i % wireColors.Length];
            if (leftColors[i]) leftColors[i].color = c;
            if (rightColors[correctMapping[i]]) rightColors[correctMapping[i]].color = c;

            int idx = i;
            leftWires[i].onClick.RemoveAllListeners();
            leftWires[i].onClick.AddListener(() => OnLeftClick(idx));
            rightWires[i].onClick.RemoveAllListeners();
            rightWires[i].onClick.AddListener(() => OnRightClick(idx));

        }

    }

    private void OnLeftClick(int index)
    {
        if (connected[index]) return;
        selectedLeft = index;
    }

    private void OnRightClick(int index)
    {
        if (selectedLeft < 0) return;

        if (correctMapping[selectedLeft] == index)
        {
            connected[selectedLeft] = true;
            connectedCount++;

            leftWires[selectedLeft].interactable = false;
            rightWires[index].interactable = false;

            selectedLeft = -1;

            if (connectedCount >= leftWires.Length)
            {
                PlayerState.SetBusy(false);
                panel.SetActive(false);
                Complete(true);
            }
        }
        else
        {
            selectedLeft = -1;
        }

    }
}
