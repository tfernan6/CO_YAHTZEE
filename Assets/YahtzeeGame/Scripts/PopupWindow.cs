using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

using edu.jhu.co;

public class PopupWindow : MonoBehaviour
{
    /*public GameObject popupWindowObject;
    public Button yesButton;
    public Button noButton;
    public Text popupMessage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPopupWindow(string message, Score score)
    {
        popupWindowObject.SetActive(true);
        yesButton.onClick.AddListener(YesClicked);
        noButton.onClick.AddListener(noClicked);
        popupMessage.text = message;
    }

    //include all logic to set score to 0 and reset counter and endturn
    public void YesClicked()
    {
        popupWindowObject.SetActive(false);
        Debug.Log("Yes Clicked");

        isSelected = true;
        scoreValue = 0;
        updateScoreText();
        // code to change image color to reflect that score is chosen

        this.transform.Find("Borderline").gameObject.GetComponent<Image>().color = new Color32(0, 115, 16, 255);
        transcriptController.SendMessageToTranscript("Selected Score of " + this.GetComponent<TMP_Text>().text + " for " + gameObject.name + " Slot",
        TranscriptMessage.SubsystemType.score);
        transcriptController.SendMessageToTranscript("Turn complete", TranscriptMessage.SubsystemType.turn);

        diceController.resetRollCounter();

        scorecard.calculateSum();
        scorecard.calculateTotal();
        Debug.Log("Score has been selected");
        diceController.resetDice();

        photonView.RPC("updateOtherClients", RpcTarget.Others, this.transform.parent.transform.Find("playerName").gameObject.GetComponent<TMP_Text>().text,
           gameObject.name, scoreValue);

        scorecard.clearUnselectedScores();

        SetTurnIsDone();
    }

    public void noClicked()
    {
        popupWindowObject.SetActive(false);
        Debug.Log("No Clicked");
    }*/
}
