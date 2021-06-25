using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitializationButton : MonoBehaviour
{
    public const string ipKey = "ip_store";
    public const string portKey = "port_store";
    public InputField ipInput;
    public InputField portInput;

    protected void Start()
    {
        Debug.Log("Start called checking player prefs");
        if (PlayerPrefs.HasKey(ipKey))
        {
            Debug.Log("IP key present");
            ipInput.text = PlayerPrefs.GetString(ipKey);
        }
        if (PlayerPrefs.HasKey(portKey))
        {
            Debug.Log("Port Key present");
            portInput.text = PlayerPrefs.GetString(portKey);
        }
    }

    public void OnConnectButtonPressed()
    {
        GameObject clientConnector = GameObject.FindWithTag("ClientConnector");
        ClientSocketScript css = clientConnector.GetComponent<ClientSocketScript>();
        string ip = ipInput.text;
        int port = int.Parse(portInput.text);
        Debug.Log("Connect Button Pressed, creating connection with ip " + ip + " and port " + port);
        PlayerPrefs.SetString(ipKey, ip);
        PlayerPrefs.SetString(portKey, port.ToString());
        PlayerPrefs.Save();
        Debug.Log("Saved player prefs for later");
        css.InitializeConnection(ip, port);
        Debug.Log("Loading Controller screen");
        SceneManager.LoadSceneAsync("Controller");
    }
}
