using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : SynchronizedValue
{
    public Slider sliderRef;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sliderRef = GetComponent<Slider>();
        sliderRef.onValueChanged.AddListener(OnSliderValueChanged);
    }
    
    public void OnSliderValueChanged(float newSlider)
    {
        if(connection != null) connection.SetSpeedWithNotify(newSlider);
    }

    protected override void OnControllerStateChange(ControllerState newState)
    {
        sliderRef.value = newState.speed;
    }
}
