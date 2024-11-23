using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellTutorial : MonoBehaviour
{
    private HashSet<string> singleSpellsCast = new HashSet<string>();
    private bool comboSpellCast = false;

    private Attack attackScript;
    private int previousSpellCount = 0;
    private List<string> lastCastSpells = new List<string>();

    // Dialogue UI component
    [SerializeField] private TMP_Text dialogueText;
    private bool hasShownComboHint = false;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool dialogueComplete = false;
    private bool tutorialDone = false;

    public GameObject spawner;

    private Spawner spawnScript;

    public AudioSource textSound;

    void Start()
    {
        if (!tutorialDone)
        {
            // Get the Attack script attached to the same GameObject
            attackScript = GetComponent<Attack>();

            spawnScript = spawner.GetComponent<Spawner>();

            if (attackScript == null)
            {
                Debug.LogError("No Attack script found on the player.");
            }

            // Initial dialogue on scene open
            ShowDialogue("I've finally found it...\n\nThe legendary ruins of Spelltrace!\n\nAll that's left to do now is click and drag over the shape of the ruins and then right-click to confirm for each of the three shapes... (Space to remove text)");
        }
        else
        {

        }
    }

    void Update()
    {
        // Check if a spell was just cast
        DetectSpellCasts();

        // Handle space bar input to either show full dialogue or hide it
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Finish typing and show the full message instantly
                CompleteDialogue();
            }
            else if (dialogueComplete)
            {
                // Hide the dialogue
                HideDialogue();
            }
        }

        if (tutorialDone)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndTutorial();
            }
        }
    }

    private void DetectSpellCasts()
    {
        if (attackScript == null)
            return;

        int currentSpellCount = attackScript.spellBuffer.Count;

        if (previousSpellCount > 0 && currentSpellCount == 0)
        {
            ProcessSpellCast(lastCastSpells);

            if (TutorialConditionsMet())
            {
                ShowDialogue("The combinations are endless...\n\nI have unlimited power! (Press space to continue)");
                tutorialDone = true;
            }
        }

        if (currentSpellCount > 0)
        {
            lastCastSpells = new List<string>(attackScript.spellBuffer);
        }

        previousSpellCount = currentSpellCount;
    }

    private void ProcessSpellCast(List<string> spellsCast)
    {
        if (spellsCast.Count == 1)
        {
            string spellType = spellsCast[0];
            if (!singleSpellsCast.Contains(spellType))
            {
                singleSpellsCast.Add(spellType);
                Debug.Log($"Single spell cast: {spellType}");

                if (singleSpellsCast.Count == 3 && !hasShownComboHint)
                {
                    ShowDialogue("Maybe I should try and combo 2 different spells together...");
                    hasShownComboHint = true;
                }
            }
        }
        else if (spellsCast.Count == 2)
        {
            if (!comboSpellCast)
            {
                comboSpellCast = true;
                Debug.Log("Combo spell cast.");
            }
        }
    }

    private bool TutorialConditionsMet()
    {
        return singleSpellsCast.Contains("Fire") &&
               singleSpellsCast.Contains("Ice") &&
               singleSpellsCast.Contains("Lightning") &&
               comboSpellCast;
    }

    private void ShowDialogue(string message)
    {
        if (dialogueText != null)
        {
            // Start the typing effect coroutine
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(message));
        }
        else
        {
            Debug.LogWarning("Dialogue Text UI element is not assigned.");
        }
    }

    private IEnumerator TypeText(string message)
    {
        dialogueText.text = "";
        isTyping = true;
        textSound.Play();
        dialogueComplete = false;

        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f); // Delay for typing effect
        }

        isTyping = false;
        dialogueComplete = true;
    }

    private void CompleteDialogue()
    {
        // Instantly complete the text display
        textSound.Stop();
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = dialogueText.text; // Ensure the full text is displayed
        isTyping = false;
        dialogueComplete = true;
    }

    private void HideDialogue()
    {
        dialogueText.text = "";
        dialogueComplete = false;
    }

    public void EndTutorial()
    {
        if (dialogueText != null)
        {
            HideDialogue();
        }
        tutorialDone = true;
        spawnScript = spawner.GetComponent<Spawner>();
        spawnScript.enabled = true;
        enabled = false;
    }
}
