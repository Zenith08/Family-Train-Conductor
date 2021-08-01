using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSingleToggle : SynchronizedValue
{
    protected bool currentDirection = false;
    protected Button buttonRef;

    // Start is called before the first frame update
    protected override void Start()
    {
        Debug.LogWarning("Switch Toggle init");
        base.Start();
        buttonRef = GetComponent<Button>();
        buttonRef.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        Debug.LogWarning("SwitchToggle clicked");
        currentDirection = !currentDirection;
        if (connection != null) connection.SetNextSwitchWithnotify(currentDirection);
    }

    protected override void OnControllerStateChange(ControllerState newState)
    {
        // Pass
    }
}
