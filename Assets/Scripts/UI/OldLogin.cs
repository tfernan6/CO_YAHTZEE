using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



public class OldLogin : MonoBehaviourPunCallbacks
{
    public static string playerName; //once all the other classes uses the Player object, remove this
    public static string RoomName;


    [SerializeField] private InputField playerNameInpField;
    [SerializeField] private InputField roomNameInpField; 

    [SerializeField] private const int MaxPlayersPerRoom = 6;
    [SerializeField] private const string playerPrefsNameKey = "PlayerName";

    [SerializeField] private List<RoomInfo> RoomList;
    [SerializeField] private List<Player> PlayerList;

    private RoomOptions roomOptions = new RoomOptions();

    /// <summary>
    /// Method: takes
    ///         
    /// </summary>
    public void createNewGameRoom()
    {
         if (playerNameInpField != null)
        {
            playerName = playerNameInpField.text;
            Debug.Log("Player Name " + playerNameInpField.text);

        }
        else
        {
            Debug.LogError("Please enter a player name");
            return;
        }

        if (roomNameInpField != null)
        {
            RoomName = roomNameInpField.text;
            Debug.Log("Room Name " + RoomName);
        }


        roomOptions.MaxPlayers = (byte)MaxPlayersPerRoom;

        if (RoomName.Length != 0)
        {
             PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default);
        }
        else
        {
            Debug.LogError("You have to specify a room name before creating a new game.");
        }
    }


    public void JoinGameRoom()
    {
        if (roomNameInpField != null)
        {
            RoomName = roomNameInpField.text;
            Debug.Log("Joining Room Name " + RoomName);
        }


        PhotonNetwork.JoinRoom(RoomName);
    }

    private string GetPlayerKey()
    {
        if (!PlayerPrefs.HasKey(playerPrefsNameKey))
        {
            return "";
        }
        return PlayerPrefs.GetString(playerPrefsNameKey);
    }

    public void SavePlayerName()
    {
        PhotonNetwork.NickName = playerName;
        string defaultName = GetPlayerKey();

        PlayerPrefs.SetString(playerPrefsNameKey, playerName);

        
    }

    public override void OnJoinedRoom()
    {
        SavePlayerName();

        PhotonNetwork.LoadLevel("GameScene");
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //GameObject.Find("Button").GetComponentInChildren<Text>().text = "Start Game";
        PhotonNetwork.JoinLobby();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}