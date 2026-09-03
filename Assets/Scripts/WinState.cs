using UnityEngine;

public class WinState : MonoBehaviour
{
    [SerializeField] Canvas winScreen;
    [SerializeField] Canvas timer;
    

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
                winScreen.enabled = true;
                timer.GetComponent<TimerScript>().StopTimer();
                //level completed ui showup
            }
        }
    }
}
