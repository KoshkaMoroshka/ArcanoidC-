using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameDataScript gameData;
    public void StartGame() 
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ResumeGame()
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        gameData.Reset();
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }

    public void ExitGame()
    { 
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
