using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlayer : PlayerMovement
{
    public Transform QLeft;
    public Transform QRight;
    public CooldownImage cdQ;
    public CooldownImage cdE;
    bool hasQ;
    bool hasE;
    SpliceBall sd;
    bool splitBallIsActive;
    PhotonView splitBallView;
    Vector3 customMomentum = new Vector3(0, 0.9f, 0);
    public Transform[] ultpositions;
    [PunRPC]
    void RPC_SetBall()
    {
        splitBallIsActive = false;
        canvasPlayer.dodgeballIcons[1].SetActive(false);
    }
    protected override void AbilityUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack && !hasE)
        {
            if (cdQ.cooldownUp && !hasQ && hasDodgeball)
            {
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, true);
                hasQ = true;
                cdQ.setCooldown(30);
                canvasPlayer.dodgeballIcons[0].SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && canAttack)
        {
            if (cdE.cooldownUp && !hasE && hasDodgeball && !hasQ)
            {
                hasE = true;
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 1, true);
                canvasPlayer.dodgeballIcons[1].SetActive(true);
                splitBallIsActive = true;
                cdE.setCooldown(20);
            }else if (splitBallIsActive)
            {
                hasE = false;
                canvasPlayer.dodgeballIcons[1].SetActive(false);
                splitBallView.RPC("RPC_Split", PhotonTargets.All);
                splitBallIsActive = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && canAttack && ultCharged)
        {
            Anim.SetBool("hasBall", false);
            isAttacking = true;
            canMove = false;
            Anim.SetBool("isAttacking", true);
            Anim.SetTrigger("R");
            usedUlt();
        }
    }
    public void endAttack()
    {
        if (view.isMine)
        {
            canMove = true;
            canAttack = true;
            isAttacking = false;
            Anim.SetBool("isAttacking", false);
        }
    }
    public void R()
    {
        if (view.isMine)
        {
            foreach (Transform t in ultpositions)
            {
                sd = pool.spawnObject(t.position, t.rotation, 20).GetComponent<SpliceBall>();
                PhotonView SpliceView = sd.GetComponent<PhotonView>();
                SpliceView.RPC("RPC_SetVelocity", PhotonTargets.All, (t.forward.normalized * 22 + customMomentum) * 1.4f);
                SpliceView.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
            }
        }
    }
    public override void throwBall()
    {
        if (hasQ)
        {
            smanage.playAudioClip(0);
        }
        if (view.isMine)
        {
            canvasPlayer.dodgeballShow.SetActive(false);
            hasDodgeball = false;
            GameObject newBall = pool.spawnObject(dodgeballPosition.position, Quaternion.identity, 2);
            PhotonView ballView = newBall.GetComponent<PhotonView>();
            ballView.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
            ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 22 + customMomentum) * throwMultiplier * 1.4f));
            if (hasQ)
            {
                sd = pool.spawnObject(QLeft.transform.position, QLeft.transform.rotation, 20).GetComponent<SpliceBall>();
                PhotonView SpliceView1 = sd.GetComponent<PhotonView>();
                SpliceView1.RPC("RPC_SetVelocity", PhotonTargets.All, (QLeft.transform.forward.normalized * 22 + customMomentum) * throwMultiplier * 1.4f);
                SpliceView1.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
                sd = pool.spawnObject(QRight.transform.position, QRight.transform.rotation, 20).GetComponent<SpliceBall>();
                PhotonView SpliceView2 = sd.GetComponent<PhotonView>();
                SpliceView2.RPC("RPC_SetVelocity", PhotonTargets.All, (QRight.transform.forward.normalized * 22 + customMomentum) * throwMultiplier * 1.4f);
                SpliceView2.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
                canvasPlayer.dodgeballIcons[0].SetActive(false);
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, true);

            }
            if (hasE)
            {
                ballView.GetComponent<Dodgeball>().pool = pool;
                splitBallView = ballView;
                ballView.RPC("RPC_SetSpliceSplit", PhotonTargets.All, true);
            }
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, false);
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 1, false);
            hasQ = false;
            //inE = false;
            throwMultiplier = 0;
        }
    }
}
