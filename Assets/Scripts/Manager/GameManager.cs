using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int pointsPerBox = 10;

    private int score = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore()
    {
        score += pointsPerBox;
        UpdateUI();
        Debug.Log($"Score updated: {score}");
    }

    private void UpdateUI()
    {
        if(scoreText != null)
        {
            scoreText.text = $"Score: {score}";           
        }
    }

    public int GetScore() => score;
}
