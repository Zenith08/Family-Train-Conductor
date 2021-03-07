﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientSocketScript : MonoBehaviour
{
	#region private members 	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;

	public string ip;
	public int port;

	public bool isConnected = false;

	private ControllerState controllerState;

	public delegate void ControllerStateChange(ControllerState newState);
	public event ControllerStateChange OnControllerStateChange;

	#endregion
	// Use this for initialization 	
	void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		controllerState = new ControllerState();
	}
	// Update is called once per frame
	void Update()
	{
		//?
	}

	public void InitializeConnection(string ipin, int portin)
    {
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
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
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
			socketConnection = new TcpClient(ip, port);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
						controllerState.FromJsonOverwrite(serverMessage);
						if (OnControllerStateChange != null) OnControllerStateChange(controllerState);
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		if (socketConnection == null)
		{
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
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