using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltRemaining : MonoBehaviour
{
    public Image cooldownImage;
    public bool cooldownUp = true;
    float timeLeft;
    float maxTime;
    public PlayerMovement player;
    public void setCountdown(float cd)
    {
        cooldownImage.fillAmount = 1;
        maxTime = cd;
        cooldownUp = false;
        StartCoroutine(startCooldown());
    }
    IEnumerator startCooldown()
    {
        timeLeft = maxTime;
        while (timeLeft >= 0 || cooldownImage.fillAmount != 0)
        {
            timeLeft -= Time.deltaTime;
            float ratio = timeLeft / maxTime;
            cooldownImage.fillAmount = ratio;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("oh");
        cooldownImage.fillAmount = 0;
        cooldownUp = true;
        player.inUlt = false;
        gameObject.SetActive(false);
        BuntPlayer p = player.GetComponent<BuntPlayer>();
        if (p != null)
        {
            p.view.RPC("RPC_SetBat", PhotonTargets.All, false);
        }
    }
}
