using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadePlayer : PlayerMovement
{
    public CooldownImage cdQ;
    public CooldownImage cdE;
    bool isInvisible;
    bool inE;
    Vector3 wallPos = new Vector3(0, 2, 0);
    protected override void AbilityUpdate()
    {
        if (canAttack && Input.GetKeyDown(KeyCode.Q) && cdQ.cooldownUp)
        {
            Anim.SetTrigger("Q");
            isInvisible = true;
            view.RPC("RPC_SetInvis", PhotonTargets.All, false);
            StartCoroutine(endInvis());
            cdQ.setCooldown(40);
        }
        if (canAttack && Input.GetKeyDown(KeyCode.R) && ultCharged)
        {
            Anim.SetTrigger("R");
            if (teamOn == PunTeams.Team.red)
            {
                pool.spawnObject(wallPos, Quaternion.Euler(0, 0, 0), 11);
            }else
            {
                pool.spawnObject(wallPos, Quaternion.Euler(0, 180, 0), 11);
            }
            usedUlt();
        }
        if (canAttack && Input.GetKeyDown(KeyCode.E) && hasDodgeball)
        {
            inE = true;
            canvasPlayer.dodgeballIcons[0].SetActive(true);
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, true);
            cdE.setCooldown(30);
        }
    }
    [PunRPC]
    void RPC_SetInvis(bool inv)
    {
        foreach(Renderer r in body)
        {
            r.enabled = inv;
        }
    }
    IEnumerator endInvis()
    {
        yield return new WaitForSeconds(10);
        isInvisible = false;
        view.RPC("RPC_SetInvis", PhotonTargets.All, true);
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
            ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 20 + upwardsMomentum) * throwMultiplier * 1.3f));
            if (inE)
            {
                ballView.RPC("RPC_ShadeE", PhotonTargets.All);
                inE = false;
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, false);
                ballView.GetComponent<Dodgeball>().pool = pool;
                canvasPlayer.dodgeballIcons[0].SetActive(false);
            }
            throwMultiplier = 0;
        }
        if (isInvisible)
        {
            isInvisible = false;
            view.RPC("RPC_SetInvis", PhotonTargets.All, true);
        }
    }
}
