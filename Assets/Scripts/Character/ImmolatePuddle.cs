using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmolatePuddle : MonoBehaviour
{
    PunTeams.Team teamOn;
    List<PlayerMovement> playersIn = new List<PlayerMovement>();
    public PhotonView view;
    int playerID;
    PlayerMovement playerThrew;
    void OnTriggerEnter(Collider coll)
    {
        if (view.isMine)
        {
            PlayerMovement p = coll.GetComponent<PlayerMovement>();
            if (p != null)
            {
                if (!playersIn.Contains(p))
                {
                    playersIn.Add(p);
                    StartCoroutine(damagePlayer(p));
                }
            }
        }
    }
    void OnTriggerExit(Collider coll)
    {
        if (view.isMine)
        {
            PlayerMovement p = coll.GetComponent<PlayerMovement>();
            if (p != null)
            {
                if (playersIn.Contains(p))
                {
                    playersIn.Remove(p);
                }
            }
        }
    }
    IEnumerator damagePlayer(PlayerMovement p)
    {
        yield return new WaitForSeconds(1.5f);
        if (playersIn.Contains(p))
        {
            p.GetComponent<PhotonView>().RPC("RPC_TakeHit", PhotonTargets.All, playerID);
        }
    }
    [PunRPC]
    void RPC_SetTeam(PunTeams.Team team, int playerAttacking)
    {
        teamOn = team;
        playerThrew = PhotonView.Find(playerAttacking).GetComponent<PlayerMovement>();
        playerID = playerAttacking;
    }
}
