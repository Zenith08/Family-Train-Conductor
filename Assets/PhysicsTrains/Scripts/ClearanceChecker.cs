using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearanceChecker : MonoBehaviour
{
    HashSet<RayTrain> trainsInArea = new HashSet<RayTrain>();

    protected void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<RayTrain>(out RayTrain trainComponent))
        {
            if (!trainsInArea.Contains(trainComponent))
            {
                trainsInArea.Add(trainComponent);
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<RayTrain>(out RayTrain trainComponent))
        {
            if (trainsInArea.Contains(trainComponent))
            {
                trainsInArea.Remove(trainComponent);
            }
        }
    }

    public bool IsSwitchClear()
    {
        return trainsInArea.Count == 0;
    }
}
