using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime; 
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;
using Photon;
using ExitGames.Client.Photon;

namespace edu.jhu.co
{
    public class GameManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
    {
        [Tooltip("The Ui Text to inform the user about the connection progress")]
        [SerializeField]
        private Text CurrentPlayerName;


        [Tooltip("The Ui Text to inform the user about the connection progress")]
        [SerializeField]
        private Text GameStatus;

        [Tooltip("The Ui Text to inform the user PlayerList")]
        [SerializeField]
        private Text PlayerList;

        //Code this as a class to receive dice value(int) and criteria(enum)
        [Tooltip("Score Object")]
        [SerializeField]
        public GameObject ScoreObject; 

        //This enables the begin game button
        //only first player can start the game (change if needed)
        [Tooltip("Begin Game")]
        [SerializeField]
        public GameObject BeginGame; 

        //This is panel where the dices will be placed,
        //currently has inout field and button to test Multiplayer turn feature
        [Tooltip("GamePanel")]
        [SerializeField]
        public GameObject GamePanel;

        [Tooltip("The welcome text on top of the game board")]
        [SerializeField]
        private Text WelcomeText;

        private YahtzeePlayer yahtzeePlayer = null;

        public bool gameStarted = false; 
        public Text myDiceValue;

        public PunTurnManager turnManager;

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

        #endregion

        private void SetGameStatus()
        {
            WelcomeText.text = "Welcome " + PhotonNetwork.NickName + " to play Yahtzee++" + System.Environment.NewLine +
                       "You are in " + PhotonNetwork.CurrentRoom.Name;
        }

        /// <summary>
        /// 
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Login");
                return;
            }

            SetGameStatus();

            if(yahtzeePlayer == null)
            {
                yahtzeePlayer = new YahtzeePlayer();
            }
            yahtzeePlayer.CurrentPlayerName = PhotonNetwork.NickName;
            Debug.Log("Current Player: " + PhotonNetwork.NickName);

            //update the list of players
            CurrentPlayerName.text = PhotonNetwork.NickName;
            this.UpdatePlayerTexts();


            //define the turn manager
            this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
            this.turnManager.TurnManagerListener = this;

            // duration of the turn
            turnManager.TurnDuration = 5f; // 5seconds

            //enable begin game only for the first person
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 1)
            {
                this.BeginGame.SetActive(true);
                
            }
            else
            {
                //display that the first person has to press start
                this.BeginGame.SetActive(false);
            }
            
            //Panel for dice turn disable till game clicked
            this.GamePanel.SetActive(false);
        }

        public void RefreshPanels()
        {
            
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            LogFeedback("Player " + other.NickName + " left the Game");
            this.UpdatePlayerTexts();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // seen when other connects 
            LogFeedback("Player " + other.NickName + " joined the Game");

            //enable begin game if you have one player
            if (PhotonNetwork.IsMasterClient &&
                PhotonNetwork.PlayerList.Length > 1 &&
                gameStarted == false) 
            {
                //game not begun)
                this.BeginGame.SetActive(true);
            }

            this.UpdatePlayerTexts();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogFeedback(string message)
        {
            // we do not assume there is a feedbackText defined.
            if (GameStatus == null)
            {
                return;
            }

            // add new messages as a new line and at the bottom of the log.
            GameStatus.text = System.Environment.NewLine + message;
        }

        /// <summary>
        /// 
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Login");
                return;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                if (this.turnManager.IsOver)
                {
                    return;
                }

                /*
                // check if we ran out of time, in which case we loose
                if (turnEnd<0f && !IsShowingResults)
                {
                        Debug.Log("Calling OnTurnCompleted with turnEnd ="+turnEnd);
                        OnTurnCompleted(-1);
                        return;
                }
                */


                //LogFeedback(this.turnManager.Turn.ToString());
                if (this.turnManager.Turn > 0)
                {

                    LogFeedback("Turn Time: " + this.turnManager.RemainingSecondsInTurn.ToString("F1") + " SECONDS");

                    this.GamePanel.SetActive(true);
                }


            }

            //if user hits, escape close application
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }

            this.UpdatePlayerTexts();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Login");
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
        public void GameBegins()
        { 
            if (PhotonNetwork.IsMasterClient)
            {
                turnManager.BeginTurn();
                Debug.Log(PhotonNetwork.CurrentRoom.GetTurn());
                this.BeginGame.SetActive(false);
                gameStarted = true;
            }
            else
            {
                gameStarted = false;
                LogFeedback("Wait for the Master Client to Start the Game"); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void MakeTurn()
        {
            LogFeedback(" Making Move " + turnManager.Turn); 
            SetScore();
            int Score = PhotonNetwork.LocalPlayer.GetScore();

            this.turnManager.SendMove(Score, true);
            turnManager.BeginTurn();
            this.UpdatePlayerTexts();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnEndTurn()
        {
            this.GameBegins();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="turn"></param>
        public void OnTurnBegins(int turn)
        {
            LogFeedback("Turn " + turn);
            Debug.Log("Turn Begins...");
            Debug.Log("Turn " + turn);
            this.UpdatePlayerTexts();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="turn"></param>
        public void OnTurnCompleted(int turn)
        {
            Debug.Log("Turn Completed...");
            Debug.Log("Turn " + turn);
            this.SetScore();
            this.UpdatePlayerTexts();
        }

        public void OnPlayerMove(Player player, int turn, object move)
        {
            Debug.Log("On Player Move...");
            Debug.Log("Turn " + turn);

            this.SetScore();
            this.UpdatePlayerTexts();
        }

        /// <summary>
        /// Not being called. In test mode
        /// </summary>
        /// <param name="player"></param>
        /// <param name="turn"></param>
        /// <param name="move"></param>
        public void OnPlayerFinished(Player player, int turn, object move)
        {
            Debug.Log("On Player Finished...");
            Debug.Log("Turn " + turn);
            LogFeedback("Score " + move);
            this.SetScore();
            this.UpdatePlayerTexts();
        }

        /// <summary>
        /// Not being called. In test mode
        /// </summary>
        /// <param name="turn"></param>
        public void OnTurnTimeEnds(int turn)
        {
            Debug.Log("Turn Time Ends...");
            Debug.Log("Turn " + turn);
            this.SetScore();
            this.UpdatePlayerTexts();
        }

        public void SetScore()
        {

            InputField inputField = ScoreObject.GetComponent<InputField>();
            string value = inputField.text;
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Score is empty");
                return;
            }

            try
            {
                PhotonNetwork.LocalPlayer.SetScore(int.Parse(value));
            }
            catch (Exception ex)
            {
                PhotonNetwork.LocalPlayer.SetScore(0);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePlayerTexts()
        {
            PlayerList.text = "";
            Player LocalPlayer = PhotonNetwork.LocalPlayer;
            //PlayerList.text += LocalPlayer.NickName + " (" + LocalPlayer.GetScore() + ")" + System.Environment.NewLine;
            foreach (Player otherone in PhotonNetwork.PlayerList)
            {
              //  if (LocalPlayer.NickName != otherone.NickName)
                {
                    PlayerList.text += otherone.NickName + " (score: " + otherone.GetScore() + ")" + System.Environment.NewLine;
                }
 

            }

            //show for current player
            if (myDiceValue != null)
            {
                myDiceValue.text = LocalPlayer.GetScore().ToString();
            }
        }
    }
}
