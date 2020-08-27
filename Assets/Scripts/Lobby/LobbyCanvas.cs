using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour
{
    [SerializeField]
    private RoomLayoutGroup roomLayout;
    bool isOn = true;
    private RoomLayoutGroup RoomLayout { get { return roomLayout; } }
    public GameObject RoomMakeCanvas;
    public GameObject JoinLobbyPopup;
    public void OnClick_JoinRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {

        }else
        {
            print("Join room failed");
        }     
    }
    public void OnClick_RoomMake()
    {
        isOn = !isOn;
        gameObject.SetActive(isOn);
    }
    public void OnClick_RoomFind()
    {
        isOn = !isOn;
        gameObject.SetActive(isOn);
        RoomMakeCanvas.SetActive(isOn);
    }
    public void OnClick_PrivateRoomFind()
    {
        JoinLobbyPopup.SetActive(true);
    }
}
