using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLoco : RayTrain
{
    private float speed = 0;

    public float maxSpeedF = 5f;
    public float maxSpeedR = 3f;

    public string goFaster = "w";
    public string goSlower = "s";

    float speedMultiplier = 0.0f;
    private bool forewards = true;

    public bool dccControl = false;
    public int channel = 0;
    private DCCControler controler;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controler = GameObject.FindGameObjectWithTag("DCCControler").GetComponent<DCCControler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dccControl)
        {
            //Update speed
            forewards = speedMultiplier > 0;
            if (Input.GetKeyDown(goFaster))
            {
                speedMultiplier+=0.1f;
                speed = speedMultiplier * (speedMultiplier > 0f ? maxSpeedF : maxSpeedR);
                Debug.Log("Speed now " + speed);
            }
            else if (Input.GetKeyDown(goSlower))
            {
                speed-=0.1f;
                speed = speedMultiplier * (speedMultiplier > 0f ? maxSpeedF : maxSpeedR);
                Debug.Log("Speed now " + speed);
            }
        }
        else
        {
            speedMultiplier = controler.GetSpeed(channel);
            forewards = controler.GetDirection(channel);
            speed = forewards ? (speedMultiplier * maxSpeedF) : (speedMultiplier * -maxSpeedR);
        }
    }

    private void FixedUpdate()
    {
        if(trainAhead == null)
        {
            FixedRotationMovement(speed);
            if(trainBehind != null)
            {
                trainBehind.PullingFixedUpdate(speed);
            }
        }
    }
}
