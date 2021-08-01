using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scorecard : MonoBehaviour
{
    private static TranscriptController transcriptController;
    public Score[] scores = new Score[16];
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

    /*public YahtzeePlayer yahtzeePlayer;*/

    //should contain logic for the bonus points

    // Start is called before the first frame update         
    void Start()
    {
        transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();

        scores[0] = this.transform.Find("Ones").gameObject.GetComponent<Score>();
        scores[1] = this.transform.Find("Twos").gameObject.GetComponent<Score>();
        scores[2] = this.transform.Find("Threes").gameObject.GetComponent<Score>();
        scores[3] = this.transform.Find("Fours").gameObject.GetComponent<Score>();
        scores[4] = this.transform.Find("Fives").gameObject.GetComponent<Score>();
        scores[5] = this.transform.Find("Sixes").gameObject.GetComponent<Score>();
        scores[6] = this.transform.Find("Sum").gameObject.GetComponent<Score>();
        scores[7] = this.transform.Find("Bonus").gameObject.GetComponent<Score>();
        scores[8] = this.transform.Find("Three of a Kind").gameObject.GetComponent<Score>();
        scores[9] = this.transform.Find("Four of a Kind").gameObject.GetComponent<Score>();
        scores[10] = this.transform.Find("Full House").gameObject.GetComponent<Score>();
        scores[11] = this.transform.Find("Small Straight").gameObject.GetComponent<Score>();
        scores[12] = this.transform.Find("Large Straight").gameObject.GetComponent<Score>();
        scores[13] = this.transform.Find("Chance").gameObject.GetComponent<Score>();
        scores[14] = this.transform.Find("YAHTZEE").gameObject.GetComponent<Score>();
        scores[15] = this.transform.Find("Total Score").gameObject.GetComponent<Score>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void calculateScores()
    {
        //transcriptController.SendMessageToTranscript("Calculating Scores", TranscriptMessage.SubsystemType.scorecard);
        foreach (Score score in scores)
        {
            score.calculateScore();
        }

    }
/*    public void calculateScore(Score score)
    {

        //figure out how to take out this function from the Score class and into the Scorecard class
        scoreValue = 0;
        Debug.Log("Score value is " + scoreValue.ToString());
        if (!isSelected)
        {
            if (UpperScoreKey.ContainsKey(gameObject.name))
            {
                foreach (Die die in currentDice)
                {
                    if (die.dieValue == UpperScoreKey[gameObject.name])
                    {
                        scoreValue = scoreValue + die.dieValue;
                    }
                }
                if (scoreValue != 0)
                {
                    this.GetComponent<TMP_Text>().text = scoreValue.ToString();
                }
                else
                {
                    this.GetComponent<TMP_Text>().text = null;
                }

            }
            if (gameObject.name == "Three of a Kind")
            {
                if (diceValueCount.ContainsValue(3))
                {

                    foreach (Die die in currentDice)
                    {
                        scoreValue = scoreValue + die.dieValue;
                    }
                    this.GetComponent<TMP_Text>().text = scoreValue.ToString();
                }
                else
                {
                    this.GetComponent<TMP_Text>().text = null;
                }
            }
            if (gameObject.name == "Four of a Kind")
            {
                if (diceValueCount.ContainsValue(4))
                {
                    foreach (Die die in currentDice)
                    {
                        scoreValue = scoreValue + die.dieValue;
                    }

                    this.GetComponent<TMP_Text>().text = scoreValue.ToString();
                }
                else
                {
                    this.GetComponent<TMP_Text>().text = null;
                }
            }
            if (gameObject.name == "Full House")
            {
                if (diceValueCount.ContainsValue(3) && diceValueCount.ContainsValue(2))
                {
                    scoreValue = 25;
                    this.GetComponent<TMP_Text>().text = scoreValue.ToString();
                }
                else
                {
                    this.GetComponent<TMP_Text>().text = null;
                }
            }

            if (gameObject.name == "Small Straight")
            {
                if ((diceValueCount[1] >= 1 && diceValueCount[2] >= 1 && diceValueCount[3] >= 1 && diceValueCount[4] >= 1) |
                    (diceValueCount[2] >= 1 && diceValueCount[3] >= 1 && diceValueCount[4] >= 1 && diceValueCount[5] >= 1) |
                    (diceValueCount[3] >= 1 && diceValueCount[4] >= 1 && diceValueCount[5] >= 1 && diceValueCount[6] >= 1))
                {
                    scoreValue = 30;
                    this.GetComponent<TMP_Text>().text = scoreValue.ToString();
                }
                else
                {
                    this.GetComponent<TMP_Text>().text = null;
                }
            }
            if (gameObject.name == "Large Straight")
            {
                if ((diceValueCount[1] >= 1 && diceValueCount[2] >= 1 && diceValueCount[3] >= 1 && diceValueCount[4] >= 1 && diceValueCount[5] >= 1) |
                    (diceValueCount[2] >= 1 && diceValueCount[3] >= 1 && diceValueCount[4] >= 1 && diceValueCount[5] >= 1 && diceValueCount[6] >= 1))
                {
                    scoreValue = 40;
                    this.GetComponent<TMP_Text>().text = scoreValue.ToString();
                }
                else
                {
                    this.GetComponent<TMP_Text>().text = null;
                }
            }
            if (gameObject.name == "Chance")
            {
                foreach (Die die in currentDice)
                {
                    scoreValue = scoreValue + die.dieValue;
                }
                this.GetComponent<TMP_Text>().text = scoreValue.ToString();
            }

            if (gameObject.name == "YAHTZEE")
            {
                if (diceValueCount.ContainsValue(5))
                {
                    scoreValue = 50;
                    this.GetComponent<TMP_Text>().text = scoreValue.ToString();
                }
                else
                {
                    this.GetComponent<TMP_Text>().text = null;
                }
            }

            //add logic that if everything but Yahtzee is filled out, you have to select Yahtzee and get a 0
            //adjust logic to allow selection of 0 scores but add a prompt asking user if they are sure
        }
    }
*/
/*    private void resetDiceValueCount()
    {
        List<int> keys = new List<int>(diceValueCount.Keys);
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
*/

}
