using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCCControler : MonoBehaviour
{
    public const int NUM_CHANNELS = 4;

    private List<TrainControl> channels;

    // Start is called before the first frame update
    void Start()
    {
        channels = new List<TrainControl>(NUM_CHANNELS);
        for(int i = 0; i < NUM_CHANNELS; i++)
        {
            channels.Add(new TrainControl { speed = 0f, reverser = false, nextSwitch = false });
        }
    }

    public void SetDCCInformation(List<ControllerState> controllers)
    {
        foreach(ControllerState cs in controllers)
        {
            if(cs != null && cs.channel < NUM_CHANNELS && cs.channel >= 0)
            {
                SetController(cs.channel, cs.speed, cs.reverser, cs.nextSwitch);
            }
        }
    }

    public float GetSpeed(int channel)
    {
        return channels[channel].speed;
    }

    public bool GetDirection(int channel)
    {
        return channels[channel].reverser;
    }

    public bool GetNextSwitch(int channel)
    {
        return channels[channel].nextSwitch;
    }

    public void SetController(int channel, float speed, bool reverser, bool nextSwitch)
    {
        if(speed == 0)
        {
            //Debug.LogError("I just want a stacktrace man");
        }
        channels[channel].speed = speed;
        channels[channel].reverser = reverser;
        channels[channel].nextSwitch = nextSwitch;
    }

    public class TrainControl
    {
        public float speed;
        public bool reverser;
        public bool nextSwitch;
    }
}
