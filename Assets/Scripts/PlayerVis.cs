using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVis : MonoBehaviour
{
    public Image PlayerIcon;
    public List<GameObject> health = new List<GameObject>();
    public int life = 3;
    public PunTeams.Team team;
    int playerNum;
    GameCanvas canvas;
    [SerializeField]
    GameObject X;
    public void Start()
    {
        canvas = GetComponentInParent<GameCanvas>();
    }
    public void setInfo(Sprite icon)
    {
        PlayerIcon.sprite = icon;
    }
    public void takeDamage()
    {
        life--;
        health[life].SetActive(false);
        if (life <= 0)
        {
            X.SetActive(true);
            if (PhotonNetwork.isMasterClient)
            {
                canvas.GetComponent<PhotonView>().RPC("RPC_PlayerDead", PhotonTargets.All, team);
            }
        }
    }
    public void takeHeal()
    {
        life++;
        health[life].SetActive(true);
    }
    public void reset()
    {
        X.SetActive(false);
        life = 3;
        foreach (GameObject go in health)
        {
            go.SetActive(true);         
        }
    }
}
