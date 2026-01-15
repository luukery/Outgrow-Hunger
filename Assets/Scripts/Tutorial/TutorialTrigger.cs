using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public DialogLines dialog;
    public DialogManager dialogManager;

    public string tutorialID;

    void Start()
    {
        PlayerPrefs.DeleteAll();

        if (!PlayerPrefs.HasKey(tutorialID))
        {
            TriggerTutorial();
        }
    }

    void TriggerTutorial()
    {
        dialogManager.StartDialog(dialog.lines);
        PlayerPrefs.SetInt(tutorialID, 1);
        PlayerPrefs.Save();
    }
}
