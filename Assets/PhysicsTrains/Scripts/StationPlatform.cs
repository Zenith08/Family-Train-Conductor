using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationPlatform : MonoBehaviour
{
    public int platformId;
    private PassengerManager passengerManager;
    public TextMesh textDisplay;

    private void Start()
    {
        passengerManager = GameObject.FindWithTag("PassengerManager").GetComponent<PassengerManager>();
        textDisplay.text = platformId.ToString();
    }

    public void OnTriggerStay(Collider other)
    {
        //Debug.Log("Element staying in trigger " + other.gameObject.name);
        //If it is a train car that's great. If not I don't care.
        if (other.gameObject.TryGetComponent<RayCarriage>(out RayCarriage traincar))
        {
            //Debug.Log("Element is a traincar and stopped = " + traincar.IsStopped());
            if (traincar.IsStopped())
            {
                if(other.gameObject.TryGetComponent<PassengerCarrier>(out PassengerCarrier passengerCarrier))
                {
                    passengerManager.TrainStoppedAtStation(platformId, passengerCarrier);
                }
            }
        }
    }
}
