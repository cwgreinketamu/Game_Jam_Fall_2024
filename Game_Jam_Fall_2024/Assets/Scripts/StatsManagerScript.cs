using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class StatsManagerScript : MonoBehaviour
{
    public GameObject scoreObject;
    public GameObject timerObject;
    public string scoreText;
    public string timerText;
    public bool firstRun = true;
    public SpellTutorial tutorialScript;
    public static StatsManagerScript Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "DeathScene")
        {
            GameObject.FindGameObjectWithTag("Score").GetComponent<TMPro.TMP_Text>().text = "Score: " + GetScoreText();
            GameObject.FindGameObjectWithTag("Timer").GetComponent<TMPro.TMP_Text>().text = "Time: " + GetTimerText();
        }
        else if (scene.name == "AiTesting")
        {
            if (firstRun)
            {
                firstRun = false;
            }
            else
            {
                tutorialScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SpellTutorial>();
                tutorialScript.EndTutorial();
            }
            scoreObject = GameObject.FindGameObjectWithTag("Score");
            timerObject = GameObject.FindGameObjectWithTag("Timer");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreObject = GameObject.FindGameObjectWithTag("Score");
        timerObject = GameObject.FindGameObjectWithTag("Timer");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrepareGameOverScreen()
    {
        scoreText = scoreObject.GetComponent<ScoreScript>().GetScore();
        Debug.Log($"scoreText is {scoreText} in stats");
        timerText = timerObject.GetComponent<TimerScript>().GetTimer();
        Debug.Log($"timerText is {timerText} in stats");
    }

    public string GetScoreText()
    {
        return scoreText;
    }

    public string GetTimerText()
    {
        return timerText;
    }


}
