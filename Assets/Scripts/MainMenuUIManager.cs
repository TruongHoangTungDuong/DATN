using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public void SkipToChooseCharacterScene()
    {
        SceneManager.LoadSceneAsync(12);

    }
}
