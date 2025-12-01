using UnityEngine;

public class Distributionmanager : MonoBehaviour
{
    public GameObject currentNPC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        if (currentNPC != null)
        {
            
        }
    }

    public void UpdateNPC(GameObject npc)
    {
        currentNPC = npc;
    }
}
