using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject NPCList;
    // deze is temp, het moet eventueel gewoon een random npc pakken van het npc lijst
    public GameObject NPC;

    public GameObject SpawnNPC()
    {
        GameObject npc = Instantiate(NPC, new Vector3(0, 1), Quaternion.identity);
        return npc;
    }

}
