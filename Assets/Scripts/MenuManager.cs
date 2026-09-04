using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("UI")]
    public GameObject TutorialButton;
    public GameObject Level1Button;
    public GameObject Level2Button;
    public GameObject GoBackButton;

    public bool IsMenuOpen { get; private set; }
    private string currentLevelScene = null; // null = no level loaded yet

    private List<AudioListener> disabledListeners = new List<AudioListener>();
    private List<EventSystem> disabledEventSystems = new List<EventSystem>();

    void Awake()
    {
        // MainMenu is meant to be a single persistent scene - guard against accidental duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        OpenMenu(); // game boots straight into the menu, nothing else loaded yet
        CloseSelect(); // hide the level select buttons until the player clicks "Play"
    }

    // Wired to TutorialButton / Level1Button / Level2Button
    public void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        // If a level is already running, swap it rather than stacking a second one
        if (currentLevelScene != null && currentLevelScene != sceneName)
        {
            SceneManager.UnloadSceneAsync(currentLevelScene);
        }

        currentLevelScene = sceneName;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        // Make the level scene's own lighting/skybox settings apply instead of MainMenu's
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        CloseMenu();
    }

    // Go back
    public void PlayPrevious()
    {
        if (currentLevelScene == null)
        {
            LoadScene("CedricTest");
            CloseMenu();
            return;
        }
        // nothing running, ignore
        CloseMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Called on boot AND every time TAB is pressed mid-level
    public void OpenMenu()
    {
        if (IsMenuOpen) return; // already open - this is what stops the TAB-spam bug
        IsMenuOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        DisableOtherAudioListeners();
        DisableOtherEventSystems();
    }

    private void CloseMenu()
    {
        IsMenuOpen = false;
        Time.timeScale = 1f;

        SceneManager.UnloadSceneAsync("MainMenu");

        ReenableAudioListeners();
        ReenableEventSystems();
    }


    public void OpenSelect()
    {
        // Can already unload the current level scene if one is running. 
        if (currentLevelScene != null)
        {
            SceneManager.UnloadSceneAsync(currentLevelScene);
            currentLevelScene = null;
        }

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

    private void DisableOtherAudioListeners()
    {
        disabledListeners.Clear();
        foreach (var listener in FindObjectsByType<AudioListener>(FindObjectsSortMode.None))
        {
            if (listener.gameObject.scene != gameObject.scene && listener.enabled)
            {
                listener.enabled = false;
                disabledListeners.Add(listener);
            }
        }
    }

    private void DisableOtherEventSystems()
    {
        disabledEventSystems.Clear();
        foreach (var es in FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
        {
            if (es.gameObject.scene != gameObject.scene && es.enabled)
            {
                es.enabled = false;
                disabledEventSystems.Add(es);
            }
        }
    }

    private void ReenableAudioListeners()
    {
        foreach (var l in disabledListeners) if (l != null) l.enabled = true;
        disabledListeners.Clear();
    }

    private void ReenableEventSystems()
    {
        foreach (var es in disabledEventSystems) if (es != null) es.enabled = true;
        disabledEventSystems.Clear();
    }
}