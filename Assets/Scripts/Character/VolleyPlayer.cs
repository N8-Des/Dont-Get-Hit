using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolleyPlayer : PlayerMovement
{
    Vector3 wallVector;
    Quaternion wallRotation;
    public CooldownImage cdQ;
    [SerializeField]
    GameObject Qposition;
    [SerializeField]
    GameObject Rposition;
    public LayerMask mask;
    Vector3 offset = new Vector3(0, 0.2f, 0);
    Vector3 ultVector;
    bool inQ;
    public Transform volleyPos;
    Vector3 mom = new Vector3(0, 1, 0);
    protected override void AbilityUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack && cdQ.cooldownUp && !charging)
        {
            if (!inQ)
            {
                inQ = true;
                Qposition.SetActive(true);
                StartCoroutine(Q());
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && canAttack && ultCharged)
        {
            if (!inUlt)
            {
                inUlt = true;
                Rposition.SetActive(true);
                StartCoroutine(R());
            }
        }
    }
    public void endAttack()
    {
        canMove = true;
        canAttack = true;
        Anim.SetBool("isAttacking", false);
    }
    IEnumerator Q()
    {
        while (inQ && !charging)
        {
            yield return new WaitForEndOfFrame();
            RaycastHit hit;
            Ray raymond = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(raymond, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform.gameObject.tag == "Ground")
                {
                    wallVector = hit.point;
                    Qposition.transform.position = wallVector + offset;
                    wallRotation = Qposition.transform.rotation;
                } 
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                inQ = false;
                Qposition.SetActive(false);
                Anim.SetTrigger("Q");
                cdQ.setCooldown(40);
                isAttacking = true;
                canAttack = false;
            }
            if (Input.GetMouseButton(2))
            {
                inQ = false;
                Qposition.SetActive(false);
            }
        }
        isAttacking = false;
    }
    IEnumerator R()
    {
        while (inUlt)
        {
            yield return new WaitForEndOfFrame();
            RaycastHit hit;
            Ray raymond = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(raymond, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform.gameObject.tag == "Ground")
                {
                    ultVector = hit.point;
                    Rposition.transform.position = ultVector + offset;
                }
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                inUlt = false;
                Rposition.SetActive(false);
                Anim.SetTrigger("R");
                isAttacking = true;
                canAttack = false;
            }
            if (Input.GetMouseButton(2))
            {
                inUlt = false;
                Rposition.SetActive(false);
            }
        }
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
            ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 35 + mom) * throwMultiplier * 1.3f));
            throwMultiplier = 0;
        }
    }
    public void sendIndicator()
    {
        pool.spawnObject(ultVector, Quaternion.identity, 17);
    }
    public void shootBall()
    {
        GameObject volleyball = pool.spawnObject(volleyPos.position, Quaternion.identity, 15);
        volleyball.GetComponent<PhotonView>().RPC("RPC_StartSpike", PhotonTargets.All, ultVector, teamOn, view.viewID);
    }
    public void activateQ()
    {
        pool.spawnObject(wallVector + new Vector3(0, 0.6f, 0), wallRotation, 14);
    }
}
