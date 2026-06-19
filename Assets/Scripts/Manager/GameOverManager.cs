using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Slides")]
    [SerializeField] private GameOverSlide[] slides;

    [Header("Story UI")]
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private Image storyImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private Image fadePanel;

    [Header("Report UI")]
    [SerializeField] private GameObject reportPanel;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI boxesText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.03f;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Medals")]
    [SerializeField] private Image medalImage;
    [SerializeField] private TextMeshProUGUI medalNameText;

    [SerializeField] private Sprite estagiario;
    [SerializeField] private Sprite auxiliar;
    [SerializeField] private Sprite tecnico;
    [SerializeField] private Sprite supervisor;
    [SerializeField] private Sprite lenda;

    private int currentSlide;

    private bool canContinue;
    private bool isTyping;

    private void Start()
    {
        reportPanel.SetActive(false);

        continueText.gameObject.SetActive(false);

        StartCoroutine(StartStory());
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (reportPanel.activeSelf)
            return;

        if (isTyping)
        {
            StopAllCoroutines();

            storyText.text = slides[currentSlide].text;

            isTyping = false;

            canContinue = true;

            continueText.gameObject.SetActive(true);

            return;
        }

        if (canContinue)
        {
            NextSlide();
        }
    }

    IEnumerator StartStory()
    {
        yield return FadeIn();

        ShowSlide();
    }

    void ShowSlide()
    {
        continueText.gameObject.SetActive(false);

        storyImage.sprite = slides[currentSlide].image;

        StartCoroutine(TypeText(slides[currentSlide].text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        canContinue = false;

        storyText.text = "";

        foreach (char c in text)
        {
            storyText.text += c;

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        canContinue = true;

        continueText.gameObject.SetActive(true);
    }

    void NextSlide()
    {
        currentSlide++;

        if (currentSlide >= slides.Length)
        {
            StartCoroutine(ShowReportSequence());

            return;
        }

        StartCoroutine(ChangeSlide());
    }

    IEnumerator ChangeSlide()
    {
        yield return FadeOut();

        ShowSlide();

        yield return null;

        yield return FadeIn();
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            Color c = fadePanel.color;

            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            fadePanel.color = c;

            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            Color c = fadePanel.color;

            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);

            fadePanel.color = c;

            yield return null;
        }
    }

    IEnumerator ShowReportSequence()
    {
        yield return FadeOut();

        ShowReport();

        yield return FadeIn();
    }

    public void ShowReport()
    {
        storyPanel.SetActive(false);

        reportPanel.SetActive(true);

        continueText.gameObject.SetActive(false);

        LoadStats();

        SetupMedal();
    }

    private void LoadStats()
    {
        scoreText.text =
            GameStats.Score.ToString();

        boxesText.text =
            GameStats.Boxes.ToString();

        float totalSeconds =
            GameStats.TimePlayed;

        int minutes =
            Mathf.FloorToInt(totalSeconds / 60);

        int seconds =
            Mathf.FloorToInt(totalSeconds % 60);

        timeText.text =
            $"{minutes:00}:{seconds:00}";
    }

    private void SetupMedal()
    {
        int score = GameStats.Score;

        if (score < 200)
        {
            medalImage.sprite = estagiario;

            medalNameText.text = "ESTAGIÁRIO";
        }
        else if (score < 500)
        {
            medalImage.sprite = auxiliar;

            medalNameText.text = "AUXILIAR";
        }
        else if (score < 1000)
        {
            medalImage.sprite = tecnico;

            medalNameText.text = "TÉCNICO";
        }
        else if (score < 2000)
        {
            medalImage.sprite = supervisor;

            medalNameText.text = "SUPERVISOR";
        }
        else
        {
            medalImage.sprite = lenda;

            medalNameText.text = "LENDA DA BOXCORP";
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Gameplay");
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}