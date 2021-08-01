using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scorecard : MonoBehaviour
{
    private static TranscriptController transcriptController;
    public Score[] scores = new Score[16];

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
}
