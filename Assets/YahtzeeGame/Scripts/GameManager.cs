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
        public Text CurrentPlayerName;


        [Tooltip("The Ui Text to inform the user PlayerList")]
        [SerializeField]
        public Text PlayerList;

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
        public Button RollDiceButton;


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


        //private YahtzeePlayer yahtzeePlayer = null; //Player info of current player ToDo: save state
        private bool gameStarted = false;  //has the game begun? //do we need this? can't turnmanager have this info?

        //creating controller objects
        public ScoreboardController sbController;
        public DiceController diceController;
        private static TranscriptController transcriptController;

        //public List<YahtzeePlayer> yahtzeePlayers = new List<YahtzeePlayer>();
        public List<string> playerNameList;
        private static Player YtzPlayer;

        //I would need to derive from this class and construct my own
        public PunTurnManager turnManager;
        private PhotonView photonView;
        public string currentTurnPlayer;
        public List<string> winners = new List<string>();

        public Text WinnerText;

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


        /// <summary>
        /// 
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            try
            {
                //FIRST CALLS to instantiate the turnmanager and photonview
                //define the turn manager
                this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
                this.turnManager.TurnManagerListener = this;
                turnManager.TurnDuration = 15f; // 5seconds

                //set the photon view and punturnmanager
                //  this.photonView = this.gameObject.AddComponent<PhotonView>();
                // this.photonView.InstantiationId = 101;
                this.photonView = this.gameObject.GetComponent<PhotonView>();

                if (sbController == null &&
                    GameObject.Find("ScoreboardController") != null)
                {
                    sbController = GameObject.Find("ScoreboardController").GetComponent<ScoreboardController>();
                }
                else
                {
                    Debug.Log("Scoreboard controller is null");
                }

                if (transcriptController == null &&
                    GameObject.Find("TranscriptController") != null)
                {
                    transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
                }
                else
                {
                    Debug.Log("Transcript controller is null");
                }

                if (diceController == null &&
                   GameObject.Find("DiceController") != null)
                {
                    diceController = GameObject.Find("DiceController").GetComponent<DiceController>();
                }


                //connect to Photon NEtwork if it's not already been connected
                if (!PhotonNetwork.IsConnected)
                {
                    if (transcriptController != null)
                        transcriptController.SendMessageToTranscript("Loaded login scene", TranscriptMessage.SubsystemType.game);
                    SceneManager.LoadScene("Login");
                    return;
                }

                //get handle of RollDiceButton
                if (RollDiceButton == null)
                    RollDiceButton = GameObject.Find("RollDiceButton").GetComponent<Button>();

                if(WinnerText != null)
                {
                    WinnerText.enabled = false;
                }
                //set the welcome and game status messages
                SetWelcomeText();

                //create player object for current player
               /* if (yahtzeePlayer == null)
                {
                    yahtzeePlayer = new YahtzeePlayer();
                    //yahtzeePlayers.Add(yahtzeePlayer);

                }
                yahtzeePlayer.CurrentPlayerName = PhotonNetwork.NickName;*/
                Debug.Log("Current Player: " + PhotonNetwork.LocalPlayer.NickName);

                //update the list of players in the left panel
                CurrentPlayerName.text = PhotonNetwork.NickName;
                this.UpdatePlayerList();

                if (transcriptController != null)
                    transcriptController.SendMessageToTranscript("Added player " + CurrentPlayerName.text + " to player list", TranscriptMessage.SubsystemType.game);


                //enable begin game cntrol only for the first person
                if (PhotonNetwork.IsMasterClient)
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
                this.DicePanel.SetActive(false);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// Update is called once per frame
        /// this is what reflects on each client
        /// </summary>
        void Update()
        {
            try
            {

               /* if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
                {
                    SceneManager.LoadScene("Login");
                    return;
                } */


                //if more than one player, set turns to play
                if (PhotonNetwork.CurrentRoom != null &&
                    PhotonNetwork.CurrentRoom.PlayerCount >= 1 &&
                    this.turnManager != null &&
                    this.turnManager.Turn != 0)
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

                    //ToDo: have everyone finished their turn?
                    if (this.turnManager.IsCompletedByAll) //never hits true!!!????
                    {
                        //Checking the status of turn being completed by all players (Round finished!)
                        Debug.Log("Finished the Round");
                        this.turnManager.BeginTurn();
                    }
                    else if (this.turnManager.Turn > 0 && !this.turnManager.IsCompletedByAll)
                    {
                        Debug.Log("Not Finished the Round");
                    }


                    LogTurnTime(this.turnManager.RemainingSecondsInTurn.ToString("F1"));
                    LogTurnDiceRollStatus(); 
 
                    // update list to all players
                    this.UpdatePlayerList();
               }


                //if user hits, escape close application
                if (Input.GetKey("escape"))
                {
                    if (transcriptController != null)
                        transcriptController.SendMessageToTranscript("Exiting the application", TranscriptMessage.SubsystemType.game);
                    Application.Quit();
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void LogTurnDiceRollStatus()
        {
            Player LocalPlayer = PhotonNetwork.LocalPlayer;
            DiceController localDiceController = GameObject.Find("DiceController").GetComponent<DiceController>();

            string rollsLeft =  "You have " + localDiceController.rollCounter + " rolls. ";

            if (PhotonNetwork.PlayerList.Length > 1 && 
                this.turnManager.Turn <= 13)
            {
                Player nextPlayer = (YtzPlayer != null) ? YtzPlayer.GetNext() : PhotonNetwork.LocalPlayer.GetNext(); //should be the next player in turn
                if (nextPlayer != null)
                {
                    LogFeedback(System.Environment.NewLine
                        + "<i><color=orange>You have 3 rolls in a turn. Select a value to end your turn</color> </i>" + System.Environment.NewLine
                        + " [Turn:  " + this.turnManager.Turn.ToString() + "] "
                        + "Next player to play is: <b>" + nextPlayer.NickName + "</b>"
                        );
                }
            }
            else if(PhotonNetwork.PlayerList.Length > 1 &&
                this.turnManager.Turn > 13) //Game is over
            {
                WinnerText.enabled = true;
                this.TurnStatus.text = string.Empty;

                List<string> weHaveAWinner = sbController.checkGameConcluded();
                LogFeedback("Game has ended. The winner is " + weHaveAWinner[0] );
                RollDiceButton.interactable = false;
            }

        }

        public void endGame()
        {
            string temp = "";
            if (winners.Count == 1) {
                LogFeedback(System.Environment.NewLine + "<i><color=green>The winner is </color> </i>" + winners[0]);
            } else
            {
                foreach (string winner in winners) {
                    if (temp == "")
                    {
                        temp = winner;
                    } else
                    {
                        temp = temp + ", " + winner;
                    }
                }
                LogFeedback(System.Environment.NewLine + "<i><color=green>The winners are </color> </i>" + temp);
            }
        }

        [PunRPC]
        private void selectDiceForPlayer(Player playerToPlay)
        {
            bool enable = (PhotonNetwork.LocalPlayer == playerToPlay) ? true : false;
            if(RollDiceButton != null)
            {
                RollDiceButton.interactable = enable;
            }
            currentTurnPlayer = playerToPlay.NickName;
            YtzPlayer = playerToPlay;
            IndicatePlayingPlayer(YtzPlayer);
        }
        /// <summary>
        /// called whenever the dice are rolled
        /// </summary>
        public void OnDiceRoll()
        {
            Debug.Log("My roll counter value" + diceController.rollCounter.ToString());
            LogTurnDiceRollStatus();
            diceController.rollDice(); 


            //your 3 rolls are complete
            if (diceController.rollCounter <= 0) 
            {
                RollDiceButton.interactable = false;
                //CompleteTurn();
            }
            else if(diceController.rollCounter == 3) //begin turn for next player(check if ur turn is complete)
            {
                turnManager.BeginTurn(); //set for next turn, here or elsewhere to indicate next turn
            }
            else if (diceController.rollCounter > 3) //don't know why it goes beyond 3 (error handling code)
            {
                diceController.resetRollCounter();
                RollDiceButton.interactable = false;
            }

        }

        public void RefreshPanels()
        {
            
            
        }

        #region MultiPlayerCalls

        /// <summary>
        /// caled when local user leaves room
        /// </summary>
        public override void OnLeftRoom()
        {
            try
            {
                SceneManager.LoadScene("Login");
                base.OnLeftRoom();
            }
            catch(System.Exception ex)
            {
               // Debug.LogException(ex);
            }
        }

        /// <summary>
        /// called when remote player leaves room
        /// </summary>
        /// <param name="other"></param>
        public override void OnPlayerLeftRoom(Player other)
        {
            try
            {

                if (PhotonNetwork.PlayerList.Length > 1)
                {
                    Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
                    LogFeedback("Player " + other.NickName + " left the Game");

                    //check if the player who left is next player to play
                    if(YtzPlayer == other)
                    {
                        YtzPlayer = other.GetNext();
                    }
                    this.UpdatePlayerList();
                }
            }
            catch (System.Exception ex)
            {
               // Debug.LogException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public override void OnPlayerEnteredRoom(Player other)
        {
            try
            {
               // Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // seen when other connects 
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
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }

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
                //PhotonNetwork.LocalPlayer.SetScore(0);
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

            //display players in left panel and in chat window
            foreach (Player otherone in PhotonNetwork.PlayerList)
            {
                //no score to show so just show player name
                if (otherone == YtzPlayer) //indicator
                {
                    PlayerList.text = PlayerList.text + "<color=orange>" + otherone.NickName + "</color>" + System.Environment.NewLine;
                }
                else
                {
                    PlayerList.text += otherone.NickName + System.Environment.NewLine;
                }

                //list maintained by scoreboard
                if (! playerNameList.Contains(otherone.NickName))
                {
                    print("adding player " + otherone.NickName + " to playerNameList");
                    playerNameList.Add(otherone.NickName);
                }
            }
            sbController.assignScorecards(); //free any allocated memory since this method is caleld on update of this page
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


        #endregion

        #region PageEventHandlers
        /// <summary>
        /// leave game and go to login page
        /// </summary>
        public void LeaveRoom()
        {
            try
            {
                if (transcriptController != null)
                    transcriptController.SendMessageToTranscript("Player left the room", TranscriptMessage.SubsystemType.game);

                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer); //UNLOAD ALL PREFAB CONTROLLERS
                PhotonNetwork.LeaveRoom(); 
            }
            catch (System.Exception ex)
            {
               // Debug.Log(ex.Message);
            }
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
                YtzPlayer = PhotonNetwork.LocalPlayer;
                photonView.RPC("selectDiceForPlayer", RpcTarget.All, PhotonNetwork.LocalPlayer);


                gameStarted = true;
                 turnManager.BeginTurn(); //set your turn


                //need to check all paths
                Debug.Log(PhotonNetwork.CurrentRoom.GetTurn());

                //begun so don't let anyone to click begin anymore
                this.BeginGame.SetActive(false);
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
        /// <param name="player"></param>
        public void IndicatePlayingPlayer(Player YtzPlayer)
        {
            //display players in left panel and in chat window
            PlayerList.text = "";
            foreach (Player otherone in PhotonNetwork.PlayerList)
            {
                //no score to show so just show player name
                //no score to show so just show player name
                if (otherone == YtzPlayer) //indicator
                {
                    PlayerList.text = PlayerList.text + "<color=orange>" + otherone.NickName + "</color>" + System.Environment.NewLine;
                }
                else
                {
                    PlayerList.text += otherone.NickName + System.Environment.NewLine;
                }
            }

        }

        /// <summary>
        /// player commited his/her turn
        /// </summary>
        public void CompleteTurn()
        {
            //inform the turnmanager (ToDo: pass score value)
            ScoreValue = 10;//dummy value, pass actual value for that roll from Score class
            
            if (diceController != null) { diceController.resetRollCounter(); }
            this.turnManager.SendMove(ScoreValue, true);  //pass value and say my turn is over (need to be called by score)
            if (RollDiceButton != null) { RollDiceButton.interactable = false; }
        }
        #endregion

        #region PunTurnManagerCallbacks
        /// <summary>
        /// 
        /// </summary>
        public void OnEndTurn()
        {
            sbController.checkGameConcluded();
            LogFeedback("On End Turn");

        }

        /// <summary>
        /// Called when the next turn begins
        /// </summary>
        /// <param name="turn">Turn Index</param>
        public void OnTurnBegins(int turn)
        {
            //if turnManager.Turn > 13, game has ended. Stop the game!!!
            List<string> weHaveAWinner = sbController.checkGameConcluded(); //teena

            //allow to play
            if (this.turnManager.Turn <= 13 && 
                PhotonNetwork.PlayerList.Length == 1)
            {
                RollDiceButton.interactable = true; //its just me, so keep it enabled
            }
            else if (this.turnManager.Turn > 13)
            {
                //game has ended
                this.LogFeedback("Game has ended");
                RollDiceButton.interactable = false;
            }
            
   
        }

        /// <summary>
        /// Called when  turn is complete -  finished by all players 
        /// </summary>
        /// <param name="turn">Turn Index</param>
        public void OnTurnCompleted(int turn)
        {
            LogFeedback("Turn " + turn + " completed");
            Debug.Log("Turn " + turn);
            this.SetScore();


            //set next turn
            this.turnManager.BeginTurn(); 
        }

        /// <summary>
        /// Called when a player moved (but did not finish the turn). Never comes here, because not called
        /// </summary>
        /// <param name="player">Player reference</param>
        /// <param name="turn">Turn Index</param>
        /// <param name="move">Move Object data</param>
        public void OnPlayerMove(Player player, int turn, object move)
        {
            Debug.Log("OnPlayerMove: " + player + " turn: " + turn + " action: " + move);
            //   LogFeedback("OnPlayerMove: " + player.NickName + " turn: " + turn + " action: " + move);
            YtzPlayer = player;
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
           // LogFeedback("PlayerFinished: " + player.NickName + " turn: " + turn + ". turn completed by all?: " + this.turnManager.IsCompletedByAll);

            //enable dice roll for the next
            if (PhotonNetwork.PlayerList.Length > 1 &&
                photonView != null) //multi player
            {
                Player nextPlayer = player.GetNext();
                photonView.RPC("selectDiceForPlayer", RpcTarget.All, nextPlayer); 
            }

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
