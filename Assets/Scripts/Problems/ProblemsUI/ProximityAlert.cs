using UnityEngine;

public class ProximityAlert : MonoBehaviour
{
    private bool playerInside = false;
    private Component FatherObject;

    private Sprite toolSprite;
    private Sprite problemSprite;


    private void Awake()
    {
       FatherObject = VerifyParentComponent();
    }

    Component VerifyParentComponent()
    {
        BoxMachine box = GetComponentInParent<BoxMachine>();
        if (box != null) return box;

        FuseBox fuseBox = GetComponentInParent<FuseBox>();
        if (fuseBox != null) return fuseBox;

        RatNest ratNest = GetComponentInParent<RatNest>();
        if (ratNest != null) return ratNest;

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || playerInside || FatherObject == null) return;

        bool p = false;

        if (FatherObject is BoxMachine box)
        {
            p = box.HasProblem;
            if (p)
            {
                problemSprite = box.GetProblemSprite();
                toolSprite = box.GetToolSprite();
            }
        }
        else if (FatherObject is FuseBox fuseBox)
        {
            p = fuseBox.HasProblem;
            if (p)
            {
                problemSprite = fuseBox.GetProblemSprite();
                toolSprite = fuseBox.GetToolSprite();
            }
        }
        else if (FatherObject is  RatNest ratNest)
        {
            p = ratNest.HasProblem;
            if (p)
            {
                problemSprite = ratNest.GetProblemSprite();
                toolSprite = ratNest.GetToolSprite();
            }
        }

        if (!p) return;
        playerInside = true;

        ProblemUIManager.Instance.ShowProblem(toolSprite, problemSprite);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInside = false;
    }

}
