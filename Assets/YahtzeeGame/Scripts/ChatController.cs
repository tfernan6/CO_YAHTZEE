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
    private static TranscriptController transcriptController;
    public Text dropdownValue;


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
        //UnityEngine.Debug.Log("Connection to chat successful");
        if (transcriptController != null)
            transcriptController.SendMessageToTranscript("Subscribing to group chat", TranscriptMessage.SubsystemType.chat);
        chatClient.Subscribe(new string[] {"RegionChannel"});
    }
    public void OnDisconnected()
    {
        if (transcriptController != null)
            transcriptController.SendMessageToTranscript("Disconnecting from chat server", TranscriptMessage.SubsystemType.chat);
        isConnected = false;
        chatPanel.SetActive(false);
    }
    public void OnGetMessages(string channelName, string[] senders, object[] messages) 
    {
        if (transcriptController != null)
            transcriptController.SendMessageToTranscript(string.Format("Pulling {0} new message(s) from chat server", messages.Length), TranscriptMessage.SubsystemType.chat);
       // UnityEngine.Debug.Log("Calling OnGetMessages");
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}: {1}", senders[i], messages[i]);
            chatDisplay.text += "\n" + msgs;
            //UnityEngine.Debug.Log(msgs);
        }
    }
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string msgs = "";
        msgs = string.Format("(Private) {0}: {1}", sender, message);
        chatDisplay.text += "\n " + msgs;
        //UnityEngine.Debug.Log(msgs);
    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatPanel.SetActive(true);
      //  UnityEngine.Debug.Log("Subscribed to new channel");
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
        if (transcriptController != null)
            transcriptController.SendMessageToTranscript("Connecting to chat server", TranscriptMessage.SubsystemType.chat);
        
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
        
        if (transcriptController != null)
            transcriptController.SendMessageToTranscript("Connection to chat server successful", TranscriptMessage.SubsystemType.chat);
        //UnityEngine.Debug.Log("Connection to chat");
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
            if (transcriptController != null)
                transcriptController.SendMessageToTranscript("Sending public chat", TranscriptMessage.SubsystemType.chat);
            
            //UnityEngine.Debug.Log("InSubmitPublicChatOnClick " + currentChat);
            //UnityEngine.Debug.Log("SubmitPublic:  calling publish message");
            
            if (transcriptController != null)
                transcriptController.SendMessageToTranscript(string.Format("Publishing new message to chat: {0}", currentChat), TranscriptMessage.SubsystemType.chat);
            
            chatClient.PublishMessage("RegionChannel", currentChat);
            chatBox.text = "";
            currentChat = "";
        }

    }
    public void SubmitPrivateChatOnClick()
    {
        if (privateReceiver != "" && currentChat != "") {
            transcriptController.SendMessageToTranscript("Sending private chat to " + privateReceiver, TranscriptMessage.SubsystemType.chat);
            //UnityEngine.Debug.Log("InSubmitPrivateChatOnClick " + currentChat);
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
        
        //UnityEngine.Debug.Log("Receiver value changed to: " + chatDropdown.options[chatDropdown.value].text );
        privateReceiver = chatDropdown.options[chatDropdown.value].text;
        dropdownValue.text = privateReceiver;
        if (transcriptController != null)
            transcriptController.SendMessageToTranscript("Updating chat recipient to " + privateReceiver, TranscriptMessage.SubsystemType.chat);
        
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
        try
        {

            // might need to switch authentication values to Photon.Chat something
            if (transcriptController == null &&
                GameObject.Find("TranscriptController") != null)
            {
                transcriptController = GameObject.Find("TranscriptController").GetComponent<TranscriptController>();
            }
            else
            {
                UnityEngine.Debug.Log("Transcript controller is null");
            }

            dropdownValue.text = "public";

            Application.runInBackground = true;
            ChatConnect();

            username = PhotonNetwork.NickName;
            setChatDropdown();
        }
        catch(System.Exception ex)
        {
            UnityEngine.Debug.Log(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
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
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.Log(ex.Message);
        }

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