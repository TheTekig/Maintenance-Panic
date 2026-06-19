using UnityEngine;

public class ProblemUIManager : MonoBehaviour
{
    public static ProblemUIManager Instance;

    [SerializeField] private Transform container;
    [SerializeField] private ProblemAlertUI cardPrefab;

    [SerializeField] private Sprite test1;
    [SerializeField] private Sprite test2;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }

    public void ShowProblem( Sprite tool, Sprite problem)
    {
        Debug.Log(cardPrefab);
        Debug.Log(container);

        ProblemAlertUI card = Instantiate(cardPrefab, container, false);

        Debug.Log(card.transform.parent);
      

        card.Setup(tool, problem);
        card.PlaySpawnAnimation();
    }
}
