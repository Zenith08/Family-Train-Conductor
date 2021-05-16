using System;
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

    public ControllerState baseState;

    public void CreateAndStartThread()
    {
        Debug.Log("Client started, beginning thread");
        clientListenerThread = new Thread(new ThreadStart(ListenForControl));
        clientListenerThread.IsBackground = true;
        baseState = new ControllerState();
        baseState.channel = channel;
        baseState.speed = 0;
        baseState.reverser = true;

        //Smart thing to do: this last
        clientListenerThread.Start();
        Debug.Log("Client initialized, sending message");
        SendMessage(baseState);
    }

    public void ListenForControl()
    {
        Debug.Log("Listener thread starting");
        int length;
        byte[] bytes = new byte[1024];
        using (NetworkStream stream = client.GetStream())
        {
            Debug.Log("Network Stream found");
            alive = true;
            // Read incomming stream into byte arrary. 						
            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                Debug.Log("Recieved packet");
                var incommingData = new byte[length];
                Array.Copy(bytes, 0, incommingData, 0, length);
                // Convert byte array to string message.
                string clientMessage = Encoding.ASCII.GetString(incommingData);
                Debug.Log("client message received as: " + clientMessage);
                if (clientMessage.Contains("}{"))
                {
                    Debug.LogError("SOMETHING IS WRONG REJECTING PACKAGE");
                    continue; //Abandon this package
                }
                else
                {
                    baseState.FromJsonOverwrite(clientMessage);
                }
            }
        }
        Debug.LogWarning("Client " + channel + " lost");
        alive = false;
    }

    private void SendMessage(ControllerState message)
    {
        Debug.Log("Trying to send message to client");
        baseState = message;
        if (client == null)
        {
            Debug.LogError("Client socket null, failing");
            return;
        }

        try
        {
            Debug.Log("Attempting to send message");
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
