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
	//private TcpClient connectedTcpClient;
	private List<ConnectedClient> clients = new List<ConnectedClient>(4);
	#endregion

	// Start is called before the first frame update
	void Start()
    {
		Debug.Log("Core game server starting");
		// Start TcpServer background thread 		
		tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
		tcpListenerThread.IsBackground = true;
		tcpListenerThread.Start();
	}

	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests()
	{
		try
		{
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8052); //Localhost may be the issue here
			tcpListener.Start();
			Debug.Log("Server is listening");
			//Byte[] bytes = new Byte[1024];
			TcpClient connectedTcpClient;

			while (true)
			{
				Debug.Log("In main listener loop");
				using (connectedTcpClient = tcpListener.AcceptTcpClient())
				{
					Debug.Log("Recieved client connection");
					int channel = 0;
					while(clients[channel] != null && clients[channel].alive)
                    {
						channel++;
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

					/*// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream())
					{
						int length;
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var incommingData = new byte[length];
							Array.Copy(bytes, 0, incommingData, 0, length);
							// Convert byte array to string message. 							
							string clientMessage = Encoding.ASCII.GetString(incommingData);
							Debug.Log("client message received as: " + clientMessage);
						}
					}*/
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("SocketException " + socketException.ToString());
		}
	}
}
