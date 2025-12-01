using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool isSpawned = false;
    public GameObject NPCList;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawned)
        {
            isSpawned = true;
            // npc needs to be random
            int npc = 0;
            Instantiate(NPCList.transform.GetChild(0));
        }
    }


    void Despawn()
    {
        isSpawned = false;
    }
}
