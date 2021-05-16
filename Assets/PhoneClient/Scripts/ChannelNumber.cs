using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelNumber : SynchronizedValue
{

    public Text textRef;

    protected override void Start()
    {
        base.Start();
        textRef = GetComponent<Text>();
        textRef.text = "-1";
    }

    protected void Update()
    {
        textRef.text = ClientSocketScript.controllerState.channel.ToString();
    }

    protected override void OnControllerStateChange(ControllerState newState)
    {
        textRef.text = newState.channel.ToString();
    }
}
