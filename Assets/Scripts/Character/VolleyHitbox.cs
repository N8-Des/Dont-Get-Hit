using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolleyHitbox : PlayerHitbox
{
    public override void OnTriggerEnter(Collider coll)
    {
        if (view.isMine)
        {
            if (coll.tag == "Character")
            {
                PhotonView hit = coll.GetComponent<PhotonView>();
                if (hit.owner.GetTeam() != teamOn)
                {
                    hit.RPC("RPC_TakeHit", PhotonTargets.All, playerHit);
                    hit.RPC("RPC_TakeHit", PhotonTargets.All, playerHit);
                }

            }
        }
    }
}
