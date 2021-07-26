using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListing : MonoBehaviourPunCallbacks
{
    [SerializeField] RoomItem roomItem;
    [SerializeField] Transform content;

    private List<RoomItem> _listings = new List<RoomItem>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                /*  int index = _listings.FindIndex(x >= x.RoomInfo.Name == roomInfo.Name);
                if (indexer != -1) { Destroy(_listing[index].gameObject); } 
                _listings.RemoveAt(index);*/
            }
            else
            {
                RoomItem listing = (RoomItem)Instantiate(roomItem, content);

                //there's error or repeat


                if (listing != null)
                {
                    listing.SetRoomInfo(roomInfo);
                    _listings.Add(listing); //not sure why this is needed
                }
            }
        }
    }
}
