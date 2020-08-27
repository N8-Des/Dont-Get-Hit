using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLayoutGroup : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerListingPrebab;
    private GameObject playerListingPrefab { get { return _playerListingPrebab; } }

    public List<PlayerListing> playerListings;
    public int playern;
    int currentPlayerID;
    public Text roomStateText;
    public PlayerNetwork pnet;
    public Text stageName;
    public Image stageSprite;
    public LobbyCanvas mainMenu;
    public GameObject makeRoom;
    public GameObject findRoom;
    private void OnJoinedRoom()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        MainCanvasManager.Instance.currCanvas.transform.SetAsLastSibling();
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i =0; i < photonPlayers.Length; i++)
        {
            playern = photonPlayers.Length;

            PlayerJoinedRoom(photonPlayers[i], i +1);
        }

    }
    private void PlayerJoinedRoom(PhotonPlayer photonPlayer, int id)
    {
        if (photonPlayer == null)
        {
            return;
        }
        PlayerLeftRoom(photonPlayer);
        GameObject playerListingObject = Instantiate(playerListingPrefab);
        playerListingObject.transform.SetParent(transform, false);
        PlayerListing playerListing = playerListingObject.GetComponent<PlayerListing>();
        playerListing.applyPlayerNumber(id);
        playerListing.ApplyPhotonPlayer(photonPlayer);
        playerListings.Add(playerListing);
    }
    private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
    }
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        PlayerJoinedRoom(photonPlayer, PhotonNetwork.playerList.Length);
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            photonPlayer.SetTeam(PunTeams.Team.blue);
            pnet.GetComponent<PhotonView>().RPC("RPC_SetPlayerTeam", PhotonTargets.All, true, photonPlayer);
        }else
        {
            photonPlayer.SetTeam(PunTeams.Team.red);
            pnet.GetComponent<PhotonView>().RPC("RPC_SetPlayerTeam", PhotonTargets.All, false, photonPlayer);

        }
    }
    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
    {
        PlayerLeftRoom(photonPlayer);
    }
    private void PlayerLeftRoom(PhotonPlayer photonPlayer)
    {
        int index = playerListings.FindIndex(x => x.photonPlayer == photonPlayer);
        if (index != -1)
        {
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);
        }
    }
    public void OnClick_Right()
    {       
        if (pnet.stageID + 1 >= pnet.stages.Count)
        {
            pnet.stageID = 0;
            swapImage(pnet.stageID);
        }
        else
        {
            pnet.stageID++;
            swapImage(pnet.stageID);
        }
        pnet.GetComponent<PhotonView>().RPC("RPC_SetMap", PhotonTargets.All, pnet.stageID);
    }
    public void OnClick_Left()
    {
        if (pnet.stageID == 0)
        {
            pnet.stageID = pnet.stages.Count - 1;
            swapImage(pnet.stageID);
        }else
        {
            pnet.stageID--; ;
            swapImage(pnet.stageID);
        }
        pnet.GetComponent<PhotonView>().RPC("RPC_SetMap", PhotonTargets.All, pnet.stageID);

    }
    void swapImage(int stage)
    {
        stageName.text = pnet.stages[stage].stageName;
        stageSprite.sprite = pnet.stages[stage].image;
        pnet.GetComponent<PhotonView>().RPC("RPC_ChangeStage", PhotonTargets.All, stage);
    }
    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        mainMenu.OnClick_RoomFind();
        for (int index = 0; index < playerListings.Count; index++)
        {
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);
        }
    }
    public void OnClick_RoomState()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;
        PhotonNetwork.room.IsVisible = PhotonNetwork.room.IsOpen;

        if (PhotonNetwork.room.IsOpen)
        {
            roomStateText.text = "Room is Open";
        }else
        {
            roomStateText.text = "Room is Closed";
        }
    }
}
