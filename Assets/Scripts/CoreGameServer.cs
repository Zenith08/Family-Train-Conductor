using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class CoreGameServer : MonoBehaviour
{
	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener;
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;
	/// <summary> 	
	/// Create handle to connected tcp client.
	/// </summary>
	private List<ConnectedClient> clients = new List<ConnectedClient>(4);

	public DCCControler controllerRef;

	public bool serverAlive = true;

	private static ConnectedClient EMPTY_CLIENT = new ConnectedClient
	{
		client = null,
		clientListenerThread = null,
		channel = 0,
		alive = false,
		baseState = new ControllerState()
	};
	#endregion

	// Start is called before the first frame update
	void Start()
    {
		Debug.Log("Core game server starting");
		serverAlive = true;
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests))
        {
            IsBackground = true
        };

		for(int i = 0; i < 4; i++)
        {
			clients.Add(EMPTY_CLIENT);
			controllerStates.Add(new ControllerState(-1));
        }
		Debug.Log("Starting thread");
		tcpListenerThread.Start();
	}

    private void OnDestroy()
    {
		serverAlive = false;
		tcpListener.Stop();
		Debug.Log("The server is shutting down. This will generate an exception. Everything is fine");
		tcpListenerThread.Abort();
		tcpListenerThread.Join();
    }

    private List<ControllerState> controllerStates = new List<ControllerState>();
    private void Update()
    {
        for(int i = 0; i < clients.Count && i < 4; i++)
        {
			//controllerStates[i] = clients[i].baseState;
			ConnectedClient client = clients[i];
			if (client.alive && client.baseState != null)
            {
				//Debug.Log("Client " + i + " has DCC data, On Channel " + client.baseState.channel + " sending speed " + client.baseState.speed + " reverser " + client.baseState.reverser);
				ControllerState state = client.baseState;
				controllerRef.SetController(state.channel, state.speed, state.reverser, state.nextSwitch);
			}
        }
		//controllerRef.SetDCCInformation(controllerStates);
    }

    /// <summary> 	
    /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
    /// </summary> 	
    private void ListenForIncommingRequests()
	{
		Debug.Log("Function listen for incoming requests has started");
		try
		{
			Debug.Log("Try catch block has started");
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Any, 8052);
			tcpListener.Start();
			Debug.Log("Server is listening");
			TcpClient connectedTcpClient = null;

			while (true)
			{
				Debug.Log("In main listener loop");
				try
				{
					connectedTcpClient = tcpListener.AcceptTcpClient();
					Debug.Log("Recieved client connection");

					int channel = 0;
					bool channelFound = false;
					while(!channelFound)
                    {
						if(clients.Count > channel)
                        {
							if(!clients[channel].alive)
                            {
								channelFound = true;
                            }
                            else
                            {
								channel++;
                            }
                        }
                        else
                        {
							clients.Add(EMPTY_CLIENT);
							channelFound = true;
                        }
                    }
					Debug.Log("Client found channel " + channel);
					//There is a race condition here where 2 threads try to modify the same client at the same time but whatever
					ConnectedClient newClient = new ConnectedClient();
					newClient.alive = true;
					newClient.channel = channel;
					newClient.client = connectedTcpClient;
					clients[channel] = newClient;

					Debug.Log("Starting client thread");
					clients[channel].CreateAndStartThread();
				}
				catch(Exception e)
                {
					if (connectedTcpClient != null) connectedTcpClient.Dispose();
					Debug.LogError(e.ToString());
					Debug.LogError(e.Message);
					Debug.LogError(e.StackTrace);
					Debug.LogError(e.Source);
                }
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("SocketException " + socketException.ToString());
		}
	}
}
