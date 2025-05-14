using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Paused()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }

    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1; // Untuk memastikan waktu berjalan normal saat pindah scene
        SceneManager.LoadScene(sceneName);
    }
}
