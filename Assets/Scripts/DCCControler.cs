using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCCControler : MonoBehaviour
{

    public const int NUM_CHANNELS = 4;

    public List<TrainControl> channels;

    // Start is called before the first frame update
    void Start()
    {
        channels = new List<TrainControl>(NUM_CHANNELS);
        for(int i = 0; i < NUM_CHANNELS; i++)
        {
            channels.Add(new TrainControl { speed = 0f, reverser = false });
        }
    }

    public void SetDCCInformation(List<ControllerState> controllers)
    {
        foreach(ControllerState cs in controllers)
        {
            if(cs.channel < NUM_CHANNELS && cs.channel >= 0)
            {
                SetController(cs.channel, cs.speed, cs.reverser);
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

    public void SetController(int channel, float speed, bool reverser)
    {
        channels[channel].speed = speed;
        channels[channel].reverser = reverser;
    }

    public class TrainControl
    {
        public float speed;
        public bool reverser;
    }
}
