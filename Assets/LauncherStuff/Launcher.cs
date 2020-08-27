using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections.Generic;

namespace Com.Sponkr.DodgeOrDie
{
    public class Launcher : Photon.PunBehaviour
    {
        #region Private Serializable Fields
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 10;

        #endregion


        #region Private Fields


        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "0.0.1";


        #endregion
        #region Public Fields
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Panel for joining a room with specific name")]
        [SerializeField]
        private GameObject lobbyPanels;
        private string lobbyName;
        RoomOptions opt;
        TypedLobby lob;
        #endregion

        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            lobbyPanels.SetActive(false);
            controlPanel.SetActive(true);
            Connect();
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        ///
        public void setLobbyName(string lobby)
        {
            if (string.IsNullOrEmpty(lobby))
            {
                Debug.LogError("Lobby Name is null or empty");
                return;
            }
            lobbyName = lobby;
        }
        public void Connect()
        {
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            PhotonNetwork.ConnectUsingSettings("0.0.1");
            PhotonNetwork.gameVersion = "0.0.1"; 
        }
        public void switchToLobby()
        {
            lobbyPanels.SetActive(true);
            controlPanel.SetActive(false);
        }
        public void joinLobby()
        {
            if (PhotonNetwork.connected)
            {
                Debug.Log("uh");
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinOrCreateRoom(lobbyName, null, null);
                lobbyPanels.SetActive(false);
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings("0.0.1");
                PhotonNetwork.gameVersion = "0.0.1";
            }
        }
        #endregion

    }
}