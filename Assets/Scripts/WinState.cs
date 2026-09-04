using UnityEngine;

public class WinState : MonoBehaviour
{
    [SerializeField] Canvas winScreen;
    [SerializeField] Canvas timer;
    
    [SerializeField] LevelLoader levelLoader;

    void Start()
    {
        winScreen.enabled = false;
    }

    void OnTriggerEnter(Collider enteredCollider)
    {
        GameObject enteredObject = enteredCollider.gameObject;
        if (enteredObject.TryGetComponent<PlayerMovement>(out PlayerMovement script))
        {
            if (script.isActive)
            {
                script.isActive = false;
                //winScreen.enabled = true;
                timer.GetComponent<TimerScript>().StopTimer();
                int nextLevelIndex = levelLoader.getNextLevelIndex();
                levelLoader.LoadScene(nextLevelIndex);
                //level completed ui showupe
            }
        }
    }
}
