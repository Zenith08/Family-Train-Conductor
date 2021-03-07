using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCarriage : RayTrain
{
    protected float distanceFromEngine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(trainAhead != null)
        {
            distanceFromEngine = Vector3.Distance(transform.position, trainAhead.transform.position);
        }        
    }

    protected override void NotifyBeingPulled(RayTrain puller)
    {
        if(trainAhead == null)
        {
            base.NotifyBeingPulled(puller);
            distanceFromEngine = Vector3.Distance(transform.position, trainAhead.transform.position);
        }
    }

    public override void PullingFixedUpdate(float engineSpeed)
    {
        //Debug.Log("Pulling Carriage Update");
        float speed = engineSpeed;

        if(trainAhead != null)
        {
            float mult = Vector3.Distance(transform.position, trainAhead.transform.position) - distanceFromEngine;
            //Debug.Log("Distance from engine for train " + name + " offset is " + mult);
            speed += mult;
            FixedRotationMovement(speed);
            if(trainBehind != null)
            {
                trainBehind.PullingFixedUpdate(speed);
            }
        }
    }
}
