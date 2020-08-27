using UnityEngine;
using System.Collections.Generic;
public class CurrentRoomCanvas : MonoBehaviour
{
    public List<CharacterSelect> css;
    public Select sel;
    public void OnClick_StartSync()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
        }
    }
}
