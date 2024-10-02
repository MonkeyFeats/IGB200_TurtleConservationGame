using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollingDialogue : MonoBehaviour
{
    private TextMeshProUGUI dialogueText; // The TMP component for displaying dialogue
    public bool playsOnStart = true;
    public float scrollSpeed = 0.05f;    // Time between characters appearing
    private bool isScrolling = false;    // Check if scrolling is ongoing
    private Coroutine scrollingCoroutine;
    private string fullText;             // The text that will be revealed

    void Start()
    {
        // Get the existing text from the TextMeshProUGUI component
        dialogueText = GetComponent<TextMeshProUGUI>();
        fullText = dialogueText.text;

        if (playsOnStart)
            StartScrolling();
    }

    public void StartScrolling()
    {
        if (scrollingCoroutine != null)
        {
            StopCoroutine(scrollingCoroutine);
        }
        scrollingCoroutine = StartCoroutine(ScrollText());
    }

    public void SkipToFullText()
    {
        if (isScrolling)
        {
            StopCoroutine(scrollingCoroutine);
            dialogueText.text = fullText;
            isScrolling = false;
        }
    }

    private IEnumerator ScrollText()
    {
        isScrolling = true;
        dialogueText.text = ""; // Clear the text

        foreach (char letter in fullText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(scrollSpeed);
        }

        isScrolling = false;
    }

    void Update()
    {
        // Example input to skip to full text
        if (Input.GetKeyDown(KeyCode.Space) && isScrolling)
        {
            SkipToFullText();
        }
    }
}
