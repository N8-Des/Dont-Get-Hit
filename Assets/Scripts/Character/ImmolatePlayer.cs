using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmolatePlayer : PlayerMovement
{
    bool inQ;
    public CooldownImage CDQ;
    public CooldownImage CDE;
    [SerializeField]
    GameObject QShowOff;
    [SerializeField]
    Transform qposition;
    bool activeE;
    protected override void AbilityUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack && hasDodgeball && CDQ.cooldownUp && !charging)
        {
            inQ = true;
            QShowOff.SetActive(true);
            StartCoroutine(testQ());
        }
        if (Input.GetKeyDown(KeyCode.E) && canAttack && hasDodgeball && CDE.cooldownUp)
        {
            activeE = true;
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, true);
            canvasPlayer.dodgeballIcons[0].SetActive(true);
            CDE.setCooldown(30);
        }
        if (Input.GetKeyDown(KeyCode.R) && canAttack && ultCharged)
        {
            Anim.SetBool("hasBall", false);
            Anim.SetBool("isAttacking", true);
            Anim.SetTrigger("R");
            usedUlt();
        }
    }
    public void spawnUlt()
    {
        if (view.isMine)
        {
            GameObject fire = pool.spawnObject(transform.position, transform.rotation, 1);
            fire.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
        }
    }
    #region Q
    IEnumerator testQ()
    {
        while (inQ)
        {
            if (Input.GetMouseButton(0))
            {
                Anim.SetBool("hasBall", false);
                Anim.SetTrigger("QL");
                Anim.SetBool("isAttacking", true);
                isAttacking = true;
                inQ = false;
                QShowOff.SetActive(false);
                CDQ.setCooldown(25);
            }
            if (Input.GetMouseButton(1))
            {
                Anim.SetBool("hasBall", false);
                Anim.SetTrigger("QR");
                Anim.SetBool("isAttacking", true);
                isAttacking = true;
                hasDodgeball = false;
                inQ = false;
                QShowOff.SetActive(false);
                CDQ.setCooldown(25);
            }
            if (Input.GetMouseButton(2))
            {
                print("cancel");
                inQ = false;
                QShowOff.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public void QLeft()
    {
        GameObject newBall = pool.spawnObject(dodgeballPosition.position, Quaternion.identity, 2);
        PhotonView ballView = newBall.GetComponent<PhotonView>();
        ballView.RPC("RPC_ImmolateCurveball", PhotonTargets.All, transform.forward.normalized * 16, true, qposition.position);
        ballView.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
        selectedDodgeball = null;
        hasDodgeball = false;
    }
    public void QRight()
    {
        GameObject newBall = pool.spawnObject(dodgeballPosition.position, Quaternion.identity, 2);
        PhotonView ballView = newBall.GetComponent<PhotonView>();
        ballView.RPC("RPC_ImmolateCurveball", PhotonTargets.All, (transform.forward.normalized)* 16, false, qposition.position);
        ballView.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
        selectedDodgeball = null;
        hasDodgeball = false;
    }
    #endregion

    protected override void dodgeballUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F) && selectedDodgeball != null && !hasDodgeball)
        {
            selectedDodgeball.GetComponent<PhotonView>().RPC("RPC_Kill", PhotonTargets.All);
            hb.numBalls--;
            hasDodgeball = true;
            canvasPlayer.dodgeballShow.SetActive(true);
        }
        if (hasDodgeball)
        {
            if (!isAttacking && !inQ)
            {
                Anim.SetBool("hasBall", true);
            }
            if (Input.GetMouseButtonDown(0) && !isAttacking && !inQ)
            {
                StartCoroutine(holdThrow());
                canvasPlayer.throwStrengthSlider.SetActive(true);
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
            if (activeE)
            {
                ballView.RPC("RPC_ImmolatePuddle", PhotonTargets.All);
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, false);
                canvasPlayer.dodgeballIcons[0].SetActive(false);
                ballView.GetComponent<Dodgeball>().pool = pool;
            }
            activeE = false;
            throwMultiplier = 0;
        }
    }
}
