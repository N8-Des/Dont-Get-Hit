using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameCanvas : MonoBehaviour
{
    public List<PlayerVis> blueImages = new List<PlayerVis>();
    public List<PlayerVis> redImages = new List<PlayerVis>();
    public List<Sprite> playerSprites = new List<Sprite>();

    int bluePlayers;
    int redPlayers;
    [SerializeField]
    GameObject redWin;
    [SerializeField]
    GameObject blueWin;
    public int winsRed;
    public int winsBlue;
    public Text redWinsText;
    public Text blueWinsText;
    public Transform[] dodgeballSpawns;
    public GameObject redFloor;
    public GameObject blueFloor;
    public GameObject killfeed;
    public GameObject killbox;
    public GameObject tabMenu;
    public PlayerTab[] tabsBlue;
    public PlayerTab[] tabsRed;
    public GameObject[] stages;
    Animator anim;
    public GameObject lifelinkAlert;
    PlayerNetwork pnet;
    void Start()
    {
        anim = GetComponent<Animator>();
        pnet = FindObjectOfType<PlayerNetwork>();
    }
    [PunRPC]
    public void RPC_SetEverything(string hell)
    {
        int bluePlayerOn = 0;
        int redPlayerOn = 0;
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            char play = hell[i * 2];
            int player1 = play - '0';
            string team = hell[(i * 2) + 1].ToString();
            if (team == "B")
            {
                blueImages[bluePlayerOn].setInfo(playerSprites[player1]);
                PlayerTab t = tabsBlue[bluePlayerOn];
                t.playerIcon.sprite = playerSprites[player1];
                t.playerName.text = PhotonNetwork.playerList[bluePlayerOn + redPlayerOn].NickName;
                t.kills.text = "0";
                t.deaths.text = "0";
                t.hits.text = "0";
                t.ultcharge.text = "0/0";
                bluePlayerOn++;
            }
            else
            {
                redImages[redPlayerOn].setInfo(playerSprites[player1]);
                PlayerTab t = tabsRed[redPlayerOn];
                t.playerIcon.sprite = playerSprites[player1];
                t.playerName.text = PhotonNetwork.playerList[bluePlayerOn + redPlayerOn].NickName;
                t.kills.text = "0";
                t.deaths.text = "0";
                t.hits.text = "0";
                t.ultcharge.text = "0/0";
                redPlayerOn++;
            }
        }
        redPlayers = redPlayerOn + 1;
        bluePlayers = bluePlayerOn + 1;
        foreach (PlayerTab tab in tabsBlue)
        {
            if (tab.playerName.text == "PLAYERNAME" && tab.kills.text == "K")
            {
                tab.gameObject.SetActive(false);
            }
        }
        foreach (PlayerTab tab in tabsRed)
        {
            if (tab.playerName.text == "PLAYERNAME" && tab.kills.text == "K")
            {
                tab.gameObject.SetActive(false);
            }
        }
    }
    [PunRPC]
    public void RPC_SetStage(int stageID)
    {
        stages[stageID].SetActive(true);
    }
    [PunRPC]
    public void RPC_SetStats(int statType, string change, PunTeams.Team team, int playerID)
    {
        switch (statType)
        {
            case 0:
                if (team == PunTeams.Team.blue)
                {
                    PlayerTab t = tabsBlue[playerID];
                    t.killsN++;
                    t.kills.text = t.killsN.ToString();
                }
                else
                {
                    PlayerTab t = tabsRed[playerID];
                    t.killsN++;
                    t.kills.text = t.killsN.ToString();
                }
                break;
            case 1:
                if (team == PunTeams.Team.blue)
                {
                    PlayerTab t = tabsBlue[playerID];
                    t.deathsN++;
                    t.deaths.text = t.deathsN.ToString();
                }
                else
                {
                    PlayerTab t = tabsRed[playerID];
                    t.deathsN++;
                    t.deaths.text = t.deathsN.ToString();
                }
                break;
            case 2:
                if (team == PunTeams.Team.blue)
                {
                    PlayerTab t = tabsBlue[playerID];
                    t.hitsN++;
                    t.hits.text = t.hitsN.ToString();
                }
                else
                {
                    PlayerTab t = tabsRed[playerID];
                    t.hitsN++;
                    t.hits.text = t.hitsN.ToString();
                }
                break;
            case 3:
                if (team == PunTeams.Team.blue)
                {
                    PlayerTab t = tabsBlue[playerID];
                    t.ultcharge.text = change;
                }
                else
                {
                    PlayerTab t = tabsRed[playerID];
                    t.ultcharge.text = change;
                }
                break;

        }
    }
    [PunRPC]
    public void RPC_PlayerDead(PunTeams.Team team)
    {
        if (team == PunTeams.Team.blue)
        {
            if (PhotonNetwork.isMasterClient)
            {
                bluePlayers--;
            }
            if (bluePlayers <= 1)
            {
                redWin.SetActive(true);
                if (PhotonNetwork.isMasterClient)
                {
                    GetComponent<PhotonView>().RPC("RPC_SyncInt", PhotonTargets.All, true);
                }
                StartCoroutine(startNewRound());
            }
        }else
        {
            if (PhotonNetwork.isMasterClient)
            {
                redPlayers--;
            }
            if (redPlayers <= 1)
            {
                blueWin.SetActive(true);
                if (PhotonNetwork.isMasterClient)
                {
                    GetComponent<PhotonView>().RPC("RPC_SyncInt", PhotonTargets.All, false);
                }
                StartCoroutine(startNewRound());
            }
        }
    }
    [PunRPC]
    void RPC_SyncInt(bool red)
    {
        if (red)
        {
            winsRed++;
            redWinsText.text = winsRed.ToString();
        }
        else
        {
            winsBlue++;
            blueWinsText.text = winsBlue.ToString();
        }
    }
    [PunRPC]
    void RPC_StartGame()
    {
        StartCoroutine(startNewRound());
    }
    IEnumerator startNewRound()
    {
        yield return new WaitForSeconds(2);
        blueWin.SetActive(false);
        redWin.SetActive(false);
        foreach (PlayerVis vis in redImages)
        {
            vis.reset();
        }
        foreach (PlayerVis vis in blueImages)
        {
            vis.reset();
        }
        if (PhotonNetwork.isMasterClient)
        {
            FindObjectOfType<PlayerNetwork>().GetComponent<PhotonView>().RPC("RPC_Reset", PhotonTargets.All);
        }
        anim.SetTrigger("Countdown");
    }
    public void countdown()
    {
        if (GetComponent<PhotonView>().isMine)
        {
            foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>())
            {
                p.GetComponent<PhotonView>().RPC("RPC_EndCountdown", PhotonTargets.All);
            }
        }

    }
    [PunRPC]
    public void RPC_TakeDamage(int playerSending, PunTeams.Team playerTeam)
    {
        if (playerTeam == PunTeams.Team.red)
        {
            redImages[playerSending].takeDamage();
        }else
        {
            blueImages[playerSending].takeDamage();
        }
    }
    [PunRPC]
    public void RPC_TakeHeal(int playerSending, PunTeams.Team playerTeam)
    {
        if (playerTeam == PunTeams.Team.red)
        {
            redImages[playerSending].takeDamage();
        }
        else
        {
            blueImages[playerSending].takeDamage();
        }
    }
    [PunRPC]
    void RPC_ShowKill(bool red, string player1, string player2)
    {
        Killfeed k = Instantiate(killbox, killfeed.gameObject.transform, false).GetComponent<Killfeed>();
        k.setAll(red, player1, player2);
    }
}
