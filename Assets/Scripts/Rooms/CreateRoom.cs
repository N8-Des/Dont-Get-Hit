using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    [SerializeField]
    private Text roomName;
    [SerializeField]
    private Text roomPassword;
    bool isVisible = true;
    public Image publicButton;
    public Image privateButton;
    public Color32 greyedOut;
    public Color32 selected;
    private ExitGames.Client.Photon.Hashtable password = new ExitGames.Client.Photon.Hashtable();
    
    private Text RoomName
    {
        get { return roomName; }
    }
    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10};
        if (PhotonNetwork.CreateRoom(roomName.text, roomOptions, TypedLobby.Default))
        {
            print("Create room successful.");           
        }
        else
        {
            print("Create room failed.");
        }
    }
    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("Create room failed: " + codeAndMessage[1]);
    }
    private void OnCreatedRoom()
    {
        print("Room created successfully.");
    }
    public void setPlayerName(string name)
    {
        PhotonNetwork.player.NickName = name;
    }
    public void setRoomState(bool isPublic)
    {
        if (isPublic)
        {
            privateButton.color = greyedOut;
            publicButton.color = selected;
        }else
        {
            publicButton.color = greyedOut;
            privateButton.color = selected;
        }
        isVisible = isPublic;
    }
}

