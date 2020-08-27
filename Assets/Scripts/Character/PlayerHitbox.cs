using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public PunTeams.Team teamOn;
    protected int playerHit;
    public PhotonView view;
    public virtual void OnTriggerEnter(Collider coll)
    {
        if (view.isMine)
        {
            if (coll.tag == "Character")
            {
                PhotonView hit = coll.GetComponent<PhotonView>();
                if (hit.owner.GetTeam() != teamOn)
                {
                    hit.RPC("RPC_TakeHit", PhotonTargets.All, playerHit);
                }

            }
        }
    }
    [PunRPC]
    public void RPC_SetTeam(PunTeams.Team team, int playerAttack)
    {
        teamOn = team;
        playerHit = playerAttack;
    }
}
