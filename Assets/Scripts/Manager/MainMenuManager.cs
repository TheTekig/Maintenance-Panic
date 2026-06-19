using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("StoryScene");
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("ControlsScene");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Saindo do jogo...");
    }
}