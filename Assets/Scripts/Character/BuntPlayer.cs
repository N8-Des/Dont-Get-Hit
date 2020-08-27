using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuntPlayer : PlayerMovement
{
    public BuntBat bat;
    public UltRemaining ultBar;
    public GameObject buntBat;
    public GameObject buntGun;
    public Transform ballPos;
    protected override void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.isMine)
        {
            pool = GameObject.FindGameObjectWithTag("PrefabPool").GetComponent<PrefabPool>();
            cam.SetActive(true);
            canvasPlayer.gameObject.SetActive(true);
            menu = Instantiate(escMenuPrefab).GetComponent<EscMenu>();
            menu.gameObject.transform.SetParent(canvasPlayer.transform, false);
            menu.gameObject.SetActive(false);
            menu.p = this;
            canvas = GameObject.FindGameObjectWithTag("MenuCanvas").GetComponent<GameCanvas>();
            bat.player = view;
            bat.team = teamOn;
            string val = "0/" + maxUltCharges.ToString();
            canvas.GetComponent<PhotonView>().RPC("RPC_SetStats", PhotonTargets.All, 3, val, teamOn, playerNum);
        }
    }
    protected override void AbilityUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R) && canAttack && ultCharged)
        {
            Anim.SetBool("isIdle", true);
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", false);
            Anim.SetBool("MoveForward", false);
            Anim.SetBool("MoveLeft", false);
            Anim.SetBool("MoveBack", false);
            Anim.SetBool("MoveRight", false);
            Anim.SetTrigger("R");
            inUlt = true;
            ultBar.gameObject.SetActive(true);
            ultBar.setCountdown(7);
            view.RPC("RPC_SetBat", PhotonTargets.All, true);
            canAttack = false;
            canMove = false;
            usedUlt();
        }
    }
    [PunRPC]
    void RPC_SetBat(bool set)
    {
        buntBat.SetActive(!set);
        buntGun.SetActive(set);
        Anim.SetBool("Gun", set);
    }
    protected override IEnumerator colorUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        Renderer r = body[0];
            foreach (Material m in r.materials)
            {
                if (view.isMine)
                {
                    m.SetFloat("_OutlineWidth", 0.0f);
                    m.SetColor("_OutlineColor", new Color32(0, 0, 0, 0));
                }
                else
                {
                    if (teamOn == PunTeams.Team.red)
                    {
                        m.SetColor("_OutlineColor", new Color32(254, 73, 92, 254));
                    }
                    else
                    {
                        m.SetColor("_OutlineColor", new Color32(92, 171, 229, 254));
                    }
                }
            }            
    }
    protected override void dodgeballUpdate()
    {
        if (canAttack)
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                canAttack = false;
                canMove = false;
                Anim.SetBool("isIdle", true);
                Anim.SetBool("isWalking", false);
                Anim.SetBool("isAttacking", false);
                Anim.SetBool("MoveForward", false);
                Anim.SetBool("MoveLeft", false);
                Anim.SetBool("MoveBack", false);
                Anim.SetBool("MoveRight", false);
                if (!inUlt)
                {
                    Anim.SetTrigger("Swing");

                }
                else
                {
                    Anim.SetTrigger("Shoot");

                }
            }
        }
    }
    public void shoot()
    {
        if (view.isMine)
        {
            PhotonView ballView =  pool.spawnObject(ballPos.position, Quaternion.identity, 5).GetComponent<PhotonView>();
            ballView.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
            ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 30) * 1.3f));
        }
    }
    public void endSwing()
    {
        canAttack = true;
        canMove = true;
    }
    protected override void walk()
    {
        if (!isStunned)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);
            if (Mathf.Abs(z) <= 0f && Mathf.Abs(x) <= 0f)
            {
                Anim.SetBool("isIdle", true);
                Anim.SetBool("isWalking", false);
                Anim.SetBool("isAttacking", false);
                Anim.SetBool("MoveForward", false);
                Anim.SetBool("MoveLeft", false);
                Anim.SetBool("MoveBack", false);
                Anim.SetBool("MoveRight", false);
            }
            else
            {
                Anim.SetBool("isIdle", false);
                Anim.SetBool("isWalking", true);
                Anim.SetBool("isAttacking", false);
                if (Vector3.Distance(transform.forward.normalized, controller.velocity.normalized) <= 1)
                {
                    Anim.SetBool("MoveForward", true);
                    Anim.SetBool("MoveLeft", false);
                    Anim.SetBool("MoveBack", false);
                    Anim.SetBool("MoveRight", false);
                }
                else if (Vector3.Distance(-transform.forward.normalized, controller.velocity) <= 1)
                {
                    Anim.SetBool("MoveForward", false);
                    Anim.SetBool("MoveBack", true);
                    Anim.SetBool("MoveRight", false);
                    Anim.SetBool("MoveLeft", false);
                }
                else if ((Vector3.Distance(transform.right.normalized, controller.velocity.normalized) <= 1))
                {
                    Anim.SetBool("MoveForward", false);
                    Anim.SetBool("MoveBack", false);
                    Anim.SetBool("MoveRight", true);
                    Anim.SetBool("MoveLeft", false);
                }
                else if ((Vector3.Distance(-transform.right.normalized, controller.velocity.normalized) <= 1))
                {
                    Anim.SetBool("MoveForward", false);
                    Anim.SetBool("MoveBack", false);
                    Anim.SetBool("MoveRight", false);
                    Anim.SetBool("MoveLeft", true);
                }
            }
        }
    }
    protected override void grabUpdate()
    {
    }
}
