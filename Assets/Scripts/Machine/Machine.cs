using UnityEngine;

public class Machine : MonoBehaviour, IInteractable
{
    public bool isBroken = true;

    public void Interact(PlayerCarry player)
    {
        Debug.Log("Interacao Perto");
        if (isBroken)
        {
            isBroken = false;
            Debug.Log("Maquina consertada");
        }
    }

    public void GrappleInteract(PlayerCarry player)
    {
        Debug.Log("Interacao com grapple");
        if (isBroken)
        {
            isBroken = false;
            Debug.Log("Concertado a distancia");
        }
    }
}
