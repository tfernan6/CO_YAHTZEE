using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

using edu.jhu.co;

public class Score : MonoBehaviour
{
    //this class will contain all of the game logic to calculate the scores based on dice values

    public bool isSelected = false;
    public int scoreValue = 0;

    public PopupWindow popupWindow;

    private DiceController diceController;
    private Die[] currentDice = new Die[5];
    private GameManager gameManager;
    private static TranscriptController transcriptController;
    public Scorecard scorecard;
    private Dictionary<int, int> diceValueCount = new Dictionary<int, int>()
        {
                {1, 0 },
                {2, 0 },
                {3, 0 },
                {4 ,0 },
                {5, 0 },
                {6, 0 }
        };
    private Dictionary<string, int> UpperScoreKey = new Dictionary<string, int>()
        {
            {"Ones", 1 },
            {"Twos", 2 },
            {"Threes", 3},
            {"Fours", 4 },
            {"Fives", 5 },
            {"Sixes", 6 }
        };
    public PhotonView photonView;

    void Start()
    {
        diceController = GameObject.Find("DiceController").GetComponent<DiceController>();
        scorecard = this.transform.parent.gameObject.GetComponent<Scorecard>();
        currentDice = diceController.diceObjects;
        transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
        gameManager = GameObject.Find("GameRoomObject").GetComponent<GameManager>();
        photonView = this.GetComponent<PhotonView>();
    }
    public void selectScore()
    {
        //include logic later that asks user to confirm if they want to select a score that is empty i.e. 0;
        //adjust logic to allow selection of 0 scores but add a prompt asking user if they are sure

        //include logic where only your Score can be locked by you
        if (diceController.rollCounter < 3) {
            if (gameManager.currentTurnPlayer == scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text)
            {
                if (PhotonNetwork.LocalPlayer.NickName == scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text)
                {
                    if (gameObject.name != "Sum" && gameObject.name != "Bonus" && gameObject.name != "Total Score")
                    {
                        if (!isSelected && !string.IsNullOrEmpty(this.GetComponent<TMP_Text>().text))
                        {
                            isSelected = true;

                            // code to change image color to reflect that score is chosen

                            this.transform.Find("Borderline").gameObject.GetComponent<Image>().color = new Color32(0, 115, 16, 255);
                            transcriptController.SendMessageToTranscript("Selected Score of " + this.GetComponent<TMP_Text>().text + " for " + gameObject.name + " Slot",
                                TranscriptMessage.SubsystemType.score);
                            transcriptController.SendMessageToTranscript("Turn complete", TranscriptMessage.SubsystemType.turn);

                            diceController.resetRollCounter();
                            scorecard.calculateSum();
                            scorecard.calculateTotal();
                            Debug.Log("Score has been selected");
                            diceController.resetDice();

                            photonView.RPC("updateOtherClients", RpcTarget.Others, this.transform.parent.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text,
                                gameObject.name, scoreValue);

                            scorecard.clearUnselectedScores();

                            SetTurnIsDone();
                        }
                        if (!isSelected && string.IsNullOrEmpty(this.GetComponent<TMP_Text>().text))
                        {
                            isSelected = true;
                            scorecard.popupWindowObject.GetComponent<PopupWindow>().OpenPopupWindow("Are you sure you want to score 0 in this category?", this);
                            Debug.Log("Empty box is clicked");
                        }
                    }
                }
            }
        }
    }
    //include all logic to set score to 0 and reset counter and endturn
    /*public void YesClicked()
    {
        popupWindowObject.SetActive(false);
        Debug.Log("Yes Clicked");

        isSelected = true;
        scoreValue = 0;
        updateScoreText();
        // code to change image color to reflect that score is chosen

        this.transform.Find("Borderline").gameObject.GetComponent<Image>().color = new Color32(0, 115, 16, 255);
        transcriptController.SendMessageToTranscript("Selected Score of " + this.GetComponent<TMP_Text>().text + " for " + gameObject.name + " Slot",
        TranscriptMessage.SubsystemType.score);
        transcriptController.SendMessageToTranscript("Turn complete", TranscriptMessage.SubsystemType.turn);

        diceController.resetRollCounter();

        scorecard.calculateSum();
        scorecard.calculateTotal();
        Debug.Log("Score has been selected");
        diceController.resetDice();

        photonView.RPC("updateOtherClients", RpcTarget.Others, this.transform.parent.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text,
           gameObject.name, scoreValue);

        scorecard.clearUnselectedScores();

        SetTurnIsDone();
    }

    public void noClicked(){
        popupWindowObject.SetActive(false);
        Debug.Log("No Clicked");
    }
*/
    public void SetTurnIsDone()
    {
        if (gameManager != null)
        {
            gameManager.CompleteTurn();
        }
    }

    public void calculateUpperScore()
    {
        getDiceValueCount();
        int tempScore = 0;
        if (!isSelected)
        {
            if (UpperScoreKey.ContainsKey(gameObject.name))
            {
                foreach (Die die in currentDice)
                {
                    if (die.dieValue == 7)
                    {
                        continue;
                    }
                    if (die.dieValue == UpperScoreKey[gameObject.name])
                    {
                        tempScore += + die.dieValue;
                    }
                    this.scoreValue = tempScore;
                }
                if (this.scoreValue != 0)
                {
                    updateScoreText();
                }
                else
                {
                    blankOutScore();
                }

            }
           

            //add logic that if everything but Yahtzee is filled out, you have to select Yahtzee and get a 0

            //begininng of special Yahtzee logic

        }
    }

    public void calculateLowerScore()
    {
        Debug.Log("Calculating Lower Scores");
        getDiceValueCount();
        int tempScore = 0;
        if (!isSelected)
        {
            if (gameObject.name == "Three of a Kind")
            {
                if (diceValueCount.ContainsValue(3))
                {

                    foreach (Die die in currentDice)
                    {
                        tempScore += die.dieValue;
                    }
                    this.scoreValue = tempScore;
                    updateScoreText();
                }
                else
                {
                    blankOutScore();
                }
            }
            if (gameObject.name == "Four of a Kind")
            {
                if (diceValueCount.ContainsValue(4))
                {
                    foreach (Die die in currentDice)
                    {
                        tempScore += die.dieValue;
                    }
                    this.scoreValue = tempScore;
                    updateScoreText();
                }
                else
                {
                    blankOutScore();
                }
            }
            if (gameObject.name == "Full House")
            {
                if (diceValueCount.ContainsValue(3) && diceValueCount.ContainsValue(2))
                {
                    this.scoreValue = 25;
                    updateScoreText();
                }
                else
                {
                    blankOutScore();
                }
            }
            if (gameObject.name == "Small Straight")
            {
                if ((diceValueCount[1] >= 1 && diceValueCount[2] >= 1 && diceValueCount[3] >= 1 && diceValueCount[4] >= 1) |
                    (diceValueCount[2] >= 1 && diceValueCount[3] >= 1 && diceValueCount[4] >= 1 && diceValueCount[5] >= 1) |
                    (diceValueCount[3] >= 1 && diceValueCount[4] >= 1 && diceValueCount[5] >= 1 && diceValueCount[6] >= 1))
                {
                    this.scoreValue = 30;
                    updateScoreText();
                }
                else
                {
                    blankOutScore();
                }
            }
            if (gameObject.name == "Large Straight")
            {
                if ((diceValueCount[1] >= 1 && diceValueCount[2] >= 1 && diceValueCount[3] >= 1 && diceValueCount[4] >= 1 && diceValueCount[5] >= 1) |
                    (diceValueCount[2] >= 1 && diceValueCount[3] >= 1 && diceValueCount[4] >= 1 && diceValueCount[5] >= 1 && diceValueCount[6] >= 1))
                {
                    this.scoreValue = 40;
                    updateScoreText();
                }
                else
                {
                    blankOutScore();
                }
            }
            if (gameObject.name == "Chance")
            {
                foreach (Die die in currentDice)
                {
                    tempScore += die.dieValue;
                }
                this.scoreValue = tempScore;
                updateScoreText();
            }
        }
        if (gameObject.name == "YAHTZEE")
        {
            //have to implement that if you get Yahtzee again, you can select the YAHTZEE score
            if (diceValueCount.ContainsValue(5))
            {
                this.isSelected = false;
                Debug.Log("YAHTZEE");
                if (this.scoreValue == 0)
                {
                    if (isSelected)
                    {
                        joker();
                    }
                    this.scoreValue = 50;
                }
                else if (this.scoreValue == 50)
                {
                    Debug.Log("double Yahtzee!");
                    this.scoreValue += 100;
                    joker();
                }
                updateScoreText();
            }
            else
            {
                if(this.scoreValue > 0)
                {
                    isSelected = true;
                } else
                {
                    blankOutScore();
                }
               
            }
        }
    }

    private void getDiceValueCount()
    {
        List<int> keys = new List<int>(diceValueCount.Keys);
        
        //reset diceValueCount
        foreach (int key in keys)
        {
            diceValueCount[key] = 0;
        }
        //loop to populate diceCountValue dictionary
        foreach (Die die in currentDice)
        {
            diceValueCount[die.dieValue] = diceValueCount[die.dieValue] + 1;
        }
    }

    public void updateScoreText()
    {
        this.GetComponent<TMP_Text>().text = scoreValue.ToString();
    }

    public void blankOutScore()
    {
        this.scoreValue = 0;
        this.GetComponent<TMP_Text>().text = null;
    }

    public void joker()
    {
        //tells turn manager that we have a joker
    }

    public void updateSummaryScoresForOthers()
    {
        photonView.RPC("updateOtherClients", RpcTarget.All, this.transform.parent.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text,
                            gameObject.name, scoreValue);
    }

    //add logic to update the entire scorecard. Can we call this 16 times on the Scorecard class?
    [PunRPC]
    private void updateOtherClients(string playerName, string scoreType, int scoreValue)
    {
        if (this.transform.parent.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text == playerName)
        {
            this.transform.parent.transform.Find(scoreType).gameObject.GetComponent<Score>().scoreValue = scoreValue;
        }
        updateScoreText();
    }

   
}
