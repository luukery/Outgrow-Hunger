using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogBox;
    public TMP_Text dialogText;

    private string[] lines;
    private int index;

    void Update()
    {
        if (dialogBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

    public void StartDialog(string[] dialogLines)
    {
        lines = dialogLines;
        index = 0;
        dialogBox.SetActive(true);
        dialogText.text = lines[index];
    }

    void NextLine()
    {
        index++;
        if (index < lines.Length)
        {
            dialogText.text = lines[index];
        }
        else
        {
            dialogBox.SetActive(false);
        }
    }
}
