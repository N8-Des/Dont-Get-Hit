using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLock : MonoBehaviour
{
    private Transform camPos;

    // Update is called once per frame
    void Update()
    {
        try
        {
            camPos = Camera.main.transform;
            if (camPos != null)
            {
                transform.LookAt(camPos);
            }
        }catch
        {
            //do nothing
        }

    }
}
