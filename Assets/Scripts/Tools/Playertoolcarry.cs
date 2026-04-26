using UnityEngine;

public class Playertoolcarry : MonoBehaviour
{
    [SerializeField] private Vector2 toolOffset = new Vector2(0f, 0.8f);

    private Tool currentTool;
    public bool HasTool => currentTool != null;
    public Tool CurrentTool => currentTool;

    private void Update()
    {
        if (currentTool != null)
        {
            currentTool.transform.position = (Vector2)transform.position + (Vector2)(transform.right * toolOffset.x + transform.up * toolOffset.y);
        }

        if(Input.GetKeyDown(KeyCode.Q) && HasTool)
        {
            DropTool();
        }
    }

    public bool TryPickup(Tool tool)
    {
        if (HasTool) return false;
        currentTool = tool;
        return true;
    }

    public void DropTool()
    {
        if (currentTool == null) return;

        currentTool.Drop((Vector2)transform.position + Vector2.down * 0.5f);
        currentTool = null;

    }

    public Tooltype? GetTooltype()
    {
        if (!HasTool) return null;
        return currentTool.tooltype;
    }
    
}
