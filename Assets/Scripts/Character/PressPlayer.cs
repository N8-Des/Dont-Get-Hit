using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressPlayer : PlayerMovement
{
    bool pullBallIsActive;
    bool hasQ;
    [SerializeField]
    CooldownImage cdQ;
    [SerializeField]
    CooldownImage cdE;
    PhotonView gravBallView;
    public LayerMask mask;
    [SerializeField]
    GameObject ultPosition;
    [SerializeField]
    GameObject RShowOff;
    Vector3 ultVector;
    bool inE;
    Vector3 uniqueMomentum = new Vector3(0, 2.7f, 0);
    [PunRPC]
    void RPC_SetBall()
    {
        pullBallIsActive = false;
    }
    protected override void AbilityUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack)
        {
            if (cdQ.cooldownUp && !hasQ && hasDodgeball)
            {
                hasQ = true;
                cdQ.setCooldown(30);
                pullBallIsActive = true;
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, true);
                canvasPlayer.dodgeballIcons[0].SetActive(true);
                gravBallView = selectedDodgeball.GetComponent<PhotonView>();                
            }else if (pullBallIsActive)
            {
                hasQ = false;
                canvasPlayer.dodgeballIcons[0].SetActive(false);
                gravBallView.RPC("RPC_GravPull", PhotonTargets.All);
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && canAttack && ultCharged)
        {
            if (!inUlt)
            {
                inUlt = true;
                ultPosition.SetActive(true);
                StartCoroutine(ult());
                RShowOff.SetActive(true);
                Anim.SetBool("inUlt", true);
                Anim.SetBool("hasBall", false);
                isAttacking = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && canAttack && cdE.cooldownUp && hasDodgeball)
        {
            inE = true;
            canvasPlayer.dodgeballIcons[1].SetActive(true);
            view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 1, true);
            cdE.setCooldown(45);
        }
    }
    IEnumerator ult()
    {
        while (inUlt)
        {
            yield return new WaitForEndOfFrame();
            RaycastHit hit;
            Ray raymond = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(raymond, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform.gameObject.tag == "Wall")
                {
                    ultPosition.transform.LookAt(cam.transform.position, Vector3.up);
                }else
                {
                    ultPosition.transform.rotation = Quaternion.Euler(-90, 0, 0);
                }
                ultVector = hit.point;
                ultPosition.transform.position = ultVector;
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            { 
                activateUlt();
            }
            if (Input.GetMouseButton(2))
            {
                inUlt = false;
                RShowOff.SetActive(false);
                ultPosition.SetActive(false);
            }
        }
        isAttacking = false;
        Anim.SetBool("inUlt", false);
        if (hasDodgeball)
        {
            Anim.SetBool("hasBall", false);
        }
        RShowOff.SetActive(false);
    }
    void activateUlt()
    {
        Anim.SetTrigger("R");
        ultPosition.SetActive(false);
        foreach (Dodgeball d in FindObjectsOfType<Dodgeball>())
        {
            d.GetComponent<PhotonView>().RPC("RPC_PressUlt", PhotonTargets.All, ultVector + new Vector3(0, 1, 0));
            d.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, teamOn, view.viewID);
        }
        inUlt = false;
        canMove = false;
        isAttacking = true;
        canAttack = false;
        usedUlt();
    }
    public void endUlt()
    {
        canMove = true;
        canAttack = true;
        isAttacking = false;
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
            ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 18 + uniqueMomentum) * throwMultiplier * 1.3f));
            if (hasQ)
            {
                ballView.RPC("RPC_SetPress", PhotonTargets.All, true);
                canvasPlayer.dodgeballIcons[0].SetActive(false);
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 0, false);

            }
            if (inE)
            {
                ballView.RPC("RPC_PressE", PhotonTargets.All);
                canvasPlayer.dodgeballIcons[1].SetActive(false);
                newBall.GetComponent<Dodgeball>().pressHasControl = true;
                view.RPC("RPC_SetDodgeballEffects", PhotonTargets.All, 1, false);

            }
            hasQ = false;
            inE = false;
            throwMultiplier = 0;
        }
    }

    
}
