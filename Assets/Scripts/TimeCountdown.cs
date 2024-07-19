using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeCountdown : MonoBehaviour
{
    public float currentTime;
    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1f);
        currentTime += 1f;
        UIManager.Instance.SetTimeCountdown(Convert.ToString(currentTime));
        StartCoroutine(Countdown());
    }
    void Start()
    {
        currentTime = 0;
        StartCoroutine(Countdown());
    }
    void Update()
    {

    }
 }
