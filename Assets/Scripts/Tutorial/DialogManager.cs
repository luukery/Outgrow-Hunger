using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogBox;
    public GameObject background;
    public TMP_Text dialogText;

    private string[] lines;
    private int index;

    void Awake()
    {
        Debug.Log("DialogManager Awake - dialogBox assigned: " + (dialogBox != null));
    }

    void Update()
    {
        // Check if we're currently displaying dialog (lines is not null and we have content)
        if (lines != null && lines.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextLine();
            }
        }
    }

    public void StartDialog(string[] dialogLines)
    {
        Debug.Log("StartDialog called with " + dialogLines.Length + " lines");
        lines = dialogLines;
        index = 0;
        Debug.Log("DialogBox null? " + (dialogBox == null));
        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
            Debug.Log("DialogBox activated");
        }
        if (background != null) background.SetActive(true);
        if (dialogText != null && lines.Length > 0) dialogText.text = lines[index];
        Debug.Log("Dialog started with " + lines.Length + " lines");
    }

    void NextLine()
    {
        index++;
        if (index < lines.Length)
        {
            if (dialogText != null) dialogText.text = lines[index];
            Debug.Log("Dialog line " + index);
        }
        else
        {
            if (dialogBox != null) dialogBox.SetActive(false);
            if (background != null) background.SetActive(false);
            lines = null; // Clear the lines array so Update stops listening
            Debug.Log("Dialog finished");
        }
    }
}
