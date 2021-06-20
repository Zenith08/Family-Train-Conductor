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

    private bool targetSwitchLastFrame = false;

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
            //Debug.Log("Getting speed from DCC, result " + speedMultiplier + " speed is " + speed);
            if(targetSwitchLastFrame != controler.GetNextSwitch(channel))
            {
                targetSwitchLastFrame = controler.GetNextSwitch(channel);
                //Get rotation from track
                bool rayDown = Physics.Raycast(rayStart.transform.position, -rayStart.transform.up, out RaycastHit downHit, 0.5f);
                Debug.DrawRay(rayStart.transform.position, -rayStart.transform.up, Color.blue, 0.5f);
                if (!rayDown)
                {
                    Debug.LogError("Not on a track or something because switch check failed.");
                }
                else
                {
                    Track tk = downHit.collider.gameObject.GetComponent<Track>();
                    SwitchTrack st = forewards ? tk.nextSwitchForwards : tk.nextSwitchReverse;
                    if(st != null)
                    {
                        st.DoSwitch();
                    }
                }
            }
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
