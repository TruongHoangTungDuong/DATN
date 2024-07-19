using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public Text scoreText;
    public Text coinText;
    public Text timeCountdownText;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    private Vector3 playerStartPosition;
    [SerializeField]private bool isGameOver=false;
    public Text highestScoreText;
    /*public CanvasGroup canvasGroup;
    public float fadeInDuration = 1f;
    public float delayBeforeFadeIn = 0.5f;*/
    public RectTransform pausePanelRect;
    public float middlePosY, topPosY;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("UIManager");
                    instance = singletonObject.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;

    }
    public void SkipToChooseLevelScene()
    {
        SceneManager.LoadSceneAsync(1);
        Time.timeScale = 1f;
    }
    public void LoadLevel(string level)
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = playerStartPosition;
        SceneManager.LoadScene(level);
        AudioManager.Instance.audioBackground.Play();
        isGameOver = false;
    }
    public void SetScoreText(string txt)
    {
        scoreText.text = txt;
    }
    public void SetCoinText(string txt)
    {
        coinText.text = txt;
    }
    public void ShowPausePanel()
    {
        if (isGameOver == false)
        {
            pausePanelRect.DOAnchorPosY(middlePosY, 1).SetUpdate(true);
            //pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public void HidePausePanel()
    {
        pausePanelRect.DOAnchorPosY(topPosY, 1).SetUpdate(true);
        //pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        AudioManager.Instance.audioBackground.Stop();
        Time.timeScale = 0;
        isGameOver = true;
    }
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
    public void SetTimeCountdown(string txt)
    {
        timeCountdownText.text= txt;
    }
    public void UpdateHighestScore(int level, int score)
    {
        highestScoreText.text = "Highest Score: " + score.ToString();
    }
    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        AudioManager.Instance.audioBackground.Play();
        Time.timeScale = 1f;
        isGameOver = false;
    }
    void Start()
    {
        SetScoreText(Convert.ToString(0));
        SetCoinText(Convert.ToString(0));
        playerStartPosition = new Vector3(-2.71f, 0.17f, 0);
        int highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0);

        // Hi?n th? ?i?m cao nh?t trên giao di?n
        highestScoreText.text = "High Score: " + highScore.ToString();
    }

    void Update()
    {
    }
}
