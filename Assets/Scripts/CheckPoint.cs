using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CheckPoint : MonoBehaviour
{
    public int nextScene;
    public TimeCountdown timeCountdown;
    public GameObject[] player;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].activeInHierarchy)
                {
                    player[i].GetComponent<Player>().Point += 500;
                    if (player[i].GetComponent<Player>().Point > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name))
                    {
                        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, player[i].GetComponent<Player>().Point);
                        PlayerPrefs.Save();
                    }
                }
            }
            UnLockNewLevel();
            SceneManager.LoadSceneAsync(nextScene);
            timeCountdown.currentTime = 200f;
        }
    }
    void UnLockNewLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex+1);
            PlayerPrefs.SetInt("UnLockedLevel", PlayerPrefs.GetInt("UnLockedLevel",1)+1);
            PlayerPrefs.Save();
        }
    }
}
