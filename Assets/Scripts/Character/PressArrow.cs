using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressArrow : Photon.MonoBehaviour
{
    Vector3 prevRot;
    Vector3 midpt;
    void Update()
    {
        prevRot = transform.position;
        midpt = transform.position - prevRot;
        Quaternion look = Quaternion.LookRotation(midpt);
        transform.rotation = look;
    }  
}
