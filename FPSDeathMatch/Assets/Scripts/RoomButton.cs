using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
public class RoomButton : MonoBehaviour
{
    [SerializeField] public RoomInfo roomInfo;
    [SerializeField] private TMP_Text roomName;
    public void SetRoomInfo(RoomInfo info)
    {
        roomInfo = info;
        roomName.text = info.Name;
    }

    public RoomInfo RoomInformation
    {
        get { return roomInfo;}
        set { roomInfo = value;}
    }
    public void JoinRoom()
    {
        NetworkManager.instance.JoinRoom(roomInfo.Name);
    }
}