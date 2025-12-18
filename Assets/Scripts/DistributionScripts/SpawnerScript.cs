using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    
    /* code for a single npc
    public GameObject NPC;

    public NPC SpawnNPC()
    {
        GameObject npc = Instantiate(NPC);
        return npc.GetComponent<NPC>();
    }
    */

    public GameObject[] NPCs;
    public NPC SpawnNPC()
    {
        int index = Random.Range(0, NPCs.Length);
        GameObject npc = Instantiate(NPCs[index], new Vector3(0, 1), Quaternion.identity);
        return npc.GetComponent<NPC>();
    }

}
