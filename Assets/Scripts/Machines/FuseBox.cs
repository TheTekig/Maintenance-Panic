using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System;

public class FuseBox : MonoBehaviour, IInteractable
{
    private PowerOutageProblem activeProblem;

    [SerializeField] private Conveyor[] conveyor;
    [SerializeField] private BoxSpawner[] spawner;

    [SerializeField] private Light2D globalLight;
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0.8f;

    [SerializeField] private float flickerDuration = 0.8f;
    [SerializeField] private float flickerSpeed = 0.05f;
    [SerializeField] private float fadeSpeed = 2f;

    [SerializeField] private GameObject playerFlashLight;

    [SerializeField] private WireMinigame wireMinigame;

    public bool HasProblem => activeProblem != null && !activeProblem.IsFixed;

    private Coroutine currentRoutine;
    private Action onMachineCleared;
    public void RegisterOutage(PowerOutageProblem problem, Action onMachineCleared = null)
    {
        activeProblem = problem;
        this.onMachineCleared = onMachineCleared;
        Debug.Log("Falta de Energia detectada");

        if (currentRoutine != null )
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(PowerOutageSequence());
    }

    private IEnumerator PowerOutageSequence()
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            timer += flickerSpeed;

            if (globalLight != null )
            {
                globalLight.intensity = UnityEngine.Random.Range(0.2f, onIntensity);
            }

            yield return new WaitForSeconds(flickerSpeed);
        }

        yield return StartCoroutine(FadeLight(globalLight.intensity, offIntensity));

        if(playerFlashLight != null )
        {
            playerFlashLight.SetActive(true);
        }
    }

    public void Interact(PlayerCarry player)
    {
        if (activeProblem == null || activeProblem.IsFixed)
        {
            return;
        }

        if (player.GetCarried() == null) return;

        TryFixWithTool(player);

    }

    private void TryFixWithTool(PlayerCarry player)
    {
        if (MinigameManager.Instance.IsMinigameActive) return;

        void OnFixed()
        {
            activeProblem.Fix();
            activeProblem = null;

            onMachineCleared?.Invoke();
            onMachineCleared = null;

            if (currentRoutine != null)
            {
                StopCoroutine(currentRoutine);
            }

            currentRoutine = StartCoroutine(RestorePowerSequence());
        }
        if (activeProblem is PowerOutageProblem pop)
        {
            pop.TryFix(player, OnFixed);
        }
    }

    private IEnumerator RestorePowerSequence()
    {
        if (playerFlashLight != null)
        {
            playerFlashLight.SetActive(false);
        }
        yield return StartCoroutine(FadeLight(globalLight.intensity, onIntensity));
    }

    private IEnumerator FadeLight(float start, float target)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;

            if(globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(start, target, t);
            }
             yield return null; 
        }

        if (globalLight != null)
            globalLight.intensity = target;
    }

    public Sprite GetProblemSprite() => activeProblem.ProblemSprite;
    public Sprite GetToolSprite() => activeProblem.ToolSprite;
}
