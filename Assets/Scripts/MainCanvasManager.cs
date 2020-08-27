using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager Instance;
    [SerializeField]
    private LobbyCanvas _lobbyCanvas;
    public LobbyCanvas lobbyCanvas { get { return _lobbyCanvas; } }
    [SerializeField]
    private CurrentRoomCanvas _currCanvas;
    public CurrentRoomCanvas currCanvas { get { return _currCanvas; } }

    private void Awake()
    {
        Instance = this;
    }
}
