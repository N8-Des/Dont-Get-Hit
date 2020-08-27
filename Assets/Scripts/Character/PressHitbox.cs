using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressHitbox : MonoBehaviour
{
    public Dodgeball dodgeball;
    Vector3 position;
    public void OnTriggerStay(Collider coll)
    {
        if (dodgeball.GetComponent<PhotonView>().isMine)
        {
            if (coll.tag == "Character")
            {
                if (coll.GetComponent<PhotonView>().owner.GetTeam() != dodgeball.teamOn && !coll.GetComponent<PlayerMovement>().beingPulled)
                {
                    position = transform.position;
                    if (transform.position.y < 1)
                    {
                        position.y = 1;
                    }
                    coll.GetComponent<PhotonView>().RPC("RPC_GetPulled", PhotonTargets.All, transform.position);
                }
            }
        }
    }
}
