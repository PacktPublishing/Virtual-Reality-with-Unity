using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawner : MonoBehaviour
{
    public List<GameObject> toSpawn;

    private bool hasFired = false;

    void OnTriggerEnter(Collider collider)
    {
        if(!hasFired)
        {
            if (collider.GetComponent<VRTK.VRTK_HeadsetCollider>() != null)
            {
                foreach (var obj in toSpawn)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }

                }
            }

            hasFired = true;
        }
        
        
    }
}
