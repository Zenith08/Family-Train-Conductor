using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ControllerState
{
    public bool enabled;
    public float speed;
    public bool nextSwitch;
    public bool reverser;
    public int channel;
    public float masSpeedForewards;
    public float maxSpeedBackwards;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public ControllerState FromJsonOverwrite(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
        return this;
    }

    public static ControllerState FromJson(string json)
    {
        return JsonUtility.FromJson<ControllerState>(json);
    }
}
