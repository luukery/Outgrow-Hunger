using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public bool WalkUp = false;
    public bool Talking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (WalkUp)
        {
            transform.position += new Vector3(0, 0.01f);
            if (transform.position == new Vector3(0, 1))
            {
                Talking = true;
                WalkUp = false;
            }
        }
    }


    void CloseDialog()
    {
        Talking = false;
        transform.position += new Vector3(0, -0.01f);
        if (transform.position == new Vector3(0, -1))
        {
            Destroy(gameObject);
            // somehow tell spawner script to spawn more
        }
    }
}
