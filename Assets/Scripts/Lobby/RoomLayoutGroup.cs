﻿using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviour
{
    [SerializeField]
    GameObject roomListingPrefab;
    GameObject RoomListingPrefab { get { return roomListingPrefab; } }
    private List<RoomListing> roomListingButtons = new List<RoomListing>();
    private List<RoomListing> RoomListingButtons { get { return roomListingButtons; } }
    public GameObject noRooms;
    private void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }
        RemoveOldRooms();
    }
    private void RoomReceived(RoomInfo room)
    {
        int index = RoomListingButtons.FindIndex(x => x.RoomName == room.Name);
        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject roomListingObj = Instantiate(RoomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);
                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                RoomListingButtons.Add(roomListing);
                index = (RoomListingButtons.Count - 1);
            }
        }
        if (index != -1)
        {
            RoomListing roomListing = RoomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.Updated = true;
        }
    }
    private void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();
        foreach (RoomListing roomListing in RoomListingButtons)
        {
            if (!roomListing.Updated)
            {
                removeRooms.Add(roomListing);
            }else
            {
                roomListing.Updated = false;
            }
        }
        foreach (RoomListing roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
    void Update()
    {
        if (PhotonNetwork.countOfRooms <= 0)
        {
            noRooms.SetActive(true);
        }
        else
        {
            noRooms.SetActive(false);
        }
    }
}
