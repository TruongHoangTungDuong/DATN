using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnload : MonoBehaviour
{
    public string sceneName;
    void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sceneName = SceneManager.GetActiveScene().name;
        Debug.Log(sceneName);
        if (sceneName == "Main Menu" || sceneName == "Choose Level")
        {
            Debug.Log(1);
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
