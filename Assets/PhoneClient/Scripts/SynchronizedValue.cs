using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SynchronizedValue : MonoBehaviour
{
    protected ClientSocketScript connection;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GameObject connector = GameObject.FindWithTag("ClientConnector");
        if (connector != null)
        {
            connection = connector.GetComponent<ClientSocketScript>();
            connection.OnControllerStateChange += OnControllerStateChange;
        }
        else
        {
            Debug.LogError("Started object " + name + " without connection. Things will fail");
        }
    }

    protected abstract void OnControllerStateChange(ControllerState newState);
}
