using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        PlayerMovement p = coll.GetComponent<PlayerMovement>();
        if (p != null)
        {
            p.view.RPC("RPC_ForceDie", PhotonTargets.All);
        }
    }
}
