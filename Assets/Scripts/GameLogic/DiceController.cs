using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour
{
    public Sprite[] diceImages = new Sprite[6];
    public Die[] diceObjects = new Die[5];
    public static int rollCounter = 3;
    private static TranscriptController transcriptController;
    public ScorecardController scorecardController;

    // Start is called before the first frame update
    void Start()
    {
        //grabs the transcript controller and populates the transcript controller
        transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
        scorecardController = GameObject.Find("ScorecardController").GetComponent<ScorecardController>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < diceObjects.Length; i++)
        {
            diceObjects[i].GetComponent<Image>().sprite = diceImages[diceObjects[i].dieValue-1];
        }
    }

    public void rollDice()
    {
        if (rollCounter >= 1) {
            for (int i = 0; i < diceObjects.Length; i++)
            {
                if (diceObjects[i].isHold == false)
                {
                    diceObjects[i].dieValue = Random.Range(1, 7);

                }
            }
            rollCounter -= 1;
        }
        else {
            transcriptController.SendMessageToTranscript("No more rerolls! Please select a score to end your turn"
                , TranscriptMessage.SubsystemType.dice);
        }

        transcriptController.SendMessageToTranscript("Rerolled Dice -- Rolls Left: " + rollCounter, TranscriptMessage.SubsystemType.dice);
        scorecardController.calculateScores();
    }


    public void resetRollCounter()
    {
        rollCounter = 3;
    }
    //include UI to show number of rolls left
}
