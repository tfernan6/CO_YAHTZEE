using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

using edu.jhu.co;

public class Die : MonoBehaviour
{
    public bool isHold = false;
    public int dieValue = 7;
    public Sprite[] diceImages = new Sprite[7];
    private PhotonView photonView;
    private static TranscriptController transcriptController;
    private Toggle toggle;

    void Start()
    {
        photonView = this.GetComponent<PhotonView>();
        toggle = GetComponent<Toggle>();
    }

    public void toggleDie()
    {
        if (dieValue != 7) 
        { 
            isHold = !isHold;
            transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
            this.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn = isHold;
            photonView.RPC("updateToggleUIforOthers", RpcTarget.All, isHold);

            if (isHold)
            {
                transcriptController.SendMessageToTranscript("Holding Die", TranscriptMessage.SubsystemType.dice);

            }
            if(!isHold)
            {
                transcriptController.SendMessageToTranscript("Releasing Die", TranscriptMessage.SubsystemType.dice);
            }
        }
    }
    public void rollDie()
    {
        if (!isHold)
        {
            dieValue = Random.Range(1, 7);
            updateDiceSprite();
            photonView.RPC("updateDieValueforOthers", RpcTarget.All, dieValue);
        }
    }

    private void updateDiceSprite()
    {
        this.GetComponent<Image>().sprite = diceImages[dieValue - 1];
    }


    public void resetDie()
    {
        //dieValue of 7 represents blank die
        dieValue = 7;
        isHold = false;
        updateDiceSprite();
        updateToggleUI();
        photonView.RPC("updateToggleUIforOthers", RpcTarget.All, isHold);
    }

    public void updateToggleUI()
    {
        this.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn = isHold;
    }

    //allows other clients to update the local client's dice when they roll
    [PunRPC]
    private void updateDieValueforOthers(int newDieValue)
    {
        dieValue = newDieValue;
        updateDiceSprite();
    }

    [PunRPC]
    private void updateToggleUIforOthers(bool isOn)
    {
        isHold = isOn;
        updateToggleUI();
    }

    public void callPunRPCUpdateUI()
    {
        photonView.RPC("updateDieValueforOthers", RpcTarget.All, dieValue);
    }

}
