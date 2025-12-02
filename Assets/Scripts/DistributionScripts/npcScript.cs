using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCScript : MonoBehaviour
{
    public bool WalkUp = false;
    public bool Talking = false;

    public Canvas canvas;
    private Button feedButton, denyButton;
    private SpriteRenderer box;
    private TextMeshProUGUI dialogue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        box = canvas.transform.Find("DialogueBox").GetComponent<SpriteRenderer>();
        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        HideAndShowDialogue(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (WalkUp)
        {
            transform.position += new Vector3(0, 0.01f);
            if (transform.position == new Vector3(0, 1))
            {
                Talking = true;
                WalkUp = false;
                HideAndShowDialogue(true);
            }
        }
    }


    void CloseDialog()
    {
        Talking = false;
        HideAndShowDialogue(false);
        transform.position += new Vector3(0, -0.01f);
        if (transform.position == new Vector3(0, -1))
        {
            Destroy(gameObject);
            // somehow tell spawner script to spawn more
        }
    }

    void HideAndShowDialogue(bool show)
    {
        feedButton.gameObject.SetActive(show);
        denyButton.gameObject.SetActive(show);
        dialogue.gameObject.SetActive(show);
        box.gameObject.SetActive(show);
    }
}
