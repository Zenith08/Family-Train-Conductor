using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    public List<PassengerTrip> activeTrips;

    public List<PassengerTrip> baseTripsToGenerate;

    public List<int> tripGenerationPattern;

    public int timeBetweenGenerations;

    public int bookAheadTime = 30;

    private int lastTimeTripGenerated;
    private int nextTripToGenerate;
    private int locationInGenerationPattern = 0;

    public GameObject uiHudPrefab;
    private GameObject uiHudRoot;

    // Start is called before the first frame update
    void Start()
    {
        nextTripToGenerate = tripGenerationPattern[0];
        activeTrips = new List<PassengerTrip>();
        uiHudRoot = GameObject.FindWithTag("UIHudRoot");
        Debug.Log("PassengerSystem started");
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastTimeTripGenerated >= timeBetweenGenerations)
        {
            //Generate Trip
            Debug.Log("PassengerSystem scheduling new route based on template " + tripGenerationPattern[locationInGenerationPattern]);
            lastTimeTripGenerated = Mathf.RoundToInt(Time.time);
            ScheduleRoute();
        }
    }

    private void ScheduleRoute()
    {
        GameObject hudComponent = GameObject.Instantiate(uiHudPrefab, uiHudRoot.transform);
        PassengerTrip newTrip = new PassengerTrip(baseTripsToGenerate[nextTripToGenerate], Mathf.RoundToInt(Time.time + bookAheadTime), hudComponent.GetComponent<RouteHudElement>());
        UpdateTripGeneration(); //Restores this for the next cycle
        activeTrips.Add(newTrip);
    }

    private void UpdateTripGeneration()
    {
        locationInGenerationPattern++;
        if(locationInGenerationPattern >= tripGenerationPattern.Count)
        {
            locationInGenerationPattern = 0;
        }
        nextTripToGenerate = tripGenerationPattern[locationInGenerationPattern];
    }

    public void TrainStoppedAtStation(int stationNumber, PassengerCarrier train)
    {
        if(train.activeRoute != -1)
        {
            //The train is on a route, see if the station needs marking
            PassengerTrip trainsRoute = activeTrips[train.activeRoute];
            //Sanity check, if this is false we got a problem
            if (trainsRoute.CurrentlyAssignedTrain() != train.uniqueId)
            {
                Debug.LogError("Train was assigned to route but the route didn't know it. High Risk situation");
                return;
            }

            if (trainsRoute.ReportTrainStoppedAtStation(stationNumber))
            {
                Debug.Log("PassengerSystem Train has finished route " + train.uniqueId);

                train.activeRoute = -1;
            }
        }
        else
        {
            //Is the station eligable to start a route
            for(int i = 0, count = activeTrips.Count; i < count; i++)
            {
                if(!activeTrips[i].isRouteDone && 
                    activeTrips[i].CurrentlyAssignedTrain() == 0 && 
                    activeTrips[i].StationStartsRoute(stationNumber))
                {
                    if(Time.time > activeTrips[i].scheduledStart)
                    {
                        Debug.Log("PassengerSystem Train " + train.uniqueId + " is starting route " + i);

                        activeTrips[i].TrainStartingRoute(train);
                        train.activeRoute = i;
                    }
                }
            }
        }
        //If the train is supposed to be at the station, mark it as stopped
        //If the route is done flag it and free the train
    }
}

[System.Serializable]
public class PassengerTrip
{
    [SerializeField]
    public List<RequiredStops> route;
    [SerializeField]
    public int scheduledStart;
    [SerializeField]
    public int actualStart;
    [SerializeField]
    public int end;
    [SerializeField]
    public int assignedTrain;

    public bool isRouteDone = false;
    //UI component
    private RouteHudElement uiComponent;

    public PassengerTrip(List<RequiredStops> stops, int scheduledStart, RouteHudElement hudElement)
    {
        this.route = stops;
        this.scheduledStart = scheduledStart;
        this.uiComponent = hudElement;
        uiComponent.ConfigureWithData(this);
    }

    public PassengerTrip(PassengerTrip original, int scheduledStart, RouteHudElement hudElement)
    {
        this.scheduledStart = scheduledStart;
        route = new List<RequiredStops>(original.route.Count);
        for(int i = 0, count = original.route.Count; i < count; i++)
        {
            route.Add(new RequiredStops
            {
                hasStopped = false,
                timeStopped = 0,
                stationId = original.route[i].stationId
            });
        }
        this.uiComponent = hudElement;
        uiComponent.ConfigureWithData(this);
    }

    public bool StationStartsRoute(int station)
    {
        return route[0].stationId == station;
    }

    public int CurrentlyAssignedTrain()
    {
        return assignedTrain;
    }

    public bool ReportTrainStoppedAtStation(int station)
    {
        for(int i = 0, count = route.Count; i < count; i++)
        {
            if(station == route[i].stationId)
            {
                route[i].hasStopped = true;
                route[i].timeStopped = Mathf.RoundToInt(Time.time);
            }
            else if (!route[i].hasStopped)
            {
                Debug.LogWarning("Train seems to have skipped a station.");
                return false;
            }
        }
        return CheckRouteDone();
    }

    public bool CheckRouteDone()
    {
        bool anyNotStopped = false;
        for(int i = 0, count = route.Count; i < count; i++)
        {
            if (!route[i].hasStopped)
            {
                anyNotStopped = true;
            }
        }
        if (!anyNotStopped) isRouteDone = true;
        if (isRouteDone)
        {
            end = Mathf.RoundToInt(Time.time);
            uiComponent.MarkDone();
            uiComponent = null;
        }
        return isRouteDone;
        //Update ui stuff
    }

    public void TrainStartingRoute(PassengerCarrier train)
    {
        assignedTrain = train.uniqueId;
        route[0].hasStopped = true;
        uiComponent.NotifyRouteStarted();
    }
}

[System.Serializable]
public class RequiredStops
{
    public int stationId;
    public bool hasStopped;
    public int timeStopped;
}
