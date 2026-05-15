using UnityEngine;
using System.Collections.Generic;

public class FireNode : MonoBehaviour
{
    [SerializeField] private Fire firePrefab;
    [SerializeField] private List<FireNode> nearbyNodes = new();

    private Fire currentFire;

    public bool HasFire => currentFire != null;

    public void Ignite(Fire firePrefab)
    {
        if (HasFire) return;

        currentFire = Instantiate(firePrefab, transform.position, Quaternion.identity);

        currentFire.Setup(this);
    }

    public void ClearFire()
    {
        currentFire = null;
    }

    public List<FireNode> GetNearbyNodes()
    {
        return nearbyNodes;
    }
}
