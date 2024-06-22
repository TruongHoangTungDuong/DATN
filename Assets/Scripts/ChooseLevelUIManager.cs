using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseLevelUIManager : MonoBehaviour
{
    public TimeCountdown timeCountdown;

    public void LoadLevel(int scene)
    {
        SceneManager.LoadSceneAsync(scene);
        //timeCountdown.currentTime = 200f;
    }
    public void QuitToChooseCharacterScene()
    {
        SceneManager.LoadSceneAsync(12);
    }
}
