using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

using edu.jhu.co;

public class Die : MonoBehaviour
{
    public bool isHold = false;
    public int dieValue = 1;
    public Sprite[] diceImages = new Sprite[6];
    public PhotonView photonView;

    void Start()
    {
        photonView = this.GetComponent<PhotonView>();
    }

    public void toggleDice()
    {
        isHold = !isHold;
      /*  TranscriptController transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
        

        if (isHold)
        {
            transcriptController.SendMessageToTranscript("Holding Die", TranscriptMessage.SubsystemType.dice);

        }
        if(!isHold)
        {
            transcriptController.SendMessageToTranscript("Releasing Die", TranscriptMessage.SubsystemType.dice);
        }*/
    }
    public void rollDie()
    {
        if (!isHold)
        {
            Debug.Log("rolling dice");
            dieValue = Random.Range(1, 7);
            updateDiceSprite();
            photonView.RPC("updateDiceforOthers", RpcTarget.All, dieValue);
        }
    }

    public void updateDiceSprite()
    {
        this.GetComponent<Image>().sprite = diceImages[dieValue - 1];
    }

    //allows other clients to update the local client's dice when they roll
    [PunRPC]
    void updateDiceforOthers(int newDieValue)
    {
        Debug.Log("RPC running");
        dieValue = newDieValue;
        updateDiceSprite();
    }

}
