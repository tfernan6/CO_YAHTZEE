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


        [Tooltip("The Ui Text to inform the user about the connection progress")]
        [SerializeField]
        private Text GameStatus;

        [Tooltip("The welcome text on top of the game board")]
        [SerializeField]
        private Text WelcomeText;


        [Tooltip("The room name text on top of the game board")]
        [SerializeField]
        private Text GameRoomText;


        [Tooltip("The timer text on top of the game board")]
        [SerializeField]
        private Text TurnStatus;


        private YahtzeePlayer yahtzeePlayer = null; //not coded for it yet
        public bool gameStarted = false;  //maybe not needed(SHould be in the Turn)
        public bool allowForSinglePerson = true; //allows for singleperson to be played

        //creating controller objects
        public ScoreboardController sbController;
        public DiceController diceController;
        public TranscriptController tsController;
        public List<YahtzeePlayer> yahtzeePlayers = new List<YahtzeePlayer>();
        public Player[] photonPlayerList;

        //show my dice value (test variable)
        public Text myDiceValue;

        //I would need to derive from this class and construct my own
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

        /// <summary>
        /// 
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            //connect to Photon NEtwork if it's not already been connected
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Login");
                return;
            }

            //set the welcome and game status messages
            SetWelcomeText();

            //create player object for current player
            if(yahtzeePlayer == null)
            {
                yahtzeePlayer = new YahtzeePlayer();
                yahtzeePlayers.Add(yahtzeePlayer);

            }
            yahtzeePlayer.CurrentPlayerName = PhotonNetwork.NickName;
            Debug.Log("Current Player: " + PhotonNetwork.NickName);

            //update the list of players in the left panel
            CurrentPlayerName.text = PhotonNetwork.NickName;
            this.UpdatePlayerList();


            //define the turn manager
            this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
            this.turnManager.TurnManagerListener = this;

            // duration of the turn
            turnManager.TurnDuration = 5f; // 5seconds


            //enable begin game cntrol only for the first person
            /* if (PhotonNetwork.IsMasterClient )
                // && (PhotonNetwork.PlayerList.Length > 1 || allowForSinglePerson))
             {
                 this.BeginGame.SetActive(true);

             }
             else
             {
                 //display that the first person has to press start
                 this.BeginGame.SetActive(false);
             }*/
            this.BeginGame.SetActive(true);

            //Panel for dice turn disable till game clicked
            this.GamePanel.SetActive(false);

             //initiates controller objects
             //diceController = GameObject.Find("DiceController").GetComponent<DiceController>();
             sbController = GameObject.Find("ScoreboardController").GetComponent<ScoreboardController>();
             tsController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
             photonPlayerList = PhotonNetwork.PlayerList;




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

            //if more than one player, set turns to play
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

                //allow for single player
                this.GamePanel.SetActive(true);
                //need to check all paths


                //LogFeedback(this.turnManager.Turn.ToString());
                if (this.turnManager.Turn > 0)
                {
                    LogTurnTime(this.turnManager.RemainingSecondsInTurn.ToString("F1"));

                    this.GamePanel.SetActive(true);
                }


            }

            //if user hits, escape close application
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }

            //updateplayer list
            this.UpdatePlayerList();
        }

        //called whenever the dice are rolled
        public void OnDiceRoll()
        {
            /*Text DiceValue = GameObject.Find("DiceValueText").GetComponent<Text>();
            if (DiceValue != null) { DiceValue.text = ReturnDiceRolled().ToString(); }

            this.turnManager.SendMove(System.Convert.ToInt32(DiceValue.text), turnOver);  //change to correct value
            if (diceController.rollCounter < 1) 
            {
                this.turnManager.BeginTurn(); //your turn over, next player to move
            }*/

        }

        public void RefreshPanels()
        {
            
            
        }

        #region MultiPlayerCalls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public override void OnPlayerLeftRoom(Player other)
        {
            /*Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            LogFeedback("Player " + other.NickName + " left the Game");
            this.UpdatePlayerList();*/
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
                gameStarted == false)
                //&& PhotonNetwork.PlayerList.Length > 1 ) 
            {
                //game not begun)
                this.BeginGame.SetActive(true);
            }

            this.UpdatePlayerList();
        }
        #endregion

        #region GameManagerFunctions
        /// <summary>
        /// 
        /// </summary>
        public void SetScore()
        {
            //get value of score 
            Text DiceValue = GameObject.Find("DiceValueText").GetComponent<Text>();
            string value = DiceValue.text;
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Score is empty");
                return;
            }


            //set game player's score
            try
            {
                PhotonNetwork.LocalPlayer.SetScore(int.Parse(value));

                //also set tht his turn for this round is complete
                

            }
            catch (Exception ex)
            {
                PhotonNetwork.LocalPlayer.SetScore(0);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// 

        public void UpdatePlayerList()
        {
            PlayerList.text = "";
            Player LocalPlayer = PhotonNetwork.LocalPlayer;
            //PlayerList.text += LocalPlayer.NickName + " (" + LocalPlayer.GetScore() + ")" + System.Environment.NewLine;

           //update the player list in chat dropdown
            Dropdown chatPlayerList = GameObject.Find("DropdownPlayers").GetComponent<Dropdown>();
            if (chatPlayerList != null)
            {
                
            }
            //

            //display players in left panel and in chat window
            foreach (Player otherone in PhotonNetwork.PlayerList)
            {
                PlayerList.text += otherone.NickName + " (score: " + otherone.GetScore() + ")" + System.Environment.NewLine;

            }
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
        /// </summary>
        /// <param name="message"></param>
        public void SetWelcomeText()
        {
            if (WelcomeText != null)
            {
                WelcomeText.text = "Welcome " + PhotonNetwork.NickName + " to play Yahtzee++";
                            //+ System.Environment.NewLine + "You are in " + PhotonNetwork.CurrentRoom.Name;
            }

            if (GameRoomText != null)
            {
                GameRoomText.text = PhotonNetwork.CurrentRoom.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogTurnTime(string message)
        {
            // we do not assume there is a feedbackText defined.
            if (TurnStatus == null)
            {
                return;
            }

            // add new messages as a new line and at the bottom of the log.
            TurnStatus.text = "[Turn Time: " + message + "secs]";
        }
        #endregion

        #region PageEventHandlers
        /// <summary>
        /// leave game and go to login page
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Login");
        }

        /// <summary>
        /// close window (remove for web build)
        /// </summary>
        public void ExitApplication()
        {
            Application.Quit();
        }

        /// <summary>
        /// cliecked to begin the game
        /// </summary>
        public void GameBegins()
        { 
            if (PhotonNetwork.IsMasterClient)
            {
                turnManager.BeginTurn();

                //for running single player mode
                this.GamePanel.SetActive(true);
                //need to check all paths

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
        /// player commited his/her turn
        /// </summary>
        public void MakeTurn()
        {
            LogFeedback(" Making Move " + turnManager.Turn); 

            //get dice value selected
            int Score = 0;
            Text _diceValueText = this.GetComponent<Text>();
            if (_diceValueText != null)
            {
                Score = Convert.ToInt32(_diceValueText.text);
            }
            SetScore();

         //   int Score = PhotonNetwork.LocalPlayer.GetScore();

            //inform the turnmanager
            this.turnManager.SendMove(Score, true);
            turnManager.BeginTurn();
            this.UpdatePlayerList();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void OnEndTurn()
        {
            this.GameBegins();
        }

        /// <summary>
        /// Called the turn begins event.
        /// </summary>
        /// <param name="turn">Turn Index</param>
        public void OnTurnBegins(int turn)
        {
            LogFeedback("Turn " + turn);
            Debug.Log("Turn Begins...");
            Debug.Log("Turn " + turn);
            this.UpdatePlayerList();
        }

        /// <summary>
        /// Called when a turn is completed (finished by all players)
        /// </summary>
        /// <param name="turn">Turn Index</param>
        public void OnTurnCompleted(int turn)
        {
            Debug.Log("Turn Completed...");
            Debug.Log("Turn " + turn);
            this.SetScore();
            this.UpdatePlayerList();
        }

        /// <summary>
        /// Called when a player moved (but did not finish the turn)
        /// </summary>
        /// <param name="player">Player reference</param>
        /// <param name="turn">Turn Index</param>
        /// <param name="move">Move Object data</param>
        public void OnPlayerMove(Player player, int turn, object move)
        {
            Debug.Log("On Player Move...");
            Debug.Log("Turn " + turn);

            this.SetScore();
            this.UpdatePlayerList();
        }

        /// <summary>
        /// When a player finishes a turn (includes the action/move of that player)
        /// </summary>
        /// <param name="player">Player reference</param>
        /// <param name="turn">Turn index</param>
        /// <param name="move">Move Object data</param>
        public void OnPlayerFinished(Player player, int turn, object move)
        {
            Debug.Log("On Player Finished...");
            Debug.Log("Turn " + turn);
            LogFeedback("Score " + move);
            this.SetScore();
            this.UpdatePlayerList();
        }

        /// <summary>
        /// Called when a turn completes due to a time constraint (timeout for a turn)
        /// </summary>
        /// <param name="turn">Turn index</param>
        public void OnTurnTimeEnds(int turn)
        {
            Debug.Log("Turn Time Ends...");
            Debug.Log("Turn " + turn);
            this.SetScore();
            this.UpdatePlayerList();
        }
    }
}
