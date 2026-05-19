using UnityEngine;
using UnityEngine.UI;

//Minigame para fechar o spawn dos ratos

public class LeverMinigame : MinigameBase
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider[] Lever;
    [SerializeField] private float timeLimit = 5f;

    private bool running = false;
    private float timer = 0f;

    protected override void OnBegin()
    {
        foreach (var l in Lever)
        {
            l.value = 0f;
        }

        PlayerGrappler grappler = GetComponent<PlayerGrappler>();
        if (grappler != null)
        {
            grappler.CancelGrapple();
        }
        PlayerState.SetBusy(true);

        panel.SetActive(true);
        running = true;
        timer = timeLimit;

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
        bool AllComplete = true;

        foreach (var item in Lever)
        {

            if (item.value < 0.99f)
            {
                AllComplete = false;
                break;
            }
            
        }

        if (AllComplete)
        {
            running = false;
            PlayerState.SetBusy(false);
            panel.SetActive(false);
            Complete(true);
            return;
        }
    }
}
