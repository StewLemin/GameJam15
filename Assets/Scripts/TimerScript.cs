using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    
    [SerializeField] Canvas loseScreen;

    [SerializeField] float initialTime = 20f;

    private float currentTime;

    private float minTime = 0;
    
    private bool active = true;
    
    public LevelLoader levelLoader;

    void Start()
    {
        currentTime = initialTime;
        loseScreen.enabled = false;
    }

    public void StopTimer()
    {
        active = false;
    }

    public float getElapsedTime()
    {
        return initialTime - currentTime;
    }



    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            return;
        }

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0;
            int currentLevelIndex = levelLoader.getThisLevelIndex();
            levelLoader.LoadScene(currentLevelIndex);
            //loseScreen.enabled = true;
        }
        
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        int milisec = Mathf.FloorToInt(currentTime * 100 % 100);
        
        string final = string.Format("{0}:{1:00}:{2:00}",minutes,seconds,milisec);
        timerText.text = final;
    }
    
        

    
}
