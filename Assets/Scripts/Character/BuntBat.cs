using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuntBat : MonoBehaviour
{
    public PhotonView player;
    public PunTeams.Team team;
    public SoundManager sounds;
    Vector3 up = new Vector3(0, 1.7f, 0);
    bool hasHit;
    public void OnTriggerEnter(Collider coll)
    {
        Dodgeball d = coll.GetComponent<Dodgeball>();     
        if (d != null)
        {
            coll.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, team, player.viewID);
            coll.GetComponent<PhotonView>().RPC("RPC_SetVelocity", PhotonTargets.All, (player.transform.forward.normalized * 25 + up) * 1.3f);
            coll.GetComponent<PhotonView>().RPC("RPC_Bunt", PhotonTargets.All);
            hasHit = true;
            sounds.clips[1].Play();
        }       
    }
}
