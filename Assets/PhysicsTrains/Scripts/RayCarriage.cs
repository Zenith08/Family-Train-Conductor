using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCarriage : RayTrain
{
    protected float distanceFromEngine;

    public Color resourceColour;
    protected float resourceH;
    protected float resourceS;

    public int resourceNum;

    protected int resourceQty;

    protected Material localMat;

    protected float speed;

    public static readonly float LARGE_EPSILON = 0.0001f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(trainAhead != null)
        {
            distanceFromEngine = Vector3.Distance(transform.position, trainAhead.transform.position);
        }

        localMat = GetComponent<MeshRenderer>().material;

        //Set up for updating colour
        Color.RGBToHSV(resourceColour, out resourceH, out resourceS, out _);
        UpdateColour();
    }

    protected void UpdateColour()
    {
        float qtyBuff = resourceQty;
        float newL = 0.5f + qtyBuff / 100f;
        localMat.color = Color.HSVToRGB(resourceH, resourceS, newL);
    }

    protected override void NotifyBeingPulled(RayTrain puller)
    {
        if(trainAhead == null)
        {
            base.NotifyBeingPulled(puller);
            distanceFromEngine = Vector3.Distance(transform.position, trainAhead.transform.position);
        }
    }

    public bool IsStopped()
    {
        //Debug.Log("Checking stopped with speed " + speed);
        return Mathf.Abs(speed) <= LARGE_EPSILON;
    }

    public override void PullingFixedUpdate(float engineSpeed)
    {
        //Debug.Log("Pulling fixed update with engine speed " + engineSpeed);
        speed = engineSpeed;

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

    public int LoadAsMuchAsPossible(int max)
    {
        if(resourceQty < 100)
        {
            int maxToFull = 100 - resourceQty;
            if(max <= maxToFull)
            {
                resourceQty += max;
                UpdateColour();
                return max;
            }
            else
            {
                resourceQty += maxToFull;
                UpdateColour();
                return maxToFull;
            }
        }
        else
        {
            UpdateColour();
            return 0;
        }
    }

    public int UnloadAsMuchAsPossible()
    {
        int buffer = resourceQty;
        resourceQty = 0;
        UpdateColour();
        return buffer;
    }
}
