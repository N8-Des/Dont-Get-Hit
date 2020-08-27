using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    public PhotonView view;
    public Animator anim;
    void Start()
    {
        if (view.isMine)
        {
            StartCoroutine(LoopAround());
        }
    }

    IEnumerator LoopAround()
    {
        yield return new WaitForSeconds(15);
        int rand = Random.Range(0, 3);
        view.RPC("RPC_SetFactory", PhotonTargets.All, rand);
        StartCoroutine(LoopAround());
    }

    [PunRPC]
    void RPC_SetFactory(int what)
    {
        switch(what)
        {
            case 0:
                anim.SetTrigger("Right");
                break;
            case 1:
                anim.SetTrigger("Left");
                break;
            case 2:
                break;
        }
    }
}
