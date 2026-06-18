using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickDebugger : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // Mostra o que o EventSystem tá vendo
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        if (results.Count == 0)
        {
            Debug.Log("NENHUM objeto detectado pelo EventSystem!");
            return;
        }

        foreach (var r in results)
        {
            Debug.Log($"Hit: {r.gameObject.name} | depth: {r.depth} | distance: {r.distance} | sorting: {r.sortingOrder}");
        }
    }
}