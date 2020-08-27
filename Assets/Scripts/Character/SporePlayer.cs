using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporePlayer : PlayerMovement
{
    bool QActive;
    public CooldownImage cdQ;
    protected override void AbilityUpdate()
    {
        if (canAttack && Input.GetKey(KeyCode.Q) && cdQ.cooldownUp && hasDodgeball)
        {
            QActive = true;
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, true);
            canvasPlayer.dodgeballIcons[0].SetActive(true);

            cdQ.setCooldown(35);
        }
        if (canAttack && Input.GetKey(KeyCode.R) && ultCharged)
        {
            Anim.SetTrigger("R");
            canAttack = false;
            foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>())
            {
                if (p.teamOn == teamOn && !p.isDead)
                {
                    p.view.RPC("RPC_HealMe", PhotonTargets.All);
                }
            }
            usedUlt();
        }
    }
    public override void throwBall()
    {
        canvasPlayer.dodgeballShow.SetActive(false);
        hasDodgeball = false;
        GameObject newBall = pool.spawnObject(dodgeballPosition.position, Quaternion.identity, 2);
        PhotonView ballView = newBall.GetComponent<PhotonView>();
        ballView.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
        ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 23 + upwardsMomentum) * throwMultiplier * 1.3f));
        if (QActive)
        {
            ballView.RPC("RPC_Sporeshroom", PhotonTargets.All);
            ballView.GetComponent<Dodgeball>().pool = pool;
            canvasPlayer.dodgeballIcons[0].SetActive(false);
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, false);
            QActive = false;

        }
        throwMultiplier = 0;
    }
}
