using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastLevelUiManager : MonoBehaviour
{
    public GameObject winPanel;
    public void Quit()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
    }
    public void ShowWinPanel()
    {

    }
}
