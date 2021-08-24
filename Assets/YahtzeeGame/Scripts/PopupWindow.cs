using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

using edu.jhu.co;

public class PopupWindow : MonoBehaviour
{
    public GameObject popupWindowObject;
    public Button yesButton;
    public Button noButton;
    public Text popupMessage;
    private DiceController diceController;
    private static TranscriptController transcriptController;

    // Start is called before the first frame update
    void Start()
    {
        diceController = GameObject.Find("DiceController").GetComponent<DiceController>();
        transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenPopupWindow(string message, Score score)
    {
        popupWindowObject.SetActive(true);
        yesButton.onClick.AddListener(delegate {YesClicked(score);});
        noButton.onClick.AddListener(noClicked);
        popupMessage.text = message;
    }

    //include all logic to set score to 0 and reset counter and endturn
    public void YesClicked(Score score)
    {
        popupWindowObject.SetActive(false);
        Debug.Log("Yes Clicked");

        score.isSelected = true;
        score.scoreValue = 0;
        score.updateScoreText();

        // code to change image color to reflect that score is chosen
        score.gameObject.transform.Find("Borderline").gameObject.GetComponent<Image>().color = new Color32(0, 115, 16, 255);
        transcriptController.SendMessageToTranscript("Selected Score of " + score.gameObject.GetComponent<TMP_Text>().text + " for " + gameObject.name + " Slot",
        TranscriptMessage.SubsystemType.score);
        transcriptController.SendMessageToTranscript("Turn complete", TranscriptMessage.SubsystemType.turn);

        diceController.resetRollCounter();

        score.scorecard.calculateSum();
        score.scorecard.calculateTotal();
        Debug.Log("Score has been selected");
        diceController.resetDice();

        score.updateSummaryScoresForOthers();
        score.scorecard.clearUnselectedScores();
        score.SetTurnIsDone();
    }

    public void noClicked()
    {
        popupWindowObject.SetActive(false);
        Debug.Log("No Clicked");
    }
}
