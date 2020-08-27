using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHitbox : MonoBehaviour
{
    public PlayerMovement player;
    public int numBalls;
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Dodgeball" && !player.hasDodgeball)
        {
            if (player.selectedDodgeball != null)
            {
                player.selectedDodgeball.GetComponentInChildren<Renderer>().material.SetFloat("_OutlineWidth", 0.0f);
            }
            coll.GetComponentInChildren<Renderer>().material.SetFloat("_OutlineWidth", 0.02f);
            player.selectedDodgeball = coll.GetComponent<Dodgeball>();
            numBalls++;
        }
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Dodgeball" && !player.hasDodgeball)
        {
            coll.GetComponentInChildren<Renderer>().material.SetFloat("_OutlineWidth", 0.0f);
            numBalls--;
            if (numBalls <= 0)
            {
                numBalls = 0;
                player.selectedDodgeball = null;
            }
        }
    }
}
