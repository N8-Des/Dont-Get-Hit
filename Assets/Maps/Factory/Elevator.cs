using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public PhotonView view;
    bool isUp;
    bool isStationed;
    public Rigidbody rb;
    public void OnTriggerEnter(Collider collider)
    {
        if (view.isMine)
        {
            if (collider.tag == "Character")
            {
                if (!isUp)
                {

                }
            }
        }
    }
    public void OnTriggerExit(Collider collider)
    {
        if (view.isMine)
        {
            if (collider.tag == "Character")
            {
                if (isUp)
                {
                    view.RPC("setAnim", PhotonTargets.All, false);
                    isUp = false;
                }
            }
        }
    }
    void Update()
    {
        if (isUp)
        {
            rb.AddForce(0, 2, 0);
        }
        else
        {

        }
    }
    [PunRPC]
    void setAnim(bool up)
    {
        isUp = up;
    }
}
