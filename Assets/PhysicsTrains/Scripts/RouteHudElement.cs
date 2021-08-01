using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteHudElement : MonoBehaviour
{
    public int routeId;
    public Text route;
    public Text time;

    private int routeStartTime;

    private bool routeClaimed;

    private Color routeColour;

    public void ConfigureWithData(PassengerTrip trip)
    {
        routeStartTime = trip.scheduledStart;
        string routeText = "";
        for(int i = 0, count = trip.route.Count; i < count; i++)
        {
            if(routeText != "")
            {
                routeText += " -> ";
            }
            routeText += trip.route[i].stationId;
        }
        Debug.Log("Applying colour " + routeColour + " to the ui");
        routeColour = trip.tripColour;
        route.color = routeColour;
        route.text = routeText;
        routeClaimed = false;
    }

    private void UpdateTimer()
    {
        // positive here means the time has started, negative means the train hasn't left yet
        int totalSecondsBetween = Mathf.RoundToInt(Time.time - routeStartTime);
        bool routeStartedYet = totalSecondsBetween >= 0;
        int absSeconds = totalSecondsBetween >= 0 ? totalSecondsBetween : -totalSecondsBetween;

        int displaySeconds = absSeconds % 60;
        int displayMinutes = absSeconds / 60;
        string secondsString = displaySeconds.ToString();
        if(secondsString.Length == 1)
        {
            secondsString = "0" + secondsString;
        }
        string displayText = displayMinutes.ToString() + ":" + secondsString;
        if(routeClaimed)
        {
            time.color = Color.yellow;
        }
        else if (routeStartedYet)
        {
            time.color = Color.red;
        }
        else
        {
            time.color = Color.black;
        }
        time.text = displayText;
    }

    public void NotifyRouteStarted()
    {
        routeClaimed = true;
    }

    public void MarkDone()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }
}
