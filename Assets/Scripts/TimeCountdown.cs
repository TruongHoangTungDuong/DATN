using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeCountdown : MonoBehaviour
{
    public int time = 200;
    public float currentTime;
    IEnumerator Countdown()
    {
        currentTime = time;
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
            UIManager.Instance.SetTimeCountdown(Convert.ToString(currentTime));
        }
        UIManager.Instance.ShowGameOverPanel();
        Time.timeScale = 0;
    }
    void Start()
    {
        time = 200;
        StartCoroutine(Countdown());
    }
    void Update()
    {

    }
 }
