using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionToggle : SynchronizedValue
{

    public bool isForewards;
    protected Toggle toggleRef;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        toggleRef = GetComponent<Toggle>();
        toggleRef.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void OnToggleValueChanged(bool toggleOn)
    {
        if (toggleOn)
        {
            if (connection != null) connection.SetReverserWithnotify(isForewards);
        }
    }

    protected override void OnControllerStateChange(ControllerState newState)
    {
        toggleRef.isOn = newState.reverser == isForewards;
    }
}
