using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private Text roomName;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        roomName.text = "Room: " + roomInfo.Name + ", Players#: " + roomInfo.PlayerCount;
    }

   
}
