using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool isSpawned = false;
    public GameObject NPCList;
    public Distributionmanager distributionmanager;

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
            // int rnadnumer = 0;
            GameObject npc = Instantiate(NPCList.transform.GetChild(0).gameObject, new Vector3 (0, -4), Quaternion.identity);
            NPCScript npcScript = npc.GetComponent<NPCScript>();
            npcScript.WalkUp = true;
            distributionmanager.UpdateNPC(npc);
        }   
    }


    void Despawn()
    {
        isSpawned = false;
    }
}
