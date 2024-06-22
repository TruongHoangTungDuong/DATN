using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseCharacter : MonoBehaviour
{
    public void ChooseCharacterID(int id)
    {
        PlayerPrefs.SetInt("CharacterID", id);
    }
    public void SkipToChooseLevelScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
