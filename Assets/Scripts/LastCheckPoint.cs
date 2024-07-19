using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastCheckPoint : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject[] player;
    public GameObject levelGen;
    public MapSettings mapSetting;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].activeInHierarchy)
                {
                    player[i].GetComponent<Player>().Point += Mathf.RoundToInt(levelGen.GetComponent<LevelGenerator>().width
                        *levelGen.GetComponent<LevelGenerator>().height* mapSetting.modifier);
                    if (player[i].GetComponent<Player>().Point > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name))
                    {
                        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, player[i].GetComponent<Player>().Point);
                        PlayerPrefs.Save();
                    }
                }
            }
           winPanel.SetActive(true);
           Time.timeScale = 0;

        }
    }
}
