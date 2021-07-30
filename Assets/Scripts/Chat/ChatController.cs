using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using ExitGames.Client.Photon;


public class ChatController : MonoBehaviour, IChatClientListener
{
    public static int maxMessages = 1000;
    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public Color playerMessage, info;
    [SerializeField] string username;
    ChatClient chatClient;
    bool isConnected;
    string currentChat;
    string privateReceiver = "";


    // callbacks
    public void DebugReturn(DebugLevel level, string message)
    {
        throw new System.NotImplementedException();
    }
    public void OnChatStateChange(ChatState state)
    {
        throw new System.NotImplementedException();
    }
    public void OnConnected() {
        UnityEngine.Debug.Log("Connection to chat successful");
        chatClient.Subscribe(new string[] {"RegionChannel"});
    }
    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }
    public void OnGetMessages(string channelName, string[] senders, object[] messages) 
    {
        throw new System.NotImplementedException();
    }
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatPanel.SetActive(true);
    }
    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }
    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void UsernameOnValueChange(string valueIn)
    {
        username = valueIn;
    }

    public void ChatConnect() 
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
        UnityEngine.Debug.Log("Connection to chat");
    }

    public void TypeChatOnValueChange(string valueIn)
    {
        currentChat = valueIn;
    }

    public void SubmitPublicChatOnClick()
    {
        if (privateReceiver == "" && currentChat != "") {
            chatClient.PublishMessage("RegionChannel", currentChat);
            chatBox.text = "";
            currentChat = "";
        }

    }

    [SerializeField]
    List<Message> messageList = new List<Message>();

    // Start is called before the first frame update
    void Start()
    {
        // might need to switch authentication values to Photon.Chat something
        ChatConnect();
        //username = OldLogin.playerName;
        username = PhotonNetwork.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected)
        {
            chatClient.Service();
        }

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