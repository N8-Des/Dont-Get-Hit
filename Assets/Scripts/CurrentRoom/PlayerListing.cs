using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerListing : MonoBehaviour
{
    public Sprite[] playerImages;
    public PhotonPlayer photonPlayer;
    public Image button;
    public int playerID;
    int charID;
    object newID;
    PunTeams.Team teamSet;
    [SerializeField]
    private Text _playerName; 
    private Text playerName { get { return _playerName; } }
    public Image character;
    public PlayerNetwork pnet;
    List<PhotonPlayer> b;
    List<PhotonPlayer> r;

    public void ApplyPhotonPlayer(PhotonPlayer _photonPlayer)
    {
        playerName.text = _photonPlayer.NickName;
        photonPlayer = _photonPlayer;
        OnClick_SetTeam();
    }
    public void OnClick_SetTeam()
    {
        if (PhotonNetwork.isMasterClient || PhotonNetwork.player.ID == photonPlayer.ID)
        {
            if (photonPlayer.GetTeam() == PunTeams.Team.red)
            {
                if (PunTeams.PlayersPerTeam.TryGetValue(PunTeams.Team.blue, out b))
                {
                    if (b.Count >= 5)
                    {

                    }
                    else
                    {
                        button.color = new Color32(92, 171, 229, 254);
                        photonPlayer.SetTeam(PunTeams.Team.blue);
                        teamSet = PunTeams.Team.blue;
                        pnet.GetComponent<PhotonView>().RPC("RPC_ChangeList", PhotonTargets.All, photonPlayer);
                    }
                }
            }
            else
            {
                if (PunTeams.PlayersPerTeam.TryGetValue(PunTeams.Team.red, out r))
                {
                    if (r.Count >= 5)
                    {

                    }
                    else
                    {
                        button.color = new Color32(254, 73, 92, 254);
                        photonPlayer.SetTeam(PunTeams.Team.red);
                        teamSet = PunTeams.Team.red;
                        pnet.GetComponent<PhotonView>().RPC("RPC_ChangeList", PhotonTargets.All, photonPlayer);
                    }
                }
            }
        }
    }
    void Update()
    {
        if (photonPlayer.CustomProperties.ContainsValue(charID))
        {
            //nothin
        }else
        {
            photonPlayer.CustomProperties.TryGetValue("PlayerID", out newID);
            charID = Convert.ToInt32(newID);
            character.sprite = playerImages[charID];
        }
        if (photonPlayer.GetTeam() != teamSet)
        {
            teamSet = photonPlayer.GetTeam();
            if (teamSet == PunTeams.Team.blue)
            {
                button.color = new Color32(92, 171, 229, 254);
            }else
            {
                button.color = new Color32(254, 73, 92, 254);
            }
        }
    }
    public void applyPlayerNumber(int id)
    {
        playerID = id;
    }
    public void OnEnable()
    {
        pnet = FindObjectOfType<PlayerNetwork>();
    }
}
