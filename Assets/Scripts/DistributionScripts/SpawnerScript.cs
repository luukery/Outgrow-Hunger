using UnityEngine;
using System.Collections.Generic;

public class SpawnerScript : MonoBehaviour
{
    public GameObject[] NPCs;

    private int queueSize = 5;
    public int maxNPCs = 8;

    private List<NPC> npcQueue = new();
    private int npcSpawnCount = 0;

    public NPC CurrentNPC => npcQueue.Count > 0 ? npcQueue[0] : null;

    void Start()
    {
        FillQueue();
    }

    private void FillQueue()
    {
        for (int i = 0; i < queueSize && npcSpawnCount < maxNPCs; i++)
        {
            SpawnAtSlot(i);
        }
    }

    private void SpawnAtSlot(int slotIndex)
    {
        int index = Random.Range(0, NPCs.Length);
        Vector3 pos = GetSlotPosition(slotIndex);

        GameObject npcObj = Instantiate(NPCs[index], pos, Quaternion.identity);
        NPC npc = npcObj.GetComponent<NPC>();

        npcQueue.Add(npc);
        npcSpawnCount++;
    }

    private Vector3 GetSlotPosition(int index)
    {
        Vector3 pos = new Vector3(0, 2);
        return pos + Vector3.down * 1.3f * index;
    }

    public bool Despawn()
    {
        if (npcQueue.Count == 0)
            return false;

        // Remove front NPC
        NPC removed = npcQueue[0];
        npcQueue.RemoveAt(0);
        Destroy(removed.gameObject);

        // Move remaining NPCs up
        for (int i = 0; i < npcQueue.Count; i++)
        {
            npcQueue[i].transform.position = GetSlotPosition(i);
        }

        // Spawn new NPC at back if allowed
        if (npcSpawnCount < maxNPCs)
        {
            SpawnAtSlot(queueSize - 1);
        }

        return npcQueue.Count > 0;
    }
}
