#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private List<DialogueConversation> conversations = new List<DialogueConversation>();
    [HideInInspector] public List<DialogueConversation> Conversations => conversations;

    void Awake()
    {
        foreach (DialogueConversation convo in conversations)
            convo.LoadFromExcel();
    }

    public DialogueConversation GetConversationByID(int id)
    {
        List<DialogueConversation> group = conversations.Where(c => c.conversationId == id).ToList();
        DialogueConversation next = group.FirstOrDefault(c => !c.hasBeenPlayed);

        if (next != null)
            next.hasBeenPlayed = true;
        else
            next = group.FirstOrDefault(c => c.isDefaultDialogue);

        return next;
    }

    public void ResetAllConversations()
    {
        foreach (DialogueConversation convo in conversations)
            convo.hasBeenPlayed = false;
    }
}
