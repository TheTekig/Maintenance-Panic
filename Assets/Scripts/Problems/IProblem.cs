using UnityEngine;

public interface IProblem
{ 
    string ProblemName { get; }
    bool IsFixed { get; }

    Sprite ProblemSprite { get; }

    Sprite ToolSprite { get; }


    public void CheckFixed(System.Action onFixed);
    void Activate();
    void Fix();
}
