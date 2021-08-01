using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

using edu.jhu.co;

public class DiceController : MonoBehaviour
{
    public Sprite[] diceImages = new Sprite[6];
    public Die[] diceObjects = new Die[5];
    public static int rollCounter = 3;
    public Scorecard scorecard;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //grabs the transcript controller and populates the transcript controller
        /* if (transcriptController == null &&
             GameObject.Find("TranscriptController") != null)
         {
             transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
         }
         else
         {
             Debug.Log("Transcript controller is null");
         }


         if (GameObject.Find("ScorecardController") != null)
         {
             scorecardController = GameObject.Find("ScorecardController").GetComponent<ScorecardController>();
         }*/
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
            //rollCounter -= 1;
        }
        else
        {
           /* if (transcriptController != null)
            {
                transcriptController.SendMessageToTranscript("No more rerolls! Please select a score to end your turn"
                , TranscriptMessage.SubsystemType.dice);
            }*/
        }

        /*if (transcriptController != null)
        {
            transcriptController.SendMessageToTranscript("Rerolled Dice -- Rolls Left: " + rollCounter, TranscriptMessage.SubsystemType.dice);
        }
        if (scorecardController != null)
        {
            scorecardController.calculateScores();
        }*/

        //multiplayer
        /*Text DiceValue = GameObject.Find("DiceValueText").GetComponent<Text>();
        if (DiceValue != null) { DiceValue.text = ReturnDiceRolled().ToString(); }

        if (!allowForSinglePerson)
        { 
            bool turnOver = (rollCounter > 0) ? false : true;
            this.turnManager.SendMove(System.Convert.ToInt32(DiceValue.text), turnOver);  //change to correct value
            if (!turnOver) //if all 3 are over, your turn is over
            {
                this.turnManager.BeginTurn(); //your turn over, next player to move
            }
        }*/
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
