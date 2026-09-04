using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator crossfadeAnimator;

    public float numberOfSeconds;

    public void LoadScene(int SceneId)
    {
        StartCoroutine(LoadLevel(SceneId));
    }

    IEnumerator LoadLevel(int SceneId)
    {
        crossfadeAnimator.SetTrigger("NextScene");

        yield return new WaitForSeconds(numberOfSeconds);
        
        SceneManager.LoadScene(SceneId);
    }

    public int getThisLevelIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public int getNextLevelIndex()
    {
        return getThisLevelIndex() + 1;
    }

}
