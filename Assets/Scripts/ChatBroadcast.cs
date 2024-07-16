using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet;
using TMPro;
using FishNet.Transporting;

public class ChatBroadcast : MonoBehaviour
{
    public Transform chatHolder;
    public GameObject msgElement;
    public TMP_InputField playerUsername;
    public TMP_InputField  playerMessage;

    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnMessageRecieved);
        InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageRecieved);
    }
    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<Message>(OnMessageRecieved);
        InstanceFinder.ServerManager.UnregisterBroadcast<Message>(OnClientMessageRecieved);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }

    private void SendMessage()
    {
        Message msg = new Message()
        {
            username = playerUsername.text,
            message = playerMessage.text
        };

        playerMessage.text = "";

        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.Broadcast(msg);
        else if (InstanceFinder.IsClientStarted)
            InstanceFinder.ClientManager.Broadcast(msg);
    }

    private void OnMessageRecieved(Message msg, Channel channel)
    {
        GameObject finalMessage = Instantiate(msgElement, chatHolder);
        finalMessage.GetComponent<TextMeshProUGUI>().text = msg.username + ": " + msg.message;
    }

    private void OnClientMessageRecieved(NetworkConnection networkConnection, Message msg, Channel channel)
    {
        InstanceFinder.ServerManager.Broadcast(msg);
    }

    public struct Message : IBroadcast
    {
        public string username;
        public string message;
    }
}
