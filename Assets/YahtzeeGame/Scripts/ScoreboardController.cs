using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

using edu.jhu.co;


    public class ScoreboardController : MonoBehaviour
    {

    // For testing purposes, will grab this from GameManager
    public Scorecard[] scorecards;
    public GameManager gameManager;
    private static TranscriptController transcriptController;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameRoomObject").GetComponent<GameManager>();
        if (transcriptController == null &&
            GameObject.Find("TranscriptController") != null)
        {
            transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
        }
        else
        {
            Debug.Log("Transcript controller is null");
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    //need to update so that it will reflect the current players (currently will just keep adding per player)
    public void assignScorecards()
    {
        foreach (string playerName in gameManager.playerNameList)
        {
            print("playername is " + playerName);
            foreach (Scorecard scorecard in scorecards)
            {
                print(scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text);
                if (scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text == playerName)
                {
                    break;
                }

                if (scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text == "N/A")
                {
                    print("Scorecard ownership set to " + playerName) ;
                    scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text = playerName;
                    break;
                }
            }
        }
    }

    public List<string> checkGameConcluded()
    {
        List<string> weHaveAWinner = new List<string>();
        if (gameManager.turnManager.Turn > 13) {
            weHaveAWinner = gameManager.winners = determineWinner();
            gameManager.endGame();
        }

        return weHaveAWinner;
    }

    public string GetPlayerWithHighestScore()
    {
        string player = string.Empty;

        int highScore = scorecards[0].summaryScores[2].scoreValue;
        player = scorecards[0].transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text;
        foreach (Scorecard scorecard in scorecards)
        {
            if (scorecard.summaryScores[2].scoreValue > highScore)
            {
                highScore = scorecard.summaryScores[2].scoreValue;
                player = scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text;
            }
        }
        return player;
    }

    public List<string> determineWinner()
    {
        int topScore = 0;
        List<string> currentWinner = new List<string>();
        bool tie = false;

        foreach (Scorecard scorecard in scorecards)
        {
            if (scorecard.summaryScores[2].scoreValue > topScore)
            {
                if (tie)
                {
                    currentWinner.Clear();
                }
                topScore = scorecard.summaryScores[2].scoreValue;
                currentWinner.Add(scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text);
            }
            else if (scorecard.summaryScores[2].scoreValue == topScore)
            {
                currentWinner.Add(scorecard.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text);
            }
        }

        return currentWinner;
    }
}


