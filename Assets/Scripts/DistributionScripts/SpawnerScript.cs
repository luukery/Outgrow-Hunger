using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public NPCSprite[] npcSprites;


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


        NPCSpriteController sprite = npcObj.GetComponent<NPCSpriteController>();
        if (sprite != null)
        {
            int v = Random.Range(0, npcSprites.Length);
            sprite.ApplySprite(npcSprites[v]);
        }

        npcQueue.Add(npc);
        npcSpawnCount++;
    }

    private Vector3 GetSlotPosition(int index)
    {
        Vector3 pos = new Vector3(0, 2);
        return pos + Vector3.down * 1.3f * index;
    }

    public void Despawn()
    {
        StartCoroutine(AdvanceQueue());
    }

    private IEnumerator AdvanceQueue()
    {
        if (npcQueue.Count == 0)
            yield break;

        NPC removed = npcQueue[0];
        npcQueue.RemoveAt(0);
        Destroy(removed.gameObject);

        List<IEnumerator> moves = new();

        for (int i = 0; i < npcQueue.Count; i++)
        {
            Vector3 target = GetSlotPosition(i);
            NPCMovement movement = npcQueue[i].GetComponent<NPCMovement>();

            if (movement != null)
                moves.Add(movement.MoveTo(target));
            else
                npcQueue[i].transform.position = target;
        }

        if (npcSpawnCount < maxNPCs)
            SpawnAtSlot(queueSize - 1);

        foreach (var move in moves)
            StartCoroutine(move);

        bool moving;
        do
        {
            moving = false;
            for (int i = 0; i < npcQueue.Count; i++)
            {
                if (Vector3.Distance(
                    npcQueue[i].transform.position,
                    GetSlotPosition(i)) > 0.01f)
                {
                    moving = true;
                    break;
                }
            }
            yield return null;
        }
        while (moving);

        // invoke for hiding UI, needed later
        // OnQueueAdvanceFinished?.Invoke();
    }
}
