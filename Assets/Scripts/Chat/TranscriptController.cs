using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TranscriptController : MonoBehaviour
{
    public static int maxMessages = 1000000;
    public GameObject chatPanel, textObject;
    [SerializeField] List<TranscriptMessage> messageList = new List<TranscriptMessage>();
    public Color login, chat, dice, scorecard, score, turn, player, transcript, game;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChatController chatController = GameObject.Find("ChatController").GetComponent<ChatController>();
        if (!chatController.chatBox.isFocused && Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToTranscript("You pressed the space key!", TranscriptMessage.SubsystemType.game);
        }
    }

    public void SendMessageToTranscript(string text, TranscriptMessage.SubsystemType subsystemType)
    {

        if (messageList.Count >= maxMessages) 
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }        
        TranscriptMessage newMessage = new TranscriptMessage();
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = TranscriptMessageTypeColor(subsystemType);

        messageList.Add(newMessage);
    }

    Color TranscriptMessageTypeColor(TranscriptMessage.SubsystemType subsystemType)
    {
        Color color = login;

        switch (subsystemType)
        {
            case TranscriptMessage.SubsystemType.chat:
                color = chat;
                break;
            case TranscriptMessage.SubsystemType.dice:
                color = dice;
                break;
            case TranscriptMessage.SubsystemType.scorecard:
                color = scorecard;
                break;   
            case TranscriptMessage.SubsystemType.score:
                color = score;
                break;  
            case TranscriptMessage.SubsystemType.turn:
                color = turn;
                break;
            case TranscriptMessage.SubsystemType.player:
                color = player;
                break;
            case TranscriptMessage.SubsystemType.transcript:
                color = transcript;
                break;
            case TranscriptMessage.SubsystemType.game:
                color = game;
                break;
        }

        return color;
    }
}

[System.Serializable]
public class TranscriptMessage
{
    public string text;
    public Text textObject;
    public SubsystemType subsystemType;

    public enum SubsystemType
    {
        login,
        chat,
        dice,
        scorecard,
        score,
        turn, 
        player, 
        transcript,
        game
    }
}