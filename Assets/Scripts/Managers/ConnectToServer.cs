using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public static ConnectToServer instance;
    private bool isLeavingRoom;
    private bool isConnecting;

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
        }
    }


    void LeaveCurrentRoom()
    {
        if (PhotonNetwork.InRoom)
        {
             PhotonNetwork.LeaveRoom();
        }
    }



    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            isConnecting = false;
            PhotonNetwork.JoinLobby();
        }
        else if (isLeavingRoom)
        {
            isLeavingRoom = false;
            // join lobby, create a room or join a room
        }
    }

public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("LoginScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Disconnect()
    {
        isConnecting = false;
        isLeavingRoom = false;
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
            Debug.Log("Disconnecting. . .");
            PhotonNetwork.Disconnect();
       }
        Debug.Log("DISCONNECTED!");
    }

    public void Awake()
    {
        //if an instance already exists
       if(instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }
}
