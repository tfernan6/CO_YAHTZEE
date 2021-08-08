using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

using edu.jhu.co;
public class Scorecard : MonoBehaviour
{
    private static TranscriptController transcriptController;
    public Score[] scores = new Score[16];
    public Score[] upperScores = new Score[6];
    public Score[] lowerScores = new Score[7];
    public Score[] summaryScores = new Score[3];
    public ScoreboardController sbController;
    public string playerName;
    private PhotonView photonView;

    /*public YahtzeePlayer yahtzeePlayer;*/

    //should contain logic for the bonus points

    // Start is called before the first frame update         
    void Start()
    {
        
        transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
        sbController = GameObject.Find("ScoreboardController").GetComponent<ScoreboardController>();

        upperScores[0] = this.transform.Find("Ones").gameObject.GetComponent<Score>();
        upperScores[1] = this.transform.Find("Twos").gameObject.GetComponent<Score>();
        upperScores[2] = this.transform.Find("Threes").gameObject.GetComponent<Score>();
        upperScores[3] = this.transform.Find("Fours").gameObject.GetComponent<Score>();
        upperScores[4] = this.transform.Find("Fives").gameObject.GetComponent<Score>();
        upperScores[5] = this.transform.Find("Sixes").gameObject.GetComponent<Score>();
       
        lowerScores[0] = this.transform.Find("Three of a Kind").gameObject.GetComponent<Score>();
        lowerScores[1] = this.transform.Find("Four of a Kind").gameObject.GetComponent<Score>();
        lowerScores[2] = this.transform.Find("Full House").gameObject.GetComponent<Score>();
        lowerScores[3] = this.transform.Find("Small Straight").gameObject.GetComponent<Score>();
        lowerScores[4] = this.transform.Find("Large Straight").gameObject.GetComponent<Score>();
        lowerScores[5] = this.transform.Find("Chance").gameObject.GetComponent<Score>();
        lowerScores[6] = this.transform.Find("YAHTZEE").gameObject.GetComponent<Score>();
        
        summaryScores[0] = this.transform.Find("Sum").gameObject.GetComponent<Score>();
        summaryScores[1] = this.transform.Find("Bonus").gameObject.GetComponent<Score>();
        summaryScores[2] = this.transform.Find("Total Score").gameObject.GetComponent<Score>();

        photonView = this.GetComponent<PhotonView>();

        playerName =
        this.GetComponent<TMP_Text>().text = playerName;

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void calculateScores()
    {
        if (PhotonNetwork.LocalPlayer.NickName == playerName)
        {
            //transcriptController.SendMessageToTranscript("Calculating Scores", TranscriptMessage.SubsystemType.scorecard);
            foreach (Score score in upperScores)
            {
                score.calculateUpperScore();
            }

            foreach (Score score in lowerScores)
            {
                score.calculateLowerScore();
            }
        }
    }

    public void calculateSum()
    {
        int tempScore = 0;
        foreach (Score score in upperScores)
        {
            if (score.isSelected)
            {
                tempScore += score.scoreValue;
            }
            summaryScores[0].scoreValue = tempScore;
            if (summaryScores[0].scoreValue > 0)
            {
                summaryScores[0].updateScoreText();
            }
            else
            {
                summaryScores[0].blankOutScore();
            }
        }
        if (summaryScores[0].scoreValue >= 63)
        {
            summaryScores[1].scoreValue = 35;
            summaryScores[1].updateScoreText();
        }
    }
    
    public void calculateTotal()
    {
        summaryScores[2].scoreValue = summaryScores[0].scoreValue + summaryScores[1].scoreValue;
        foreach (Score score in lowerScores)
        {
           if (score.isSelected)
            {
                summaryScores[2].scoreValue += score.scoreValue;
            }
        }
        summaryScores[2].updateScoreText();
    }
}
