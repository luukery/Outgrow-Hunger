using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject NPCList;
    // deze is temp, het moet eventueel gewoon een random npc pakken van het npc lijst
    public NPC spawnedNPC;

    public NPC SpawnNPC()
    {
        NPC npc = Instantiate(spawnedNPC, new Vector3(0, 1), Quaternion.identity);
        return npc;
    }

}
