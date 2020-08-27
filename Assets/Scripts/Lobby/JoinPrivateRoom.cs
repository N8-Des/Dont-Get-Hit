using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinPrivateRoom : MonoBehaviour
{
    public GameObject warning;
    public Text roomName;
    public void OnClick_JoinPrivate()
    {
        PhotonNetwork.JoinRoom(roomName.text);
        StartCoroutine(wait());
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.4f);
        warning.SetActive(true);
    }
    public void OnClick_XButton()
    {
        gameObject.SetActive(false);
    }
}
