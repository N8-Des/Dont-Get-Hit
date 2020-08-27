using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscMenu : MonoBehaviour
{
    PlayerNetwork net;
    public PlayerMovement p;
    public void OnEnable()
    {
        net = FindObjectOfType<PlayerNetwork>();
    }
    public void OnClick_ExitGame()
    {
        net.leaveGame();
    }
    public void OnClick_ResumeGame()
    {
        p.exitMenu();
    }
}
