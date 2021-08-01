using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationPlatformBase : MonoBehaviour
{

    public List<GameObject> passengerObjects;
    public List<int> stationPlatformIds;
    public int crowdingCount;

    // Start is called before the first frame update
    void Start()
    {
        crowdingCount = 0;
        UpdateActiveObjects();
        StationBase sb = new StationBase()
        {
            platformIds = this.stationPlatformIds.ToArray(),
            platform = this
        };
        PassengerManager pm = GameObject.FindWithTag("PassengerManager").GetComponent<PassengerManager>();
        pm.stationPlatformBases.Add(sb);
        Debug.Log("Platform base initialized and registered");
    }

    public void UpdateCrowdingCount(int newCount)
    {
        crowdingCount = newCount;
        UpdateActiveObjects();
    }

    public void IncreaseCrowding()
    {
        crowdingCount++;
        UpdateActiveObjects();
    }

    public void DecrementCrowding()
    {
        crowdingCount--;
        if (crowdingCount < 0) crowdingCount = 0;
        UpdateActiveObjects();
    }

    private void UpdateActiveObjects()
    {
        Debug.Log("Updating active objects");
        for(int i = 0, count = passengerObjects.Count; i < count; i++)
        {
            Debug.Log("Checking slot " + i + " with crowd " + crowdingCount);
            passengerObjects[i].SetActive(i < crowdingCount);
        }
    }
}
