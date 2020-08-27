using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using System;

public class PlayerMovement : Photon.MonoBehaviour
{
    public CharacterController controller;
    public int ragdollID;
    public GameObject escMenuPrefab;
    protected EscMenu menu;
    bool inMenu;
    public float speed = 4f;
    private float gravity;
    public Vector3 spawnPosition;
    public Animator Anim;
    public PhotonView view;
    public PunTeams.Team teamOn;
    public int playerNum;
    protected GameCanvas canvas;
    public Dodgeball selectedDodgeball;
    public Transform dodgeballPosition;
    public SoundManager smanage;
    [SerializeField]
    protected Renderer[] body;
    [SerializeField]
    Collider hitbox;
    public bool hasDodgeball;
    protected bool canMove = true;
    public List<GameObject> ballEffects = new List<GameObject>();
    [SerializeField]
    protected PlayerCanvas canvasPlayer;
    protected bool isAttacking;
    protected bool canAttack = true;
    protected bool canThrow;
    protected Vector3 upwardsMomentum = new Vector3(0, 1.5f, 0);
    public GameObject cam;
    protected float throwMultiplier;
    Vector3 realPosition;
    bool isTeleporting;
    Quaternion realRotation;
    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;
    Quaternion rotationAtLastPacket = Quaternion.identity;
    protected PrefabPool pool;
    public OutlineHitbox hb;
    [SerializeField]
    protected int maxUltCharges;
    public int ultCharges;
    GameObject ragdoll;
    public bool isDead;
    protected bool ultCharged;
    [SerializeField]
    protected GrabHitbox grabHitbox;
    [SerializeField]
    private GameObject hitGO;
    protected float cap;
    int health = 3;
    public bool inUlt;
    public bool beingPulled;
    Vector3 positionPull;
    GameObject healParticle = null;
    bool beenHealed;
    public bool isStunned;
    public bool charging;
    Vector3 vec;
    public RectTransform rect;
    Color blue = new Color(92, 171, 229, 254);
    Color red = new Color(254, 73, 92, 254);
    string attackingPlayer;
    PhotonView attackerView;
    bool inRope;
    bool lifelinked;
    GameObject lifelinkParticle;
    public GameObject particleLL;
    protected virtual void Start()
    {
        canAttack = false;
        canMove = false;
        view = GetComponent<PhotonView>();
        canvas = GameObject.FindGameObjectWithTag("MenuCanvas").GetComponent<GameCanvas>();
        lifelinkParticle = Instantiate(particleLL, transform, true);
        lifelinkParticle.transform.localPosition = new Vector3(0, -0.2f, 0);
        if (view.isMine)
        {       
            pool = GameObject.FindGameObjectWithTag("PrefabPool").GetComponent<PrefabPool>();
            cam.SetActive(true);
            canvasPlayer.gameObject.SetActive(true);
            menu = Instantiate(escMenuPrefab).GetComponent<EscMenu>();
            menu.gameObject.transform.SetParent(canvasPlayer.transform, false);
            menu.gameObject.SetActive(false);
            menu.p = this;
            string val = "0/" + maxUltCharges.ToString();
            canvas.GetComponent<PhotonView>().RPC("RPC_SetStats", PhotonTargets.All, 3, val, teamOn, playerNum);

            try
            {
                grabHitbox.gameObject.SetActive(true);
                grabHitbox.teamOn = teamOn;
            }
            catch
            {
                //must be Bunt.
            }
        }
    }
    void OnEnable()
    {
        StartCoroutine(colorUpdate());
    }
    protected virtual IEnumerator colorUpdate()
    {
        yield return new WaitForSeconds(0.1f );
        foreach (Renderer r in body)
        {
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
    }
    protected void Update()
    {
        if (view.isMine)
        {
            dodgeballUpdate();
            AbilityUpdate();
            grabUpdate();
            gravityUpdate();
            menuUpdate();
            spawnUpdate();
            if (canMove)
            {
                walk();
            }
            if (isDead)
            {
                deathWalk();
            }
        }
        else
        {
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(positionAtLastPacket, realPosition, (float)(currentTime / timeToReachGoal));
            transform.rotation = Quaternion.Lerp(rotationAtLastPacket, realRotation, (float)(currentTime / timeToReachGoal));
        }
    }
    void spawnUpdate()
    {
        if (isTeleporting)
        {
            transform.position = spawnPosition;
        }
    }
    void menuUpdate()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            canvas.tabMenu.SetActive(true);
        }else
        {
            canvas.tabMenu.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !isStunned)
        {
            if (!inMenu)
            {
                canMove = false;
                canAttack = false;
                Cursor.lockState = CursorLockMode.None;
                inMenu = true;
                menu.gameObject.SetActive(true);
                cam.GetComponent<PlayerLook>().inMenu = true;
            }
            else
            {
                exitMenu();
            }
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            canvasPlayer.showcase.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.F1))
        {
            canvasPlayer.showcase.SetActive(false);
        }
    }
    [PunRPC]
    public void RPC_Stunned(float stunDur)
    {
        isStunned = true;
        canMove = false;
        canAttack = false;
        Anim.SetBool("isIdle", true);
        StartCoroutine(stunEnd(stunDur));
    }
    IEnumerator stunEnd(float stunDuration)
    {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        canMove = true;
        canAttack = true;
    }
    public void exitMenu()
    {
        canMove = true;
        canAttack = true;
        Cursor.lockState = CursorLockMode.Locked;
        inMenu = false;
        menu.gameObject.SetActive(false);
        cam.GetComponent<PlayerLook>().inMenu = false;
    }
    #region gravity
    Vector3 rope = new Vector3(0, 0.1f, 0);
    void gravityUpdate()
    {
        if (Input.GetKey(KeyCode.F) && inRope)
        {
            canMove = false;
            canAttack = false;
            controller.Move(rope);
        }
        if (Input.GetKeyUp(KeyCode.F) && inRope)
        {
            canMove = true;
            canAttack = true;
            inRope = false;
        }
        gravity -= 0.2f * Time.deltaTime;
        if (controller.isGrounded || beingPulled || inRope)
        {
            gravity = 0;
        } 
        controller.Move(new Vector3(0, gravity, 0));
    }
    void OnTriggerStay(Collider coll)
    {
        if (coll.tag == "Rope")
        {
            controller.Move(coll.transform.forward * 0.2f);
        }
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Rope")
        {
            inRope = false;
        }
    }
    #endregion
    public void playSound(int sound)
    {
        smanage.clips[sound].Play();
    }
    public void CatchBall(int playerAttacking, int dodgeball)
    {
        if (view.isMine)
        {
            PhotonView.Find(playerAttacking).RPC("RPC_TakeHit", PhotonTargets.All, view.viewID);
            PhotonView ballView = PhotonView.Find(dodgeball);
            ballView.RPC("RPC_Kill", PhotonTargets.All);
            hasDodgeball = true;
            canvasPlayer.dodgeballShow.SetActive(true);
        }
    }
    protected virtual void dodgeballUpdate()
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
            if (!isAttacking)
            {
                Anim.SetBool("hasBall", true);
            }
            if (Input.GetMouseButtonDown(0) && !isAttacking)
            {
                StartCoroutine(holdThrow());
                canvasPlayer.throwStrengthSlider.SetActive(true);
            }
        }
    }
    protected virtual void grabUpdate()
    {
        if (Input.GetMouseButtonDown(1) && !hasDodgeball && canAttack)
        {
            Anim.SetTrigger("Catch");
            canMove = false;
            Anim.SetBool("isAttacking", true);
            canAttack = false;
        }
    }
    public void endGrab()
    {
        Anim.SetBool("isAttacking", false);
        canMove = true;
        canAttack = true;
    }
    protected virtual IEnumerator holdThrow()
    {
        isAttacking = true;
        while (Input.GetMouseButton(0))
        {
            yield return new WaitForEndOfFrame();
            charging = true;
            if (throwMultiplier < 1)
            {
                throwMultiplier += 0.015f;
                canvasPlayer.bar.fillAmount = throwMultiplier;
                vec.x = 4 - throwMultiplier * 3;
                vec.y = 4 - throwMultiplier * 3;
                rect.localScale = vec;
            }
        }
        canvasPlayer.throwStrengthSlider.SetActive(false);
        Anim.SetBool("hasBall", false);
        Anim.SetBool("isAttacking", true);
        Anim.SetTrigger("throwBall");
        vec.x = 4;
        vec.y = 4;
        rect.localScale = vec;
        charging = false;
        //throw ball
    }
    protected virtual void AbilityUpdate()
    {

    }    
    [PunRPC]
    public void RPC_SetDodgeballEffects(int effId, bool set)
    {
        ballEffects[effId].SetActive(set);
    }
    public virtual void throwBall()
    {
        if (view.isMine)
        {
            canvasPlayer.dodgeballShow.SetActive(false);
            hasDodgeball = false;
            GameObject newBall = pool.spawnObject(dodgeballPosition.position, Quaternion.identity, 2);
            PhotonView ballView = newBall.GetComponent<PhotonView>();
            ballView.RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), view.viewID);
            ballView.RPC("RPC_SetVelocity", PhotonTargets.All, ((cam.transform.forward.normalized * 20 + upwardsMomentum) * throwMultiplier * 1.3f));
            throwMultiplier = 0;
        }
    }
    [PunRPC]
    public void RPC_EndCountdown()
    {
        canMove = true;
        canAttack = true;
        isTeleporting = false;
    }
    [PunRPC]
    protected void RPC_HitPlayer(Vector3 hitPos)
    {
        if (view.isMine)
        {
            Instantiate(hitGO, hitPos, Quaternion.identity);
            ultCharges++;
            canvas.GetComponent<PhotonView>().RPC("RPC_SetStats", PhotonTargets.All, 2, "nah", teamOn, playerNum);
            string val = ultCharges + "/" + maxUltCharges;
            canvas.GetComponent<PhotonView>().RPC("RPC_SetStats", PhotonTargets.All, 3, val, teamOn, playerNum);
            canvasPlayer.ultMarks[ultCharges - 1].gameObject.SetActive(true);
            if (ultCharges >= maxUltCharges)
            {
                ultCharged = true;
                canvasPlayer.ultBackground.SetActive(true);
            }
        }
    }
    public void usedUlt()
    {
        string val = "0/" + maxUltCharges.ToString();
        canvas.GetComponent<PhotonView>().RPC("RPC_SetStats", PhotonTargets.All, 2, val, teamOn, playerNum);
        foreach (Image i in canvasPlayer.ultMarks)
        {
            i.gameObject.SetActive(false);
        }
        canvasPlayer.ultBackground.SetActive(false);
        ultCharged = false;
        ultCharges = 0;
    }
    public void endThrow()
    {
        if (view.isMine)
        {
            isAttacking = false;
            Anim.SetBool("isAttacking", false);
            canAttack = true;
        }
    }
    [PunRPC]
    public void RPC_TakeHit(int playerHit)
    {
        if (view.isMine)
        {
            PhotonView.Find(playerHit).RPC("RPC_HitPlayer", PhotonTargets.All, transform.position);
            attackingPlayer = PhotonView.Find(playerHit).owner.NickName;
            attackerView = PhotonView.Find(playerHit);
            canvas.GetComponent<PhotonView>().RPC("RPC_TakeDamage", PhotonTargets.All, playerNum, PhotonNetwork.player.GetTeam());
            canvasPlayer.hit.SetActive(true);
            Invoke("endHit", 1.6f);
            health--;
            if (lifelinked)
            {
                PlayerMovement[] players = GameObject.FindObjectsOfType<PlayerMovement>();
                foreach (PlayerMovement p in players)
                {
                    if (!p.view.isMine && p.teamOn == teamOn)
                    {
                        p.view.RPC("RPC_TakeHit", PhotonTargets.All, playerHit);
                    }
                }
            }
            if (health <= 0)
            {
                view.RPC("RPC_Die", PhotonTargets.All);
            }
        }
    }
    [PunRPC]
    public void RPC_Die()
    {
        canMove = false;
        canAttack = false;
        beingPulled = false;
        foreach (Renderer b in body)
        {
            b.enabled = false;
        }
        hitbox.enabled = false;
        isDead = true;
        gameObject.layer = 9;
        if (view.isMine)
        {
            ragdoll = pool.spawnObject(transform.position, transform.rotation, ragdollID);
            bool isRed = false;
            if (teamOn == PunTeams.Team.red)
            {
                isRed = true;
            }            
            canvas.GetComponent<PhotonView>().RPC("RPC_ShowKill", PhotonTargets.All, isRed, attackingPlayer, PhotonNetwork.player.NickName);
            canvas.GetComponent<PhotonView>().RPC("RPC_SetStats", PhotonTargets.All, 1, "nah", teamOn, playerNum);
            attackerView.RPC("RPC_KilledPlayer", PhotonTargets.All);
        }
    }
    [PunRPC]
    public void RPC_KilledPlayer()
    {
        if (view.isMine)
        {
            canvas.GetComponent<PhotonView>().RPC("RPC_SetStats", PhotonTargets.All, 0, "nah", teamOn, playerNum);

        }
    }
    [PunRPC]
    public void RPC_ForceDie()
    {
        canMove = false;
        canAttack = false;
        beingPulled = false;

        foreach (Renderer b in body)
        {
            b.enabled = false;
        }
        hitbox.enabled = false;
        isDead = true;
        gameObject.layer = 9;
        if (view.isMine)
        {
            ragdoll = pool.spawnObject(transform.position, transform.rotation, ragdollID);
        }
        canvas.GetComponent<PhotonView>().RPC("RPC_TakeDamage", PhotonTargets.All, playerNum, PhotonNetwork.player.GetTeam());
        canvas.GetComponent<PhotonView>().RPC("RPC_TakeDamage", PhotonTargets.All, playerNum, PhotonNetwork.player.GetTeam());
        canvas.GetComponent<PhotonView>().RPC("RPC_TakeDamage", PhotonTargets.All, playerNum, PhotonNetwork.player.GetTeam());

    }
    [PunRPC]
    public void RPC_StartRound()
    {
        hasDodgeball = false;
        transform.position = spawnPosition;
        if (!view.isMine)
        {
            foreach (Renderer b in body)
            {
                b.enabled = true;
            }
        }
        view.RPC("RPC_SetTeleport", PhotonTargets.All, true);
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
        Anim.SetBool("MoveForward", false);
        Anim.SetBool("MoveLeft", false);
        Anim.SetBool("MoveBack", false);
        Anim.SetBool("MoveRight", false);
        isDead = false;
        canMove = false;
        canAttack = false;
        hitbox.enabled = true;
        hasDodgeball = false;
        health = 3;
        gameObject.layer = 11;
    }
    [PunRPC]
    public void RPC_SetPosition(Vector3 newpos)
    {
        transform.position = newpos;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            realPosition = (Vector3)(stream.ReceiveNext());
            realRotation = (Quaternion)(stream.ReceiveNext());
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.timestamp;
            positionAtLastPacket = transform.position;
            rotationAtLastPacket = transform.rotation;
        }
    }
    void deathWalk()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }
    [PunRPC]
    public void RPC_LifelinkStart()
    {
        if (view.isMine)
        {
            lifelinked = true;
            canvas.lifelinkAlert.SetActive(true);
        }
        else
        {
            lifelinkParticle.SetActive(true);
        }
        StartCoroutine(endLink());

    }
    IEnumerator endLink()
    {
        yield return new WaitForSeconds(10);
        lifelinked = false;
        canvas.lifelinkAlert.SetActive(false);
        lifelinkParticle.SetActive(false);
    }
    [PunRPC]
    public void RPC_HealMe()
    {
        if (view.isMine)
        {
            if (!beenHealed)
            {
                healParticle =  pool.spawnObject(transform.position, Quaternion.identity, 8);
                view.RPC("RPC_ParticleActivate", PhotonTargets.All, healParticle.GetComponent<PhotonView>().viewID);
                beenHealed = true;
            }
            if (health != 3)
            {
                canvas.GetComponent<PhotonView>().RPC("RPC_TakeHeal", PhotonTargets.All, playerNum, PhotonNetwork.player.GetTeam());
            }
        }
        if (beenHealed)
        {
            healParticle.SetActive(true);
        }
        if (health != 3)
        {
            health++;
        }
    }
    [PunRPC]
    protected void RPC_ParticleActivate(int particleID)
    {
        healParticle = PhotonView.Find(particleID).gameObject;
        healParticle.transform.parent = transform;
    }
    [PunRPC]
    public void RPC_SetTeleport(bool t)
    {
        isTeleporting = t;
    }
    protected virtual void walk()
    {
        if (!isStunned)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * (speed * 0.8f) * Time.deltaTime);
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
    void endHit()
    {
        canvasPlayer.hit.SetActive(false);
    }
    [PunRPC]
    public void RPC_GetPulled(Vector3 pullPos)
    {
        positionPull = pullPos;
        beingPulled = true;
        StartCoroutine(pull());
    }
    IEnumerator pull()
    {
        while (Vector3.Distance(transform.position, positionPull) >= 0.2 && beingPulled)
        {
            transform.position = Vector3.MoveTowards(transform.position, positionPull, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        beingPulled = false;
        canMove = true;
        canAttack = true;
    }
    [PunRPC]
    protected void RPC_SetTeam(PunTeams.Team team, int playerNumber, Vector3 spawnLocation)
    {
        teamOn = team;
        playerNum = playerNumber;
        spawnPosition = spawnLocation;
    }
    [PunRPC]
    public void RPC_SpeedChange(float change)
    {
        if (view.isMine)
        {
            speed += change;
        }
    }

}
