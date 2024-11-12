using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public Button RestartButton;
    //[SerializeField] private GameObject score;
    //[SerializeField] private GameObject timer;
    //private StatsManagerScript stats;
    // Start is called before the first frame update
    void Start()
    {
        RestartButton.onClick.AddListener(RestartGame);
        /*
        stats = StatsManagerScript.Instance;
        if (stats == null)
        {
            Debug.Log("stats not found");
        }
        if (stats.GetScoreText() == null)
        {
            Debug.Log("score is null");
        }
        score.GetComponent<TMPro.TMP_Text>().text = "Score: " + stats.GetScoreText();
        timer.GetComponent<TMPro.TMP_Text>().text = "Time: " + stats.GetTimerText();
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RestartGame()
    {
        SceneManager.LoadScene("AiTesting");
    }
}
