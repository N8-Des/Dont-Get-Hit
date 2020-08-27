using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownImage : MonoBehaviour
{
    [SerializeField]
    Image cooldownRemain;
    [SerializeField]
    Text cooldownSec;
    public bool cooldownUp = true;
    float timeLeft;
    float maxTime;
    protected float cooldownForText;
    public void setCooldown(float cooldown)
    {
        maxTime = cooldown;
        cooldownRemain.fillAmount = 1;
        cooldownUp = false;
        StartCoroutine(startCooldown());
    }
    IEnumerator startCooldown()
    {
        timeLeft = 0;
        while (timeLeft < maxTime || cooldownRemain.fillAmount != 1)
        {
            cooldownSec.gameObject.SetActive(true);
            cooldownForText = maxTime - timeLeft;
            cooldownForText = Mathf.Ceil(cooldownForText);
            cooldownSec.text = cooldownForText.ToString();
            timeLeft += Time.deltaTime;
            float ratio = timeLeft / maxTime;
            cooldownRemain.fillAmount = ratio;
            yield return new WaitForEndOfFrame();
        }
        cooldownRemain.fillAmount = 0;
        cooldownSec.gameObject.SetActive(false);
        cooldownUp = true;
    }
}
