using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class ChatController : MonoBehaviour
{
    public static int maxMessages = 1000;

    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public Color playerMessage, info;
    public string username;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    // Start is called before the first frame update
    void Start()
    {
        //username = OldLogin.playerName;
        username = PhotonNetwork.NickName;
    }

    // Update is called once per frame
    void Update()
    {

        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                TranscriptController transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
                SendMessageToChat(username + ": " + chatBox.text, Message.MessageType.playerMessage);
                

                transcriptController.SendMessageToTranscript(username + " sent a message in the chat: " + chatBox.text, TranscriptMessage.SubsystemType.chat);
                chatBox.text = "";
            }
        }
        else 
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {   
                chatBox.ActivateInputField();
            }
        }

        /*
        if (!chatBox.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
             SendMessageToChat("You pressed the space key!", Message.MessageType.info);
             Debug.Log("Space");
            }
        }
        */
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {

        if (messageList.Count >= maxMessages) 
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }        
        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        messageList.Add(newMessage);
    }

    Color MessageTypeColor(Message.MessageType messageType)
    {
        Color color = info;

        switch (messageType)
        {
            case Message.MessageType.playerMessage:
                color = playerMessage;
                break;
        }

        return color;
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage,
        info
    }

}