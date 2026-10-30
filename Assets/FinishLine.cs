using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Flagpole at the end of a level: when Mario touches it, play the flagpole sound and load the next scene.
public class FinishLine : MonoBehaviour
{
    public string nextScene = "World-1-2";
    public float delay = 1.5f;     // seconds to let the sound play before loading
    private bool reached = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (reached || !other.CompareTag("Player")) return;
        reached = true;
        AudioSource sound = GetComponent<AudioSource>();
        if (sound != null) sound.Play();
        StartCoroutine(LoadNext());
    }

    IEnumerator LoadNext()
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1.0f;
        PlayerPrefs.Save();
        // GameScore is a ScriptableObject, so the score survives the scene load
        SceneManager.LoadScene(nextScene);
    }

}
