using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProblemArrowGuide : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject worldIconPrefab;

    [SerializeField] private float edgePadding = 60f;
    [SerializeField] private Vector3 worldIconOffset = new Vector3(0f, 1.2f, 0f);

    private class TrackedMachine
    {
        public Transform transform;
        public System.Func<bool> hasProblem;
        public RectTransform edgeArrow;
        public GameObject worldIcon;
    }

    private List<TrackedMachine> tracked = new();

    void Start()
    {
        // Registra todas as BoxMachines
        foreach (var m in FindObjectsByType<BoxMachine>(FindObjectsSortMode.None))
            Register(m.transform, () => m.HasProblem);

        // Registra FuseBox
        foreach (var f in FindObjectsByType<FuseBox>(FindObjectsSortMode.None))
            Register(f.transform, () => f.HasProblem);

        // Registra Conveyors
        foreach (var c in FindObjectsByType<Conveyor>(FindObjectsSortMode.None))
            Register(c.transform, () => c.HasProblem);

        // RatNest 
        foreach (var r in FindObjectsByType<RatNest>(FindObjectsSortMode.None))
            Register(r.transform, () => r.HasProblem);
    }

    private void Register(Transform t, System.Func<bool> hasProblemFunc)
    {
        tracked.Add(new TrackedMachine
        {
            transform = t,
            hasProblem = hasProblemFunc,
        });
    }

    void Update()
    {
        foreach (var entry in tracked)
        {
            if (entry.transform == null) continue;

            if (entry.hasProblem())
                HandleActive(entry);
            else
                HandleResolved(entry);
        }
    }

    // ??? Máquina com problema ????????????????????????????????????????????????

    private void HandleActive(TrackedMachine entry)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(entry.transform.position);
        bool visible = IsOnScreen(screenPos);

        // Ícone no mundo (aparece quando visível)
        if (visible)
        {
            EnsureWorldIcon(entry);
            DestroyEdgeArrow(entry);
        }
        else
        {
            DestroyWorldIcon(entry);
            EnsureEdgeArrow(entry, screenPos);
        }
    }

    // ??? Problema resolvido ??????????????????????????????????????????????????

    private void HandleResolved(TrackedMachine entry)
    {
        DestroyWorldIcon(entry);
        DestroyEdgeArrow(entry);
    }

    // ??? Ícone flutuando acima da máquina ???????????????????????????????????

    private void EnsureWorldIcon(TrackedMachine entry)
    {
        if (entry.worldIcon != null) return;
        if (worldIconPrefab == null) return;

        entry.worldIcon = Instantiate(
            worldIconPrefab,
            entry.transform.position + worldIconOffset,
            Quaternion.identity,
            entry.transform   // filho da máquina, segue ela
        );
    }

    private void DestroyWorldIcon(TrackedMachine entry)
    {
        if (entry.worldIcon == null) return;
        Destroy(entry.worldIcon);
        entry.worldIcon = null;
    }

    // ??? Seta na borda da tela ???????????????????????????????????????????????

    private void EnsureEdgeArrow(TrackedMachine entry, Vector3 screenPos)
    {
        if (entry.edgeArrow == null)
        {
            if (arrowPrefab == null) return;
            var go = Instantiate(arrowPrefab, transform);
            entry.edgeArrow = go.GetComponent<RectTransform>();
        }

        entry.edgeArrow.anchoredPosition = GetEdgePosition(screenPos);
        entry.edgeArrow.localEulerAngles = new Vector3(0f, 0f, GetAngle(screenPos));
    }

    private void DestroyEdgeArrow(TrackedMachine entry)
    {
        if (entry.edgeArrow == null) return;
        Destroy(entry.edgeArrow.gameObject);
        entry.edgeArrow = null;
    }

    // ??? Helpers de posição ??????????????????????????????????????????????????

    private bool IsOnScreen(Vector3 screenPos)
    {
        return screenPos.z > 0
            && screenPos.x > edgePadding
            && screenPos.x < Screen.width - edgePadding
            && screenPos.y > edgePadding
            && screenPos.y < Screen.height - edgePadding;
    }

    // Projeta o raio (centro ? alvo) até a borda do retângulo da tela
    private Vector2 GetEdgePosition(Vector3 screenPos)
    {
        Vector2 center = new(Screen.width / 2f, Screen.height / 2f);
        Vector2 dir = ((Vector2)screenPos - center).normalized;

        float halfW = Screen.width / 2f - edgePadding;
        float halfH = Screen.height / 2f - edgePadding;

        float tX = dir.x != 0 ? halfW / Mathf.Abs(dir.x) : float.MaxValue;
        float tY = dir.y != 0 ? halfH / Mathf.Abs(dir.y) : float.MaxValue;
        float t = Mathf.Min(tX, tY);

        Vector2 edgeScreen = center + dir * t;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, edgeScreen, null, out Vector2 local);

        return local;
    }

    // Ângulo para apontar a seta na direção da máquina
    private float GetAngle(Vector3 screenPos)
    {
        Vector2 center = new(Screen.width / 2f, Screen.height / 2f);
        Vector2 dir = (Vector2)screenPos - center;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
