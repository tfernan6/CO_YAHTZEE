using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] private Text _playerName;

    public Player Player { get; private set; }

    public void SetPlayer(Player player)
    {
        Player = player;
        _playerName.text = player.NickName;
    }


}
