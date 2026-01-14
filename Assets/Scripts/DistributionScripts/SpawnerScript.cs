using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject[] NPCs;
    public NPC SpawnNPC()
    {
        int index = Random.Range(0, NPCs.Length);
        GameObject npc = Instantiate(NPCs[index], new Vector3(0, 1), Quaternion.identity);
        return npc.GetComponent<NPC>();
    }

}
