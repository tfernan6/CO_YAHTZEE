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

        //Roll dice button
        [Tooltip("Roll Dice ")]
        [SerializeField]
        public GameObject RollDiceButton;


        //This is panel where the scoreboard will be placed,
        [Tooltip("ScoreboardPanel")]
        [SerializeField]
        public GameObject ScoreboardPanel;

        //This is panel where the dices will be placed,
        [Tooltip("DicePanel")]
        [SerializeField]
        public GameObject DicePanel;

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

        //not used
        private int ScoreValue = 0;


        private YahtzeePlayer yahtzeePlayer = null; //Player info of current player ToDo: save state
        private bool gameStarted = false;  //has the game begun? //do we need this? can't turnmanager have this info?

        //creating controller objects
        public ScoreboardController sbController;
        public DiceController diceController;
        private static TranscriptController transcriptController;
        //public List<YahtzeePlayer> yahtzeePlayers = new List<YahtzeePlayer>();
        //public Player[] photonPlayerList;

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
            if (transcriptController == null &&
                GameObject.Find("TranscriptController") != null)
            {
                transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
            }
            else
            {
                Debug.Log("Transcript controller is null");
            }

            if (!PhotonNetwork.IsConnected)
            {
                if (transcriptController != null)
                    transcriptController.SendMessageToTranscript("Loaded login scene", TranscriptMessage.SubsystemType.game);
                SceneManager.LoadScene("Login");
                return;
            }

            //set the welcome and game status messages
            SetWelcomeText();

            //create player object for current player
            if(yahtzeePlayer == null)
            {
                yahtzeePlayer = new YahtzeePlayer();
                //yahtzeePlayers.Add(yahtzeePlayer);

            }
            yahtzeePlayer.CurrentPlayerName = PhotonNetwork.NickName;
            Debug.Log("Current Player: " + PhotonNetwork.NickName);

            //update the list of players in the left panel
            CurrentPlayerName.text = PhotonNetwork.NickName;
            if (transcriptController != null)
                transcriptController.SendMessageToTranscript("Added player " + CurrentPlayerName.text +  " to player list", TranscriptMessage.SubsystemType.game);
            this.UpdatePlayerList();


            //define the turn manager
            this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
            this.turnManager.TurnManagerListener = this;

            // duration of the turn
            turnManager.TurnDuration = 5f; // 5seconds


            //enable begin game cntrol only for the first person
            if (PhotonNetwork.IsMasterClient)
            // && (PhotonNetwork.PlayerList.Length > 1 ))
            {
                if (transcriptController != null)
                    transcriptController.SendMessageToTranscript("Begin Game enabled", TranscriptMessage.SubsystemType.game);
                this.BeginGame.SetActive(true);

            }
            else
            {
                //display that the first person has to press start
                this.BeginGame.SetActive(false);
            }

            //Panel for dice and scoreboard turn disable till game clicked
            this.ScoreboardPanel.SetActive(false);
           // this.RollDiceButton.SetActive(false);
            this.DicePanel.SetActive(false);

            //initiates controller objects

            /*sbController = GameObject.Find("ScoreboardController").GetComponent<ScoreboardController>();
            tsController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
            diceController = GameObject.Find("DiceController").GetComponent<DiceController>();*/
            //photonPlayerList = PhotonNetwork.PlayerList;




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
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 1 && this.turnManager.Turn != 0)
            {
               this.ScoreboardPanel.SetActive(true);
               this.DicePanel.SetActive(true);

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

                //ToDo: if it is not your turn, you cannot roll the dice

                //turn counter keeps incrementing, it does not reset after all players have completed (is this an issue?)
                LogTurnStatus(turnManager.Turn);

                //LogFeedback(this.turnManager.Turn.ToString());
                LogTurnTime(this.turnManager.RemainingSecondsInTurn.ToString("F1"));
            }

 
            //if user hits, escape close application
            if (Input.GetKey("escape"))
            {
                if (transcriptController != null)
                    transcriptController.SendMessageToTranscript("Exiting the application", TranscriptMessage.SubsystemType.game);
                Application.Quit();
            }

            //updateplayer list
            this.UpdatePlayerList();
        }

        /// <summary>
        /// called whenever the dice are rolled
        /// </summary>
        public void OnDiceRoll()
        {
            Debug.Log("My roll counter value" + diceController.rollCounter.ToString());
            diceController.rollDice(); // or get value you want


            //your 3 rolls are complete
            if (diceController.rollCounter == 0) 
            {
                MakeTurn();
                diceController.resetRollCounter();
            }

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
            if (transcriptController != null)
                transcriptController.SendMessageToTranscript(other.NickName + " entered the room", TranscriptMessage.SubsystemType.game);


            //enable begin game if you have one player
            if (PhotonNetwork.IsMasterClient &&
                gameStarted == false)
                //&& PhotonNetwork.PlayerList.Length > 1 ) 
            {
                //game not begun)
                if (transcriptController != null)
                    transcriptController.SendMessageToTranscript("Begin Game enabled", TranscriptMessage.SubsystemType.game);
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
            //set game player's score
            try
            {
                //  PhotonNetwork.LocalPlayer.SetScore(no set value yet);

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
            
            //update the player list in the scoreboard
            PlayerNames.text = "";
            Player LocalPlayer = PhotonNetwork.LocalPlayer;

           //update the player list in chat dropdown
            Dropdown chatPlayerList = GameObject.Find("DropdownPlayers").GetComponent<Dropdown>();
            if (chatPlayerList != null)
            {
                
            }
            //

            //display players in left panel and in chat window
           // PlayerList.text = PhotonNetwork.LocalPlayer.NickName + System.Environment.NewLine;
            foreach (Player otherone in PhotonNetwork.PlayerList)
            {
                //no score to show so just show player name
                // PlayerList.text += otherone.NickName + " (score: " + otherone.GetScore() + ")" + System.Environment.NewLine;
                PlayerList.text += otherone.NickName + System.Environment.NewLine;

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
            GameStatus.text = message;
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
            }

            //display game room name
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

        /// <summary>
        /// Log the turn feedback for testing
        /// </summary>
        /// <param name="turn"></param>
        private void LogTurnStatus(int turn)
        {
            //turn counter keeps incrementing, it does not reset after all players have completed (is this an issue or the way turnmanager is set?)
            int playerOrderInTurn = (turn % PhotonNetwork.PlayerList.Length == 0) ?
                        PhotonNetwork.PlayerList.Length : (turn % PhotonNetwork.PlayerList.Length);
            string playerWhoseTurnItIs = string.Empty;
            if ((playerOrderInTurn - 1 >= 0) || (playerOrderInTurn <= PhotonNetwork.PlayerList.Length)) //error check
            {
                playerWhoseTurnItIs = PhotonNetwork.PlayerList[playerOrderInTurn - 1].NickName;
            }

            //actual round is turn / PhotonNetwork.PlayerList.Length
            int round = (PhotonNetwork.PlayerList.Length == 1) ? turn : ((turn / PhotonNetwork.PlayerList.Length) + 1);

            //log whose turn it is
            LogFeedback("Round: " + round + " Turn: " + turn + " of " + playerWhoseTurnItIs);
            Debug.Log("Turn Begins " + turn);

        }
        #endregion

        #region PageEventHandlers
        /// <summary>
        /// leave game and go to login page
        /// </summary>
        public void LeaveRoom()
        {
            if (transcriptController != null)
                transcriptController.SendMessageToTranscript("Player left the room", TranscriptMessage.SubsystemType.game);
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Login");
        }

        /// <summary>
        /// close window (remove for web build)
        /// </summary>
        public void ExitApplication()
        {
            if (transcriptController != null)
                transcriptController.SendMessageToTranscript("Clicked \"Exit Application\"", TranscriptMessage.SubsystemType.game);
            Application.Quit();
        }

        /// <summary>
        /// User clicked to begin the game
        /// set player turn
        /// </summary>
        public void GameBegins()
        { 
             //display gaming area
            if (transcriptController != null)
                transcriptController.SendMessageToTranscript("Clicked \"Begin Game\"", TranscriptMessage.SubsystemType.game);
            this.ScoreboardPanel.SetActive(true);
            this.DicePanel.SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                 gameStarted = true;
                 turnManager.BeginTurn(); //set your turn


                //need to check all paths
                Debug.Log(PhotonNetwork.CurrentRoom.GetTurn());

                //begun so don't let anyone to click begin anymore
                this.BeginGame.SetActive(false);
              //  this.RollDiceButton.SetActive(true);
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
            //inform the turnmanager (ToDo: pass score value)
            ScoreValue = 10;//dummy value
             //set score of player here to save
            SetScore();

            this.turnManager.SendMove(ScoreValue, true); //pass value and say my turn is over
            turnManager.BeginTurn(); //set for next turn, here or elsewhere to indicate next turn
        }
        #endregion

        #region PunTurnManagerCallbacks
        /// <summary>
        /// 
        /// </summary>
        public void OnEndTurn()
        {

        }

        /// <summary>
        /// Called when the player's turn begins
        /// </summary>
        /// <param name="turn">Turn Index</param>
        public void OnTurnBegins(int turn)
        {
            LogTurnStatus(turn);
            //set the play to next person

            //ToDo: if it is not your turn you cannot roll the dice
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
            this.SetScore();
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
        }
        #endregion
    }
}
