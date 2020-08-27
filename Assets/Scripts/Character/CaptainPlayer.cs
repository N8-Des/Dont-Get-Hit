using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainPlayer : PlayerMovement
{
    public CooldownImage CDQ;
    public CooldownImage CDE;
    bool hasE;
    protected override void AbilityUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack && CDQ.cooldownUp)
        {
            canAttack = false;
            isAttacking = true;
            Anim.SetTrigger("Q");
            CDQ.setCooldown(40);
        }
        if (Input.GetKeyDown(KeyCode.E) && canAttack && !hasE)
        {
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, true);
            hasE = true;
        }
        if (Input.GetKeyDown(KeyCode.R) && canAttack)
        {
            canAttack = false;
            Anim.SetTrigger("R");
        }
    }
    public void useFlag()
    {
        if (view.isMine)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, -Vector3.up, out hit, 2f, 1 << LayerMask.NameToLayer("WallGround"));
            GameObject flag = pool.spawnObject(hit.point, Quaternion.identity, 22);
            flag.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, teamOn, view.viewID);
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
    public void setLifelink()
    {
        if (view.isMine)
        {
            PlayerMovement[] p = GameObject.FindObjectsOfType<PlayerMovement>();
            foreach (PlayerMovement player in p)
            {
                if (player.teamOn != teamOn)
                {
                    player.view.RPC("RPC_LifelinkStart", PhotonTargets.All);
                }
            }
        }
    }
    public override void throwBall()
    {
        if (view.isMine)
        {
            canvasPlayer.dodgeballShow.SetActive(false);
            hasDodgeball = false;
            GameObject newBall = pool.spawnObject(dodgeballPosition.position, Quaternion.identity, 2);
            PhotonView ballView = newBall.GetComponent<PhotonView>();
            ballView.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
            ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 25 + upwardsMomentum) * throwMultiplier * 1.3f));
            if (hasE)
            {
                ballView.RPC("RPC_CommanderGrenade", PhotonTargets.All);
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, false);
                canvasPlayer.dodgeballIcons[0].SetActive(false);
                ballView.GetComponent<Dodgeball>().pool = pool;
            }
            hasE = false;
            throwMultiplier = 0;
        }
    }
}
