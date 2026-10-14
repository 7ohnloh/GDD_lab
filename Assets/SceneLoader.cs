using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;

    // hook this up to a button's OnClick
    public void LoadScene()
    {
        // leaving from pause or game over would otherwise keep the next scene frozen
        Time.timeScale = 1.0f;
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }
}
