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
    [SerializeField] Text chatDisplay;
    ChatClient chatClient;
    bool isConnected;
    string currentChat;
    string privateReceiver = "";
    public Dropdown chatDropdown;
    public TranscriptController transcriptController;


    // callbacks
    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }
    public void OnChatStateChange(ChatState state)
    {
        if(state == ChatState.Uninitialized)
        {
            isConnected = false;
            chatPanel.SetActive(false);
        }
    }
    public void OnConnected() {
        UnityEngine.Debug.Log("Connection to chat successful");
        transcriptController.SendMessageToTranscript("subscribing to group chat", TranscriptMessage.SubsystemType.chat);
        chatClient.Subscribe(new string[] {"RegionChannel"});
    }
    public void OnDisconnected()
    {
        isConnected = false;
        chatPanel.SetActive(false);
    }
    public void OnGetMessages(string channelName, string[] senders, object[] messages) 
    {
        UnityEngine.Debug.Log("Calling OnGetMessages");
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}: {1}", senders[i], messages[i]);
            chatDisplay.text += "\n" + msgs;
            UnityEngine.Debug.Log(msgs);
        }
    }
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string msgs = "";
        msgs = string.Format("(Private) {0}: {1}", sender, message);
        chatDisplay.text += "\n " + msgs;
        UnityEngine.Debug.Log(msgs);
    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatPanel.SetActive(true);
        UnityEngine.Debug.Log("Subscribed to new channel");
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
        WaitForSeconds(1);
        isConnected = true;
        chatClient = new ChatClient(this);
        transcriptController.SendMessageToTranscript("connecting to chat server", TranscriptMessage.SubsystemType.chat);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
        transcriptController.SendMessageToTranscript("connection to chat server successful", TranscriptMessage.SubsystemType.chat);
        UnityEngine.Debug.Log("Connection to chat");
    }

    public void setChatDropdown() 
    {
        chatDropdown.options.Clear();
        chatDropdown.options.Add(new Dropdown.OptionData("public"));
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++ )
        {
            if (PhotonNetwork.PlayerList[i].NickName != PhotonNetwork.NickName)
            {
                chatDropdown.options.Add(new Dropdown.OptionData(PhotonNetwork.PlayerList[i].NickName));
            }
        }
    }

    public void TypeChatOnValueChange(string valueIn)
    {
        currentChat = valueIn;
    }

    public void SubmitPublicChatOnClick()
    {
        
        if (privateReceiver == "" && currentChat != "") {
            UnityEngine.Debug.Log("InSubmitPublicChatOnClick " + currentChat);
            UnityEngine.Debug.Log("SubmitPublic:  calling publish message");
            chatClient.PublishMessage("RegionChannel", currentChat);
            transcriptController.SendMessageToTranscript("publishing new message to chat", TranscriptMessage.SubsystemType.chat);
            chatBox.text = "";
            currentChat = "";
        }

    }
    public void SubmitPrivateChatOnClick()
    {
        if (privateReceiver != "" && currentChat != "") {
            UnityEngine.Debug.Log("InSubmitPrivateChatOnClick " + currentChat);
            chatClient.PublishMessage(privateReceiver, currentChat);
            chatBox.text = "";
            currentChat = "";
        }

    }

    public void OnApplicationQuit() 
    {
        if (chatClient != null)
            chatClient.Disconnect();
    }

    public void OnReceiverValueChanged()
    {
        UnityEngine.Debug.Log("Receiver value changed to: " + chatDropdown.options[chatDropdown.value].text );
        privateReceiver = chatDropdown.options[chatDropdown.value].text;
        if (privateReceiver == "public") 
        {
            privateReceiver = "";
        }
    }
  


    [SerializeField]
    List<Message> messageList = new List<Message>();
    

    // Start is called before the first frame update
    void Start()
    {
        
        // might need to switch authentication values to Photon.Chat something
        Application.runInBackground = true; 
        
        ChatConnect();
        
        //username = OldLogin.playerName;
        username = PhotonNetwork.NickName;
        
        setChatDropdown();
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected)
        {
            chatClient.Service();
        }
        setChatDropdown();

        if (chatBox.text != "" && Input.GetKey(KeyCode.Return))
        {
            SubmitPublicChatOnClick();
            SubmitPrivateChatOnClick();
        }

        /*
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
        */

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