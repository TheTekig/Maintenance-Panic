using UnityEngine;

public class Machine : MonoBehaviour, IInteractable
{
    public bool isBroken = true;

    public void Interact()
    {
        Debug.Log("Interacao Perto");
        if (isBroken)
        {
            isBroken = false;
            Debug.Log("Maquina consertada");
        }
    }

    public void GrappleInteract()
    {
        Debug.Log("Interacao com grapple");
        if (isBroken)
        {
            isBroken = false;
            Debug.Log("Concertado a distancia");
        }
    }
}
