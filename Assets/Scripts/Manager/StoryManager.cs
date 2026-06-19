using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [Header("Slides")]
    [SerializeField] private StorySlide[] slides;

    [Header("UI")]
    [SerializeField] private Image storyImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private Image fadePanel;

    [Header("Settings")]
    [SerializeField] float textSpeed = 0.03f;
    [SerializeField] float fadeDuraction = 1f;

    private int currentSlide;

    private bool canContinue;
    private bool isTyping;

    private void Start()
    {
        continueText.gameObject.SetActive(false);
        StartCoroutine(StartStory());
    }

    private void Update()
    {
        if(!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        if(isTyping)
        {
            StopAllCoroutines();

            storyText.text = slides[currentSlide].text;

            isTyping = false;

            canContinue = true;
            continueText.gameObject.SetActive(true);

            return;
        }

        if(canContinue)
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

        if(currentSlide >= slides.Length)
        {
            StartCoroutine(LoadGame());
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

        while(timer < fadeDuraction)
        {
            timer += Time.deltaTime;

            Color c = fadePanel.color;

            c.a = Mathf.Lerp(1, 0, timer / fadeDuraction);

            fadePanel.color = c;

            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;

        while(timer < fadeDuraction)
        {
            timer += Time.deltaTime;

            Color c = fadePanel.color;

            c.a = Mathf.Lerp(0,1 , timer / fadeDuraction);

            fadePanel.color = c;

            yield return null;
        }
    }

    IEnumerator LoadGame()
    {
        yield return FadeOut();
        SceneManager.LoadScene("Gameplay");
    }
}
