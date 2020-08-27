using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuntDodgeball : Dodgeball
{
    public override void OnCollisionEnter(Collision coll)
    {
        if (view.isMine)
        {
            if (coll.gameObject.tag == "Ground" || coll.gameObject.tag == "Wall")
            {
                view.RPC("RPC_KillMe", PhotonTargets.All);
            }
            if (coll.gameObject.tag == "Character" && isActive)
            {
                PhotonView hitView = coll.gameObject.GetComponent<PhotonView>();
                if (hitView.owner.GetTeam() != teamOn)
                {
                    hitView.RPC("RPC_TakeHit", PhotonTargets.All, playerID);
                    view.RPC("RPC_KillMe", PhotonTargets.All);
                }
            }
        }
    }
    [PunRPC]
    void RPC_KillMe()
    {
        gameObject.SetActive(false);
    }
    protected override void Update()
    {
        if (!view.isMine)
        {
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(positionAtLastPacket, realPosition, (float)(currentTime / timeToReachGoal));
            transform.rotation = Quaternion.Lerp(rotationAtLastPacket, realRotation, (float)(currentTime / timeToReachGoal));
        }
    }
}
