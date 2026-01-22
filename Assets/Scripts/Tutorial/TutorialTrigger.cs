using UnityEngine;
using System.Collections.Generic;

public class TutorialTrigger : MonoBehaviour
{
    public DialogLines dialog;
    public DialogManager dialogManager;

    public string tutorialID;

    #if UNITY_EDITOR
    private static HashSet<string> resetThisSession = new HashSet<string>();
    #endif

    void Start()
    {
        #if UNITY_EDITOR
        // For testing in editor: Clear each tutorial flag only once per play session
        if (!resetThisSession.Contains(tutorialID))
        {
            PlayerPrefs.DeleteKey(tutorialID);
            PlayerPrefs.Save();
            resetThisSession.Add(tutorialID);
        }
        #endif
        
        Debug.Log("TutorialTrigger Start - tutorialID: " + tutorialID);
        Debug.Log("PlayerPrefs has key? " + PlayerPrefs.HasKey(tutorialID));
        
        if (!PlayerPrefs.HasKey(tutorialID))
        {
            Debug.Log("Tutorial not shown yet, triggering...");
            TriggerTutorial();
        }
        else
        {
            Debug.Log("Tutorial already shown, skipping...");
            // Turn off the canvas when skipping
            if (dialogManager != null && dialogManager.dialogBox != null)
            {
                dialogManager.dialogBox.SetActive(false);
            }
            if (dialogManager != null && dialogManager.background != null)
            {
                dialogManager.background.SetActive(false);
            }
        }
    }

    void TriggerTutorial()
    {
        Debug.Log("TriggerTutorial called");
        Debug.Log("dialog null? " + (dialog == null));
        Debug.Log("dialogManager null? " + (dialogManager == null));
        
        if (dialog != null && dialogManager != null)
        {
            dialogManager.StartDialog(dialog.lines);
            PlayerPrefs.SetInt(tutorialID, 1);
            PlayerPrefs.Save();
            Debug.Log("Tutorial started!");
        }
        else
        {
            Debug.LogError("dialog or dialogManager is not assigned!");
        }
    }
}
