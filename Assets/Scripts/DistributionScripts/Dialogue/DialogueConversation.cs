using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueConversation
{
    public string conversationName;
    public int conversationId;

    public TextAsset csvFile;

    [HideInInspector] public List<DialogueLine> dialogue = new List<DialogueLine>();
    public bool hasBeenPlayed;
    public bool isDefaultDialogue;
    public string eventName;

    public void LoadFromExcel()
    {
        DialogueExcelReader reader = new DialogueExcelReader();
        if (csvFile == null)
        {
            Debug.LogWarning($"Geen CSV-bestand gekoppeld aan '{conversationName}'");
            return;
        }

        dialogue = reader.ReadCSV(csvFile);
    }

    public void TriggerEvent()
    {
        if (!string.IsNullOrWhiteSpace(eventName))
        {
          //  EventRegistry.Invoke(eventName);
        }
    }
}
