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

    // Start is called before the first frame update
    void Start()
    {
        RestartButton.onClick.AddListener(RestartGame);
        
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
