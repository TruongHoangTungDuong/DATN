using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] button;
    private void Awake()
    {
        int unlockedLevel=PlayerPrefs.GetInt("UnLockedLevel",1);
        for(int i = 0; i < button.Length; i++)
        {
            button[i].interactable = false;
        }
        for(int i = 0; i < unlockedLevel; i++)
        {
            button[i].interactable = true;
        }
    }
    /*void Start()
    {
        ResetPlayerPrefs();
    }
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.SetInt("ReachedIndex", 0);
        PlayerPrefs.SetInt("UnLockedLevel", 1);
        PlayerPrefs.Save();
    }*/
    // Update is called once per frame
    void Update()
    {
        
    }
}
