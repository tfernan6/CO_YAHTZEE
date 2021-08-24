using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace edu.jhu.co
{
#pragma warning disable 649

	/// <summary>
	/// Launch manager. Connect, join a random room or create one if none or all full.
	/// </summary>

	public class Login : MonoBehaviourPunCallbacks
	{

        #region Private Serializable Fields

        //[Tooltip("The Ui Panel to let the user enter name, connect and play")]
        //[SerializeField]
        //private GameObject controlPanel;

        [Tooltip("The Ui Text to inform the user about the connection progress")]
        [SerializeField]
        private Text feedbackText;

        [Tooltip("The maximum number of players per room")]
		public const byte MaxPlayersPerRoom = 6;

		[Tooltip("This is the client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).")]
		public const string GameVersion = "1.0";


		[Tooltip("The name of the room")]
		[SerializeField]
		private const string GameRoomName = "Caffeine Overflow";


		[Tooltip("Enter Game Button")]
		[SerializeField]
		public Button EnterGameButton;

		#endregion

		#region Private Fields
		/// <summary>
		/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
		/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
		/// Typically this is used for the OnConnectedToMaster() callback.
		/// </summary>
		bool isConnecting;

		#endregion

		#region MonoBehaviour CallBacks

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
		/// </summary>
		void Awake()
		{
			// #Critical
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.AutomaticallySyncScene = true;

		}

		void Start()
        {
			if(EnterGameButton == null)
				EnterGameButton = GameObject.Find("EnterGameButton").GetComponent<Button>();
		}

		// Update is called once per frame
		void Update()
		{

			//if user hits, escape close application
			if (Input.GetKey(KeyCode.Return)) {
				Connect();
			}

			if (Input.GetKey("escape"))
			{
				Application.Quit();
			}
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Start the connection process. 
		/// - If already connected, we attempt joining a random room
		/// - if not yet connected, Connect this application instance to Photon Cloud Network
		/// </summary>
		public void Connect()
		{
            // we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
            feedbackText.text = "";

            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = true;

			//// hide the Play button for visual consistency
			//controlPanel.SetActive(false);

			// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
			if (PhotonNetwork.IsConnected)
			{
				 
				//LogFeedback("Joining Room...");
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
				if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
				{
					//PhotonNetwork.JoinRoom(GameRoomName);
					CreateRoom();
				}
				else
				{
					Debug.LogError("Can't join random room now, client is not ready");
				}
			}
			else
			{

				LogFeedback("Connecting...");

				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.GameVersion = GameVersion;
				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void CreateRoom()
		{
			RoomOptions roomOptions = new RoomOptions();
			roomOptions.MaxPlayers = MaxPlayersPerRoom;
			roomOptions.PlayerTtl = 20000; //time in the game
			PhotonNetwork.JoinOrCreateRoom(GameRoomName, roomOptions, null);
		}

		/// <summary>
		/// 
		/// </summary>
		public void JoinLobby()
        {
			if (!PhotonNetwork.IsConnected)
			{
				LogFeedback("Connecting...");

				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.GameVersion = GameVersion;
			}
			SceneManager.LoadScene("Lobby");
		}

		/// <summary>
		/// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
		/// </summary>
		/// <param name="message">Message.</param>
		public void LogFeedback(string message)
		{
            // we do not assume there is a feedbackText defined.
            if (feedbackText == null)
            {
                return;
            }

            // add new messages as a new line and at the bottom of the log.
            feedbackText.text += System.Environment.NewLine + message;
        }

		/// <summary>
		/// 
		/// </summary>
		public void ExitApplication()
        {
			Application.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void EnableGameButton(string value)
        {
		
			if (string.IsNullOrEmpty(value))
			{
				Debug.LogError("Player Name is null or empty");
				EnterGameButton.interactable = false;
			}
            else
            {
				EnterGameButton.interactable = true;

			}
		}


		#endregion


		#region MonoBehaviourPunCallbacks CallBacks
		// below, we implement some callbacks of PUN
		// you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


		/// <summary>
		/// Called after the connection to the master is established and authenticated
		/// </summary>
		public override void OnConnectedToMaster()
		{
			// we don't want to do anything if we are not attempting to join a room. 
			// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
			// we don't want to do anything.
			if (isConnecting)
			{
				LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
				Debug.Log("YazteeGame Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

				// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
				if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
				{
					//PhotonNetwork.JoinRoom(GameRoomName);
					CreateRoom();
				}
				else
				{
					Debug.LogError("Can't join random room now, client is not ready");
				}
			}
		}

		/// <summary>
		/// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
		/// </summary>
		/// <remarks>
		/// Most likely all rooms are full or no rooms are available. <br/>
		/// </remarks>
		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			LogFeedback("Join room failed because " + message); // " return code: " + returnCode.ToString()); //32765, game full
			Debug.Log("YazteeGame Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

			// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
			if (returnCode != 32765) // game full
			{
				CreateRoom();
			}
 
		}


		/// <summary>
		/// Called after disconnecting from the Photon server.
		/// </summary>
		public override void OnDisconnected(DisconnectCause cause)
		{
			LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
			Debug.LogError("YazteeGame Launcher:Disconnected");

			
			isConnecting = false;
			//controlPanel.SetActive(true);

		}

		/// <summary>
		/// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
		/// </summary>
		/// <remarks>
		/// This method is commonly used to instantiate player characters.
		/// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
		///
		/// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
		/// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
		/// enough players are in the room to start playing.
		/// </remarks>
		public override void OnJoinedRoom()
		{
			LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
			Debug.Log("YazteeGame Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

			// #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
			if (PhotonNetwork.IsMasterClient)
			{
				Debug.Log("We load the 'Game' ");

				// #Critical
				// Load the Room Level. 
				//PhotonNetwork.LoadLevel("YazteeGameRoom");
				SceneManager.LoadScene("Game");

			}

			LogFeedback("<Color=Green>OnJoinedRoom</Color> : Total Number of Rooms " + PhotonNetwork.CountOfRooms);  
		}

		public override void OnPlayerLeftRoom(Player other)
		{
			Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
			LogFeedback("OnPlayerLeftRoom() " + other.NickName); 
		}

		public override void OnPlayerEnteredRoom(Player other)
		{
			Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // seen when other connects
			SceneManager.LoadScene("Game");
			LogFeedback("OnPlayerEnteredRoom() " + other.NickName);
		}

		#endregion

	}


}