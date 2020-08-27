using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodgeball : Photon.PunBehaviour
{
    public PunTeams.Team teamOn;
    public bool isActive;
    public int playerID;
    public Collider collid;
    public Rigidbody rb;
    protected PhotonView view;
    protected Vector3 realPosition;
    protected Quaternion realRotation;
    protected float currentTime = 0;
    protected double currentPacketTime = 0;
    protected double lastPacketTime = 0;
    protected Vector3 positionAtLastPacket = Vector3.zero;
    protected Quaternion rotationAtLastPacket = Quaternion.identity;
    public PrefabPool pool;
    [SerializeField]
    Renderer rend;
    public bool isTeleporting;
    public SoundManager sm;
    Vector3 theHold = new Vector3(-26, 0, 0);
    float dist;
    bool dontKillEars = true;
    void Start()
    {
        view = GetComponent<PhotonView>();
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
            if (!isTeleporting)
            {
                realPosition = (Vector3)stream.ReceiveNext();
                realRotation = (Quaternion)(stream.ReceiveNext());
                currentTime = 0.0f;
                lastPacketTime = currentPacketTime;
                currentPacketTime = info.timestamp;
                positionAtLastPacket = transform.position;
                rotationAtLastPacket = transform.rotation;
            }

        }
    }
    [PunRPC]
    public void RPC_SetRealPosition(Vector3 pos)
    {
        realPosition = pos;
        positionAtLastPacket = pos;
    }
    protected virtual void Update()
    {
        if (pressHasControl && pressEActive)
        {
            Vector3 lerpy = Camera.main.transform.position + Camera.main.transform.forward * dist;
            transform.position = Vector3.Lerp(transform.position, lerpy, 7 * Time.deltaTime);
        }
        if (rb != null)
        {
            if (rb.velocity.magnitude < 1f && isActive && !pressEActive)
            {
                view.RPC("RPC_SetActive", PhotonTargets.All, false);
            }
        }
        if (!view.isMine)
        {
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(positionAtLastPacket, realPosition, (float)(currentTime / timeToReachGoal));
            transform.rotation = Quaternion.Lerp(rotationAtLastPacket, realRotation, (float)(currentTime / timeToReachGoal));
        }
    }
 
    public virtual void OnCollisionEnter(Collision coll)
    {
        if (view.isMine)
        {
            if (coll.gameObject.tag == "Ground")
            {
                isActive = false;
                if (immolateEActive)
                {
                    GameObject puddle = pool.spawnObject(transform.position, Quaternion.identity, 0);
                    puddle.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, teamOn, playerID);
                    immolateEActive = false;
                }
                if (sporeQActive)
                {
                    GameObject mushroom = pool.spawnObject(transform.position, Quaternion.identity, 7);
                    mushroom.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, teamOn, playerID);
                    sporeQActive = false;
                }
                if (ShadeEActive)
                {
                    pool.spawnObject(transform.position, Quaternion.identity, 12);
                    ShadeEActive = false;
                }
                if (grenaded)
                {
                    GameObject explosion = pool.spawnObject(transform.position, Quaternion.identity, 23);
                    explosion.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, teamOn, playerID);
                    grenaded = false;
                }
            }
            if (coll.gameObject.tag == "Ground" || coll.gameObject.tag == "Wall")
            {
                immolateQActive = false;
                immolateEActive = false;
                pressEActive = false;
                rb.useGravity = true;
                view.RPC("RPC_SetTrails", PhotonTargets.All, false);
                view.RPC("RPC_SetPress", PhotonTargets.All, false);
                try
                {
                    PressPlayer p = PhotonView.Find(playerID).GetComponent<PressPlayer>();
                    if (p != null)
                    {
                        PhotonView.Find(playerID).RPC("RPC_SetBall", PhotonTargets.All);
                    }
                }
                catch
                {
                    //not Press
                }
                try
                {
                    TilePlayer p = PhotonView.Find(playerID).GetComponent<TilePlayer>();
                    if (p != null)
                    {
                        PhotonView.Find(playerID).RPC("RPC_SetBall", PhotonTargets.All);
                    }
                }
                catch
                {
                    //not Splice.
                }

            }

            if (coll.gameObject.tag == "Character" && isActive)
            {
                PhotonView hitView = coll.gameObject.GetComponent<PhotonView>();
                if (hitView.owner.GetTeam() != teamOn)
                {
                    hitView.RPC("RPC_TakeHit", PhotonTargets.All, playerID);
                    pressEActive = false;
                    rb.useGravity = true;
                    isActive = false;
                }
            }
        }
    }

    [PunRPC]
    void RPC_Kill()
    {
        isTeleporting = true;
        dontKillEars = true;
        gameObject.SetActive(false);
    }
    [PunRPC]
    public void RPC_SetVelocity(Vector3 vel)
    {
        isActive = true;
        rb.velocity = vel;
    }   
    [PunRPC]
    void RPC_SetActive(bool act)
    {
        isActive = act;
    }
    [PunRPC]
    public void RPC_SetTeam(PunTeams.Team team, int playerAttacking)
    {
        teamOn = team;
        playerID = playerAttacking;
        isTeleporting = false;
    }
    #region character interactions
    bool immolateQActive;
    Vector3 curveLoc;
    [SerializeField]
    private GameObject ImmolateQTrail;
    [SerializeField]
    private GameObject ImmolateETrail;
    Vector3 upwards = new Vector3(0, 2.5f, 0);
    bool immolateEActive;
    [SerializeField]
    private GameObject BuntTrail;
    [SerializeField]
    GameObject pullHitbox;
    bool pressing;
    public GameObject pressEffect;
    [SerializeField]
    GameObject pressTrail;
    [SerializeField]
    GameObject pressArrowTrail;
    Vector3 pressUltLocation;
    bool sporeQActive;
    [SerializeField]
    GameObject sporeTrail;
    [SerializeField]
    GameObject rampagoTrail;
    bool ShadeEActive;
    [SerializeField]
    GameObject ShadeTrail;
    [SerializeField]
    GameObject VolleyTrail;
    bool pressEActive;
    bool canSplit;
    public bool pressHasControl;
    [SerializeField]
    GameObject SplitTrail;
    [SerializeField]
    GameObject CaptainTrail;
    bool grenaded;
    [PunRPC]
    public void RPC_ImmolateCurveball(Vector3 shootLoc, bool isLeft, Vector3 newPos)
    {
        isActive = true;
        ImmolateQTrail.SetActive(true);
        transform.rotation = Quaternion.identity;
        transform.position = newPos;
        curveLoc = transform.position;
        rb.velocity = shootLoc + upwards;
        immolateQActive = true;
        StartCoroutine(immolateCurve(isLeft));
    }
    [PunRPC]
    public void RPC_ImmolatePuddle()
    {
        immolateEActive = true;
        ImmolateQTrail.SetActive(true);
    }
    [PunRPC]
    public void RPC_SetTrails(bool trails)
    {
        ImmolateQTrail.SetActive(trails);
        ImmolateETrail.SetActive(trails);
        BuntTrail.SetActive(trails);
        sporeTrail.SetActive(trails);
        rampagoTrail.SetActive(trails);
        ShadeTrail.SetActive(trails);
        pressArrowTrail.SetActive(trails);

    }
    [PunRPC]
    void RPC_Bunt()
    {
        BuntTrail.SetActive(true);
    }
    IEnumerator immolateCurve(bool isLeft)
    {
        while (immolateQActive)
        {
            Vector3 dir = rb.velocity - curveLoc;
            if (isLeft)
            {
                Vector3 left = Vector3.Cross(dir, Vector3.up).normalized * 50;
                rb.AddForce(left);
            }else
            {
                Vector3 right = Vector3.Cross(dir, -Vector3.up).normalized * 50;
                rb.AddForce(right);
            }
            yield return null;
        }
        ImmolateQTrail.SetActive(false);
    }
    [PunRPC]
    public void RPC_SetPress(bool press)
    {
        pressing = press;
        pressTrail.SetActive(press);
    }
    [PunRPC]
    public void RPC_GravPull()
    {
        if (pressing)
        {
            pressEffect.SetActive(true);
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            pullHitbox.SetActive(true);
            StartCoroutine(endPull());
            pressing = false;
            PhotonView.Find(playerID).RPC("RPC_SetBall", PhotonTargets.All);
        }
    }
    IEnumerator endPull()
    {
        sm.clips[0].Play();
        view.RPC("RPC_SetPress", PhotonTargets.All, false);
        yield return new WaitForSeconds(0.1f);
        pullHitbox.SetActive(false);
        isActive = false;
        yield return new WaitForSeconds(0.9f);
        pressEffect.SetActive(false);
        rb.useGravity = true;
    }
    [PunRPC]
    public void RPC_PressUlt(Vector3 location)
    {
        isActive = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        pressUltLocation = location;
        StartCoroutine(pressPartTwo());
    }
    IEnumerator pressPartTwo()
    {
        yield return new WaitForSeconds(1.5f);
        rb.velocity = (pressUltLocation - transform.position).normalized * 20;
        rb.useGravity = true;
    }
    [PunRPC]
    void RPC_Sporeshroom()
    {
        sporeQActive = true;
        sporeTrail.SetActive(true);
    }
    [PunRPC]
    void RPC_RampagoThrow()
    {
        sm.clips[1].Play();
        rampagoTrail.SetActive(true);
    }
    [PunRPC]
    void RPC_ShadeE()
    {
        ShadeTrail.SetActive(true);
        ShadeEActive = true;
    }
    [PunRPC]
    void RPC_PressE()
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        isActive = true;
        pressEActive = true;
        pressArrowTrail.SetActive(true);
        StartCoroutine(distance());
    }
    IEnumerator distance()
    {
        while (dist <= 10)
        {
            yield return new WaitForEndOfFrame();
            dist += 0.17f;
        }
    }
    [PunRPC]
    void RPC_SetSpliceSplit(bool split)
    {
        canSplit = split;
        SplitTrail.SetActive(split);
    }
    [PunRPC]
    void RPC_Split()
    {
        sm.playAudioClip(3);
        if (view.isMine)
        {
            Vector3 storedVelocity = rb.velocity;
            view.RPC("RPC_SetVelocity", PhotonTargets.All, Vector3.zero);
            Vector3 dir = storedVelocity.normalized;
            dir = new Vector3(-dir.z, dir.y, dir.x);
            //Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
            GameObject newball = pool.spawnObject(transform.position + dir * 0.1f, Quaternion.identity, 20);
            newball.GetComponent<PhotonView>().RPC("RPC_SetVelocity", PhotonTargets.All, dir * 20);
            dir *= -1;
            GameObject newball2 = pool.spawnObject(transform.position + dir * 0.1f, Quaternion.identity, 20);
            newball2.GetComponent<PhotonView>().RPC("RPC_SetVelocity", PhotonTargets.All, dir * 20);
            newball2.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, teamOn, playerID);
            newball.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, teamOn, playerID);
        }
    }
    [PunRPC]
    void RPC_CommanderGrenade()
    {
        grenaded = true;
    }
    #endregion
}
