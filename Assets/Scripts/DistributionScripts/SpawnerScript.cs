using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject NPCList;
    public Distributionmanager distributionmanager;
    public GameObject NPC;

    void Start()
    {
        GameObject npc = Instantiate(NPC, new Vector3(0, -4), Quaternion.identity);
        distributionmanager.UpdateNPC(npc);
    }

    void Update()
    {
        
    }

}
