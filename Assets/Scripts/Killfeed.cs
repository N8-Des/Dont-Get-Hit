using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Killfeed : MonoBehaviour
{
    public Text attackingPlayer;
    public Text hitPlayer;
    public Image image;

    void Start()
    {
        StartCoroutine(death());
    }
    public void setAll(bool red, string playerHit, string playerAttack)
    {
        attackingPlayer.text = playerAttack;
        hitPlayer.text = playerHit;
        if (red)
        {
            image.color = new Color32(254, 73, 92, 154);
        }
    }

    IEnumerator death()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
