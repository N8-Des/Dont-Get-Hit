using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampagoPlayer : PlayerMovement
{
    bool isDashing;
    Vector3 dashPosition;
    public CooldownImage cdQ;
    public CooldownImage cdE;
    bool isE;
    protected override void AbilityUpdate()
    {
        if (canAttack && Input.GetKey(KeyCode.Q) && cdQ.cooldownUp)
        {
            Debug.Log("E");
            cdQ.setCooldown(7.5f);
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            if (Mathf.Abs(z) <= 0f && Mathf.Abs(x) <= 0f)
            {
                return;
            }
            Anim.SetBool("isIdle", true);
            Anim.SetBool("isAttaking", false);
            Anim.SetBool("isWalking", false);

            canMove = false;
            canAttack = false;
            if (Vector3.Distance(transform.forward.normalized, controller.velocity.normalized) <= 1)
            {

                Anim.SetTrigger("DashForwards");
            }
            else if (Vector3.Distance(-transform.forward.normalized, controller.velocity) <= 1)
            {
                Anim.SetTrigger("DashBackwards");
            }
            else if ((Vector3.Distance(transform.right.normalized, controller.velocity.normalized) <= 1))
            {
                Anim.SetTrigger("DashRight");
            }
            else if ((Vector3.Distance(-transform.right.normalized, controller.velocity.normalized) <= 1))
            {
                Anim.SetTrigger("DashLeft");
            }
            playSound(0);
            isDashing = true;
            dashPosition = controller.velocity.normalized;
            StartCoroutine(dash());
            StartCoroutine(dashCancelPrecaution());
        }
        if (canAttack && !isE && Input.GetKey(KeyCode.E) && cdE.cooldownUp && hasDodgeball)
        {
            isE = true;
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, true);
            canvasPlayer.dodgeballIcons[0].SetActive(true);
            cdE.setCooldown(25);
        }
        if (canAttack && Input.GetKeyDown(KeyCode.R) && ultCharged)
        {
            Anim.SetTrigger("R");
            Anim.SetBool("isIdle", true);
            Anim.SetBool("isAttacking", false);
            canAttack = false;
            canMove = false;
            usedUlt();
        }
    }
    public void Ult()
    {
        if (teamOn == PunTeams.Team.red)
        {
            canvas.blueFloor.SetActive(true);
        }else
        {
            canvas.redFloor.SetActive(true);
        }
        StartCoroutine(endUltEffect());
        if (view.isMine)
        {
            foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>())
            {
                if (p.teamOn != teamOn && !p.isDead)
                {
                    p.view.RPC("RPC_Stunned", PhotonTargets.All, 2.0f);
                }
            }
        }
    }
    public void endAttack()
    {
        canMove = true;
        canAttack = true;
        Anim.SetBool("isAttacking", false);
    }
    IEnumerator dashCancelPrecaution()
    {
        yield return new WaitForSeconds(0.25f);
        isDashing = false;
        yield return new WaitForSeconds(0.18f);
        canMove = true;
        canAttack = true;
    }
    IEnumerator dash()
    {
        while (isDashing)
        {
            controller.Move(dashPosition * 12 * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator endUltEffect()
    {
        yield return new WaitForSeconds(1.5f);
        canvas.blueFloor.SetActive(false);
        canvas.redFloor.SetActive(false);

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
            if (isE)
            {
                ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 38) * throwMultiplier * 1.5f));
                ballView.RPC("RPC_RampagoThrow", PhotonTargets.All);
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, false);
                canvasPlayer.dodgeballIcons[0].SetActive(false);
                isE = false;

            }
            else
            {
                ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 28 + upwardsMomentum) * throwMultiplier * 1.3f));
            }
            throwMultiplier = 0;
        }
    }
}
