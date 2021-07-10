﻿using System.Collections;
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

    public Transform frontClearanceRay;

    private bool targetSwitchLastFrame = false;

    private bool isCollided = false;

    [Header("Directional Lights")]
    public List<GameObject> forwardLights;
    public List<GameObject> reverseLights;

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
            bool newDirection = controler.GetDirection(channel);
            if(forewards != newDirection)
            {
                forewards = newDirection;
                UpdateDirectionLights();
            }
            speed = forewards ? (speedMultiplier * maxSpeedF) : (speedMultiplier * -maxSpeedR);

            if (forewards)
            {
                bool rayFront = Physics.Raycast(frontClearanceRay.transform.position, frontClearanceRay.forward, out RaycastHit frontHit, 1f);
                if(rayFront)
                {
                    if(frontHit.collider.TryGetComponent<RayTrain>(out _))
                    {
                        speed = 0f;
                    }
                }
            }

            if (isCollided) speed = 0; //If we hit another train we are not moving until that is solved.
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

    private void UpdateDirectionLights()
    {
        foreach(GameObject go in forwardLights)
        {
            go.SetActive(forewards);
        }
        foreach(GameObject go in reverseLights)
        {
            go.SetActive(!forewards);
        }
    }
}
