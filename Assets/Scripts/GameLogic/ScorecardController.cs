using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorecardController : MonoBehaviour
{
    private static TranscriptController transcriptController;
    public Score[] scoreList = new Score[16];
    
    //should contain logic for the bonus points

    // Start is called before the first frame update
    void Start()
    {
        transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
        scoreList[0] = GameObject.Find("Ones").GetComponent<Score>();
        scoreList[1] = GameObject.Find("Twos").GetComponent<Score>();
        scoreList[2] = GameObject.Find("Threes").GetComponent<Score>();
        scoreList[3] = GameObject.Find("Fours").GetComponent<Score>();
        scoreList[4] = GameObject.Find("Fives").GetComponent<Score>();
        scoreList[5] = GameObject.Find("Sixes").GetComponent<Score>();
        scoreList[6] = GameObject.Find("Sum").GetComponent<Score>();
        scoreList[7] = GameObject.Find("Bonus").GetComponent<Score>();
        scoreList[8] = GameObject.Find("Three of a Kind").GetComponent<Score>();
        scoreList[9] = GameObject.Find("Four of a Kind").GetComponent<Score>();
        scoreList[10] = GameObject.Find("Full House").GetComponent<Score>();
        scoreList[11] = GameObject.Find("Small Straight").GetComponent<Score>();
        scoreList[12] = GameObject.Find("Large Straight").GetComponent<Score>();
        scoreList[13] = GameObject.Find("Chance").GetComponent<Score>();
        scoreList[14] = GameObject.Find("Yahtzee").GetComponent<Score>();
        scoreList[15] = GameObject.Find("Total Score").GetComponent<Score>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void calculateScores()
    {
        transcriptController.SendMessageToTranscript("Calculating Scores", TranscriptMessage.SubsystemType.scorecard);
        foreach (Score score in scoreList)
        {
            score.resetDiceValueCount();
            score.calculateScore();
        }
    }
}
