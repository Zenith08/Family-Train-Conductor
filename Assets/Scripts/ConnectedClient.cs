﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ConnectedClient
{
    public TcpClient client;
    public Thread clientListenerThread;
    public int channel;
    public bool alive;

    public void CreateAndStartThread()
    {
        clientListenerThread = new Thread(new ThreadStart(ListenForControl));
        clientListenerThread.IsBackground = true;
        clientListenerThread.Start();
        ControllerState baseState = new ControllerState();
        baseState.channel = channel;
        baseState.speed = 0;
        baseState.reverser = true;
        SendMessage(baseState);
    }

    public void ListenForControl()
    {
        int length;
        byte[] bytes = new byte[1024];
        using (NetworkStream stream = client.GetStream())
        {
            // Read incomming stream into byte arrary. 						
            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incommingData = new byte[length];
                Array.Copy(bytes, 0, incommingData, 0, length);
                // Convert byte array to string message. 							
                string clientMessage = Encoding.ASCII.GetString(incommingData);
                Debug.Log("client message received as: " + clientMessage);
            }
        }
    }

    private void SendMessage(ControllerState message)
    {
        if (client == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = client.GetStream();
            if (stream.CanWrite)
            {
                string serverMessage = message.ToJson();
                // Convert string message to byte array.                 
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                // Write byte array to socketConnection stream.               
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}
