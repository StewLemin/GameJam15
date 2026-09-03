using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject TutorialButton;
    public GameObject Level1Button;
    public GameObject Level2Button;
    public GameObject GoBackButton;

    private List<AudioListener> disabledListeners = new List<AudioListener>();
    private List<EventSystem> disabledEventSystems = new List<EventSystem>();

    public void Start()
    {
        // Hide the level select menu
        HideLevelSelectMenu();

        // Show cursor again
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause everything
        Time.timeScale = 0f;

        DisableOtherAudioListeners();
        DisableOtherEventSystems();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenu");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f; // Resume time
    }

    public void PlayPrevious()
    {
        ReenableAudioListeners();
        ReenableEventSystems();
        SceneManager.UnloadSceneAsync("MainMenu");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f; // Resume time
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void DisableOtherAudioListeners()
    {
        // FindObjectsByType finds listeners across ALL loaded scenes, active or not
        AudioListener[] allListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        foreach (var listener in allListeners)
        {
            // Skip listeners that belong to this menu's own scene
            if (listener.gameObject.scene != gameObject.scene && listener.enabled)
            {
                listener.enabled = false;
                disabledListeners.Add(listener);
            }
        }
    }

    private void DisableOtherEventSystems()
    {
        EventSystem[] allEventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        foreach (var es in allEventSystems)
        {
            if (es.gameObject.scene != gameObject.scene && es.enabled)
            {
                es.enabled = false;
                disabledEventSystems.Add(es);
            }
        }
    }

    private void ReenableEventSystems()
    {
        foreach (var es in disabledEventSystems)
        {
            if (es != null) es.enabled = true;
        }
        disabledEventSystems.Clear();
    }

    private void ReenableAudioListeners()
    {
        foreach (var listener in disabledListeners)
        {
            if (listener != null) listener.enabled = true;
        }
        disabledListeners.Clear();
    }

    public void ShowLevelSelectMenu()
    {
        // Show the TutorialButton, Level1Button, Level2Button, GoBackButton
        // If this button is pressed, we can unload all scenes except for the MainMenu
        TutorialButton.SetActive(true);
        Level1Button.SetActive(true);
        Level2Button.SetActive(true);
        GoBackButton.SetActive(true);
    }

    public void HideLevelSelectMenu()
    {
        // Hide the TutorialButton, Level1Button, Level2Button, GoBackButton
        TutorialButton.SetActive(false);
        Level1Button.SetActive(false);
        Level2Button.SetActive(false);
        GoBackButton.SetActive(false);
    }
}
