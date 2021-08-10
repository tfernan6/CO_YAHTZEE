using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using edu.jhu.co;

public class DiceController : MonoBehaviour
{
    public Die[] diceObjects = new Die[5];
    public  int rollCounter = 3;
    public ScoreboardController sbController;
    private GameManager gameManager;
    private static TranscriptController transcriptController;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //grabs the transcript controller and populates the transcript controller
        if (transcriptController == null &&
              GameObject.Find("TranscriptController") != null)
          {
              transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
          }
          else
          {
              Debug.Log("Transcript controller is null");
          }

        diceObjects[0] = GameObject.Find("Die0").GetComponent<Die>();
        diceObjects[1] = GameObject.Find("Die1").GetComponent<Die>();
        diceObjects[2] = GameObject.Find("Die2").GetComponent<Die>();
        diceObjects[3] = GameObject.Find("Die3").GetComponent<Die>();
        diceObjects[4] = GameObject.Find("Die4").GetComponent<Die>();

        if (GameObject.Find("ScoreboardController") != null)
        {
            sbController = GameObject.Find("ScoreboardController").GetComponent<ScoreboardController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void rollDice()
    {
        if (transcriptController != null)
            transcriptController.SendMessageToTranscript("Rolled dice. " + rollCounter + " rolls left in turn", TranscriptMessage.SubsystemType.dice);
        if (rollCounter >= 1)
        {
            for (int i = 0; i < diceObjects.Length; i++)
            {
                diceObjects[i].rollDie();
            }

            //commenting out counter for testing
            rollCounter -= 1;
        }
        else
        {
            if (transcriptController != null)
            {
                transcriptController.SendMessageToTranscript("No more rerolls! Please select a score to end your turn"
                , TranscriptMessage.SubsystemType.dice);
            }
        }

/*        if (transcriptController != null)
        {
            transcriptController.SendMessageToTranscript("Rerolled Dice -- Rolls Left: " + rollCounter, TranscriptMessage.SubsystemType.dice);
        }*/

        //calculate scores for all the scorecards in the scoreboard
        foreach (Scorecard scorecard in sbController.scorecards)
        {
            scorecard.calculateScores();
        }
        
    }


    /// <summary>
    /// Added this to test dice for multiplayer logic
    /// has no logic pertaining to Yahtzee
    /// </summary>
    /// <returns></returns>
    private int ReturnDiceRolled()
    {
        int diceRolled = 0;

        foreach (Die die in this.diceObjects)
        {
            diceRolled = diceRolled + die.dieValue;
        }
         return diceRolled;
    }

    public void resetRollCounter()
    {
        rollCounter = 3;
    }
    //include UI to show number of rolls left

}
