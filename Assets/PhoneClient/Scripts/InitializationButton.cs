using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitializationButton : MonoBehaviour
{
    public Text serverIP;
    public Text serverPort;

    public void OnConnectButtonPressed()
    {
        GameObject clientConnector = GameObject.FindWithTag("ClientConnector");
        ClientSocketScript css = clientConnector.GetComponent<ClientSocketScript>();
        string ip = serverIP.text;
        int port = int.Parse(serverPort.text);
        Debug.Log("Connect Button Pressed, creating connection with ip " + ip + " and port " + port);
        css.InitializeConnection(ip, port);
        Debug.Log("Loading Controller screen");
        SceneManager.LoadSceneAsync("Controller");
    }
}
