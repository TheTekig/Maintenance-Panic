using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Points Settings")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int pointsPerBox = 10;

    [Header("GameOver Settings")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private CountdownPulse countdownPulse;
    [SerializeField] private float warningThreshold = 10f;
    [SerializeField] private float maxIdleTime = 20f;
    private float idleTimer;
    private bool gameOver;

    private int score = 0;

    private int boxes = 0;

    private float playTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        warningPanel.SetActive(false);
        UpdateUI();
    }

    public void AddScore()
    {
        score += pointsPerBox;
        boxes += 1;
        RegisterBoxDelivered();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }

    public void RegisterBoxDelivered()
    {
        idleTimer = 0f; 
        warningPanel.SetActive(false);
    }
    
    private void Update()
    {
        if (gameOver) return;

        playTime += Time.deltaTime;
        idleTimer += Time.deltaTime;

        float remainingTime = maxIdleTime - idleTimer;
        remainingTime = Mathf.Max(0f, remainingTime);

        if (remainingTime <= warningThreshold)
        {
            warningPanel.SetActive(true);
            countdownText.text = Mathf.CeilToInt(remainingTime).ToString();
            countdownPulse.Animate(remainingTime);
        }
        else
        {
            warningPanel.SetActive(false);
            countdownPulse.ResetVisual();
        }

        if (idleTimer >= maxIdleTime)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        gameOver = true;
        Debug.Log("Game Over! No boxes delivered in time.");
        GameStats.Score = score;
        GameStats.Boxes = boxes;
        GameStats.TimePlayed = playTime;
        SceneManager.LoadScene("GameOverScene");
    }

    public int GetScore() => score;
}
