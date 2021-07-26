using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



    public class OldGame : MonoBehaviour
    {
        //Login Page variables
        public Text txtUserName;
        public Text txtPlayerCount;
        public Text txtPlayerList;
        private static TranscriptController transcriptController;

        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";

        private void SetPlayerList()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }


            PhotonNetwork.NickName = defaultName;
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Game Room. Player Name " + OldLogin.playerName);
            if (txtUserName != null)
            {
                txtUserName.text = OldLogin.playerName;
            }

            //TOtal players in room
            txtPlayerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
            Debug.Log("Total players: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());

            /* foreach(var player in PhotonNetwork.CurrentRoom.Players)
             {
                 Player playerObj = player.Value;
                 txtUserName.text = txtUserName.text + "," + playerObj.NickName;
             }*/
            SetPlayerList();


            //Enter the game started by in transcript
            /*  transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
              transcriptController.SendMessageToTranscript("New Game by : " + Login.playerName, TranscriptMessage.SubsystemType.game);
            */

            //  UpdatePlayerList();

        }

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;


            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        private void UpdatePlayerList()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                txtPlayerList.text = txtPlayerList.text + "\n";
                Debug.Log("playerName" + PhotonNetwork.PlayerList[i].NickName);
            }
        }

        public void OnClose()
        {
            Application.Quit();
        }

        // Update is called once per frame
        void Update()
        {

            //if user hits, escape close application
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }
