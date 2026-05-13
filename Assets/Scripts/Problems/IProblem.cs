using UnityEngine;

public interface IProblem
{ 
    string ProblemName { get; }
    bool IsFixed { get; }

    void Activate();
    void Fix();
}
