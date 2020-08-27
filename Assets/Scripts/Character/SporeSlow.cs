using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeSlow : MonoBehaviour
{
    List<PlayerMovement> playersInRadius = new List<PlayerMovement>();
    PunTeams.Team teamOn;
    [SerializeField]
    PhotonView view;
    int playerID;
    public float change;
    public bool friendly;
    void Start()
    {
        StartCoroutine(kill());
    }
    void OnTriggerEnter(Collider coll)
    {
        PlayerMovement p = coll.GetComponent<PlayerMovement>();
        if (coll.tag == "Character")
        {
            if (friendly)
            {
                if (p.teamOn == teamOn && !playersInRadius.Contains(p) && view.isMine)
                {
                    p.view.RPC("RPC_SpeedChange", PhotonTargets.All, change);
                    playersInRadius.Add(p);
                }
            }
            else
            {
                if (p.teamOn != teamOn && !playersInRadius.Contains(p) && view.isMine)
                {
                    p.view.RPC("RPC_SpeedChange", PhotonTargets.All, change);
                    playersInRadius.Add(p);
                }
            }
        }
    }
    void OnTriggerExit(Collider coll)
    {
        PlayerMovement p = coll.GetComponent<PlayerMovement>();
        if (coll.tag == "Character")
        {
            if (friendly)
            {
                if (p.teamOn == teamOn && playersInRadius.Contains(p) && view.isMine)
                {
                    p.view.RPC("RPC_SpeedChange", PhotonTargets.All, -change);
                    playersInRadius.Remove(p);
                }
            }
            else
            {
                if (p.teamOn != teamOn && playersInRadius.Contains(p) && view.isMine)
                {
                    p.view.RPC("RPC_SpeedChange", PhotonTargets.All, -change);
                    playersInRadius.Remove(p);
                }
            }

        }
    }

    IEnumerator kill()
    {
        yield return new WaitForSeconds(8);
        foreach (PlayerMovement p in playersInRadius)
        {
            p.view.RPC("RPC_SpeedChange", PhotonTargets.All, -change);
        }
        playersInRadius.Clear();
        gameObject.SetActive(false);
    }
    [PunRPC]
    void RPC_SetTeam(PunTeams.Team team, int playerAttacking)
    {
        teamOn = team;
        playerID = playerAttacking;
    }
}
