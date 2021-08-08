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
}


