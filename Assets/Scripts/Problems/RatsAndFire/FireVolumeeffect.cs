using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Coloque esse script no mesmo prefab do Fire
// Crie um Volume no prefab: Add Component > Volume
// No Volume: Is Global = FALSE, adicione Lens Distortion e Chromatic Aberration
// Coloque um SphereCollider/CircleCollider2D no Volume com Is Trigger = true pra definir a área
[RequireComponent(typeof(Volume))]
public class FireVolumeEffect : MonoBehaviour
{
    [Header("Raio de influência")]
    [SerializeField] private float maxRadius = 4f;

    [Header("Lens Distortion")]
    [SerializeField] private float maxDistortionIntensity = -0.4f;  // negativo = banha

    [Header("Chromatic Aberration")]
    [SerializeField] private float maxChromaticIntensity = 0.6f;

    [Header("Vignette")]
    [SerializeField] private float maxVignetteIntensity = 0.35f;
    [SerializeField] private Color vignetteColor = new Color(1f, 0.3f, 0f); // laranja fogo

    [Header("Suavização")]
    [SerializeField] private float lerpSpeed = 4f;

    private Volume volume;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    private Transform playerTransform;
    private float currentWeight = 0f;
    private float targetWeight = 0f;

    void Awake()
    {
        volume = GetComponent<Volume>();
        volume.isGlobal = true;
        volume.weight = 0f;

        // Pega os overrides do Volume
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out vignette);

        // Começa zerado
        volume.weight = 0f;

        if (lensDistortion != null)
            lensDistortion.intensity.Override(maxDistortionIntensity);

        if (chromaticAberration != null)
            chromaticAberration.intensity.Override(maxChromaticIntensity);

        if (vignette != null)
        {
            vignette.color.Override(vignetteColor);
            vignette.intensity.Override(maxVignetteIntensity);
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            // Quanto mais perto, mais forte (0 = longe, 1 = em cima)
            targetWeight = Mathf.Clamp01(1f - (dist / maxRadius));
        }
        else
        {
            targetWeight = 0f;
        }

        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * lerpSpeed);
        volume.weight = currentWeight;
    }

    // Fire.cs já detecta entrada/saída do player — chame esses métodos de lá
    public void OnPlayerEnter(Transform player)
    {
        playerTransform = player;
    }

    public void OnPlayerExit()
    {
        playerTransform = null;
    }
}