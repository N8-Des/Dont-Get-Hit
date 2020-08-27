using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public PlayerNetwork net;
    public int charID;
    public GameObject selectIndicator;
    public CurrentRoomCanvas canvasHold;
    public PlayerLayoutGroup layout;
    ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
    public void OnClick_SetChar()
    {
        net.OnClick_SetID(charID);
        foreach(CharacterSelect c in canvasHold.css)
        {
            c.selectIndicator.SetActive(false);
        }
        selectIndicator.SetActive(true);
        if (hash.ContainsKey("PlayerID"))
        {
            hash.Remove("PlayerID");
        }
        hash.Add("PlayerID", charID);
        PhotonNetwork.player.SetCustomProperties(hash);
        foreach (RectTransform can in canvasHold.sel.infoObject)
        {
            can.gameObject.SetActive(false);
        }
        canvasHold.sel.infoObject[charID].gameObject.SetActive(true);
    }
}
