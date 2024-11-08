using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text timerText;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<TMPro.TMP_Text>();
        timerText.text = string.Format("");
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        updateTimer(timer);
    }

    public void updateTimer(float currentTime)
    {
        currentTime += 1;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public string GetTimer()
    {
        return timerText.text;
    }
}
