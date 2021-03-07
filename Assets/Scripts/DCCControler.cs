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
