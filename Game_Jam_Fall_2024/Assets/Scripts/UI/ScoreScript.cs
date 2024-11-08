using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text scoreText;
    private int score;

    public GameObject scorePopUpPrefab;
    private List<GameObject> activePopups = new List<GameObject>();
    public Canvas uiCanvas;
    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TMPro.TMP_Text>();
        score = 0;
        scoreText.text = score.ToString("#,##0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString("#,##0");
        //ShowScorePopup(amount);
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString("#,##0");
    }

    public string GetScore()
    {
        return scoreText.text;
    }
    /*
    private void ShowScorePopup(int amount)
    {
        if (scorePopUpPrefab == null)
        {
            Debug.LogWarning("Score Popup Prefab is not assigned.");
            return;
        }

        // Instantiate the score popup prefab at the score's position
        GameObject scorePopUp = Instantiate(scorePopUpPrefab, transform.position, Quaternion.identity);
        activePopups.Add(scorePopUp); // Track the instantiated popup

        // Get the TMP_Text component from the instantiated popup
        TMP_Text textMesh = scorePopUp.GetComponentInChildren<TMP_Text>();

        if (textMesh != null)
        {
            textMesh.text = amount.ToString();
        }
        else
        {
            Debug.LogWarning("No TMP_Text component found in the damage popup prefab.");
        }

        // Optionally, if you want the popup to appear on the UI canvas:
        /*
        if (uiCanvas != null)
        {
            scorePopUp.transform.SetParent(uiCanvas.transform, false);

            // Set the position of the popup in screen space, since it's on a canvas
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            scorePopUp.transform.position = screenPosition;
        }

        // Start coroutine to animate and destroy the popup after a delay
        StartCoroutine(AnimateAndDestroyPopup(scorePopUp, 1.0f));
    }

    private IEnumerator AnimateAndDestroyPopup(GameObject popup, float seconds)
    {
        TMP_Text textMesh = popup.GetComponentInChildren<TMP_Text>();
        if (textMesh == null)
        {
            yield break;
        }

        Vector3 originalPosition = textMesh.transform.position;

        float elapsed = 0f;
        while (elapsed < seconds)
        {
            float t = elapsed / seconds;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(popup);

        // Remove from the list of active popups
        activePopups.Remove(popup);
    }
    */
}
