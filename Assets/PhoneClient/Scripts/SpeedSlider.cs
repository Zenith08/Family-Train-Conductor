using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : SynchronizedValue
{
    public Slider sliderRef;

    private float lastValue;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sliderRef = GetComponent<Slider>();
        sliderRef.onValueChanged.AddListener(OnSliderValueChanged);
    }
    
    public void OnSliderValueChanged(float newSlider)
    {
        // If we are slowing down apply snap to 0
        if(newSlider < 0.02)
        {
            newSlider = 0f;
            sliderRef.value = 0f;
        }

        if(connection != null) connection.SetSpeedWithNotify(newSlider);
        lastValue = newSlider;
    }

    protected override void OnControllerStateChange(ControllerState newState)
    {
        sliderRef.value = newState.speed;
    }
}
