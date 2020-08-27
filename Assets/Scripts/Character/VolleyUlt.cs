using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolleyUlt : MonoBehaviour
{
    bool isSpiked;
    PunTeams.Team teamOn;
    [SerializeField]
    PhotonView view;
    PrefabPool pool;
    int player;
    void Start()
    {
        pool = FindObjectOfType<PrefabPool>();
    }
    [PunRPC]
    public void RPC_StartSpike(Vector3 location, PunTeams.Team team, int play)
    {
        teamOn = team;
        player = play;
        isSpiked = true;
        StartCoroutine(move(location));
    }
    IEnumerator move(Vector3 loc)
    {
        while (isSpiked)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.MoveTowards(transform.position, loc, Time.deltaTime * 30);
        }
    }
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Ground" || coll.gameObject.tag == "Character")
        {
            isSpiked = false;
            if (view.isMine)
            {
                view.RPC("RPC_Die", PhotonTargets.All);
                GameObject plode = pool.spawnObject(transform.position, Quaternion.identity, 16);
                plode.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, teamOn, player);
            }
        }
    }
    [PunRPC]
    void RPC_Die()
    {
        gameObject.SetActive(false);
    }
}
