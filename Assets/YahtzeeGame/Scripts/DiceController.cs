using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using edu.jhu.co;

public class DiceController : MonoBehaviour
{
    public Die[] diceObjects = new Die[5];
    private const int RollTries = 4; //for somereason, when you make it 3 turn ends after 2, codes needs check to reset this to 3
    public  int rollCounter = RollTries;
    public Scorecard scorecard;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //grabs the transcript controller and populates the transcript controller
        /*  if (transcriptController == null &&
              GameObject.Find("TranscriptController") != null)
          {
              transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
          }
          else
          {
              Debug.Log("Transcript controller is null");
          }*/

        diceObjects[0] = GameObject.Find("Die0").GetComponent<Die>();
        diceObjects[1] = GameObject.Find("Die1").GetComponent<Die>();
        diceObjects[2] = GameObject.Find("Die2").GetComponent<Die>();
        diceObjects[3] = GameObject.Find("Die3").GetComponent<Die>();
        diceObjects[4] = GameObject.Find("Die4").GetComponent<Die>();

        if (GameObject.Find("Scorecard") != null)
        {
            scorecard = GameObject.Find("Scorecard").GetComponent<Scorecard>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void rollDice()
    {
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
           /* if (transcriptController != null)
            {
                transcriptController.SendMessageToTranscript("No more rerolls! Please select a score to end your turn"
                , TranscriptMessage.SubsystemType.dice);
            }*/
        }

/*        if (transcriptController != null)
        {
            transcriptController.SendMessageToTranscript("Rerolled Dice -- Rolls Left: " + rollCounter, TranscriptMessage.SubsystemType.dice);
        }*/

        //need to update this logic when we have scoreboard implemented.
        if (scorecard != null)
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
        rollCounter = RollTries;
    }
    //include UI to show number of rolls left

}
