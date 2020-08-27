using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHitbox : MonoBehaviour
{
    public PlayerMovement player;
    public PunTeams.Team teamOn;
    void OnTriggerEnter(Collider coll)
    {
        Dodgeball d = coll.GetComponent<Dodgeball>();
        if (d != null)
        {
            if (d.teamOn != teamOn && d.isActive)
            {
                player.CatchBall(d.playerID, d.GetComponent<PhotonView>().viewID);
                
            }
        }
    }
}
