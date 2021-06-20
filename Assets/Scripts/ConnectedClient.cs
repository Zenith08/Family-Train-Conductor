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
                //Message validation stage

                if (ValidateAndRecoverPacket(clientMessage, out string validMessage))
                {
                    Debug.Log("Message after validation is: " + validMessage);
                    try
                    {
                        baseState.FromJsonOverwrite(validMessage);
                    }
                    catch (Exception e)
                    {
                        //Hypothetically this never happens
                        Debug.LogError(e.GetType().Name + " Exception thrown in JSON parsing. Json reads " + validMessage);
                    }
                }
                else
                {
                    Debug.LogError("Rejected invalid packet");
                }
            }
        }
        Debug.LogWarning("Client " + channel + " lost");
        alive = false;
    }

    private bool ValidateAndRecoverPacket(string target, out string result)
    {
        if (target.Contains("}{"))
        {
            //If it is a multipacket then we can try to recover
            //Recover bad packet
            Debug.LogWarning("Bad client packet, multiple in one. Attempting to recover");
            string[] options = target.Replace("}{", "}~{").Split('~');  //target.Split(new string[] { "}{" }, StringSplitOptions.None);
            int count = options.Length;
            int countBack = 1;
            result = options[count - countBack];
            string resultLastCheck;
            while (!ValidateAndRecoverPacket(result, out resultLastCheck))
            {
                Debug.LogWarning("Internal packet was bad, attempting previous one");
                countBack++;
                if(count - countBack < 0)
                {
                    Debug.LogError("All elements in a multi-packet were invalid, rejecting");
                    result = "";
                    return false;
                }
                else
                {
                    result = options[count - countBack];
                }
            }
            result = resultLastCheck;
            return true;
        }
        else
        {
            //If not it has to pass basic tests or it will fail
            if(!target.StartsWith("{") || !target.EndsWith("}"))
            {
                Debug.LogError("Rejecting malformed packet without opening or closing bracket");
                result = "";
                return false;
            }
            else
            {
                //The original packet is a valid, single packet message.
                result = target;
                return true;
            }
        }
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
