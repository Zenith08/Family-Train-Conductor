﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;

public class ClientSocketScript : MonoBehaviour
{
	#region private members 	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;

	public string ip;
	public int port;

	public bool isConnected = false;

	public static ControllerState controllerState;

	public delegate void ControllerStateChange(ControllerState newState);
	public event ControllerStateChange OnControllerStateChange;

	#endregion

	// Use this for initialization 	
	void Start()
	{
		Debug.Log("Client Socket Script initializing");
		DontDestroyOnLoad(this.gameObject);
		controllerState = new ControllerState();
	}

    public void InitializeConnection(string ipin, int portin)
    {
		Debug.Log("Client socket creating connection to ip " + ipin);
		ip = ipin;
		port = portin;
		ConnectToTcpServer();
		isConnected = true;
    }

	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer()
	{
		Debug.Log("Attempting to make tcp connection");
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			Debug.Log("Starting listener thread");
			clientReceiveThread.Start();
			Debug.Log("Listener thread started");
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData()
	{
		try
		{
			Debug.Log("Listening for packet");
			socketConnection = new TcpClient(ip, port);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
                try
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        Debug.Log("Network stream created");
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            Debug.Log("Recieved incoming data");
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 						
                            string serverMessage = Encoding.ASCII.GetString(incommingData);
                            Debug.Log("server message received as: " + serverMessage);

                            // Validation step, mimicked from server code
                            if(ValidateAndRecoverPacket(serverMessage, out string validMessage))
                            {
                                Debug.Log("Server message after validation: " + validMessage);
                                try
                                {
                                    controllerState.FromJsonOverwrite(validMessage);
                                }
                                catch(Exception e)
                                {
                                    // Hypothetically this never happens
                                    Debug.LogError(e.GetType().Name + " Exception thrown in JSON Parsing. Json Reads: " + validMessage);
                                }

                            }

                            //if (OnControllerStateChange != null) OnControllerStateChange(controllerState);
                            //StartCoroutine(ForceEventToMainThread(controllerState));
                            stateToPush = controllerState;
                            stateAvailableToPush = true;
                        }
                        Debug.LogError("No more packets to receive");
                    }
                }
                catch(InvalidOperationException ioe)
                {
                    Debug.LogError("Invalid operation exception, the client is likely stopped");
                    Debug.LogError(ioe.ToString());
                    isConnected = false;
                    break;
                }
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
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
                if (count - countBack < 0)
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
            if (!target.StartsWith("{") || !target.EndsWith("}"))
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

    private bool stateAvailableToPush = false;
    private ControllerState stateToPush;

    private void Update()
    {
        if (stateAvailableToPush && stateToPush != null)
        {
            stateAvailableToPush = false;
            if (OnControllerStateChange != null) OnControllerStateChange(stateToPush);
        }
    }

    /*private IEnumerator ForceEventToMainThread(ControllerState stateToPush)
    {
        yield return null;
        if(OnControllerStateChange != null)
        {
            OnControllerStateChange(stateToPush);
        }
    }*/

	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		Debug.Log("Preparing to send message");
		if (socketConnection == null)
		{
			Debug.LogError("Connection is null, failed");
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				Debug.Log("Able to write stream");
				//string clientMessage = "This is a message from one of your clients.";
				string clientMessage = controllerState.ToJson();
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	public void NotifyServerOfChange()
    {
		Debug.Log("Client notified of change, sending message");
		SendMessage();
    }

	public void ChangeEnabledWithNotify(bool stateEnabled)
	{
		controllerState.enabled = stateEnabled;
		NotifyServerOfChange();
	}
	public void SetSpeedWithNotify(float speed)
    {
		controllerState.speed = speed;
		NotifyServerOfChange();
    }
	public void SetNextSwitchWithnotify(bool nextSwitch)
    {
		controllerState.nextSwitch = nextSwitch;
		NotifyServerOfChange();
    }
	public void SetReverserWithnotify(bool reverser)
    {
		controllerState.reverser = reverser;
		NotifyServerOfChange();
    }
	//I don't expect this one to show up
	public void SetChannelWithNotify(int channel)
    {
		controllerState.channel = channel;
		NotifyServerOfChange();
    }
	public void SetMaxSpeedForewardsWithNotify(float masSpeedForewards)
    {
		controllerState.masSpeedForewards = masSpeedForewards;
		NotifyServerOfChange();
	}
	public void SetMaxSpeedBackwardsWithNotify(float maxSpeedBackwards)
    {
		controllerState.maxSpeedBackwards = maxSpeedBackwards;
		NotifyServerOfChange();
    }
}
