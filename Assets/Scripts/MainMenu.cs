using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{

    [Header("UI")]
    public GameObject TutorialButton;
    public GameObject Level1Button;
    public GameObject Level2Button;
    public GameObject GoBackButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CloseSelect(); // hide the level select buttons until the player clicks "Play"
    }

    // Wired to TutorialButton / Level1Button / Level2Button
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        // Make the level scene's own lighting/skybox settings apply instead of MainMenu's
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSelect()
    {
        // Show buttons
        TutorialButton.SetActive(true);
        Level1Button.SetActive(true);
        Level2Button.SetActive(true);
        GoBackButton.SetActive(true);
    }

    public void CloseSelect()
    {
        // Hide buttons
        TutorialButton.SetActive(false);
        Level1Button.SetActive(false);
        Level2Button.SetActive(false);
        GoBackButton.SetActive(false);
    }
}