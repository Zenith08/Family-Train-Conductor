using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industry : MonoBehaviour
{
    public IndustryMode mode = IndustryMode.PRODUCER;

    [Header("Production")]
    public int producedResource;
    public float producedPerSecond;
    public int producedQuantity;
    [Header("Consumption")]
    public int consumedResource;
    public float consumedPerSecond;
    public int remainingConsumables;
    [Header("Mechanics")]
    public TextMesh resourceDisplay;

    // Update is called once per frame
    protected void Update()
    {
        if(mode == IndustryMode.PRODUCER)
        {
            producedQuantity += Mathf.RoundToInt(producedPerSecond * Time.deltaTime);
            if (producedQuantity > 400) producedQuantity = 400;
        }
        else if(mode == IndustryMode.CONSUMER)
        {
            remainingConsumables -= Mathf.RoundToInt(consumedPerSecond * Time.deltaTime);
            if (remainingConsumables < 0) remainingConsumables = 0;
        }
        else if(mode == IndustryMode.CONVERTER)
        {
            int ammountToConsume = Mathf.RoundToInt(consumedPerSecond * Time.deltaTime);
            if (remainingConsumables - ammountToConsume > 0 && producedQuantity < 400)
            {
                remainingConsumables -= ammountToConsume;
                producedQuantity += Mathf.RoundToInt(producedPerSecond * Time.deltaTime);
                if (producedQuantity > 400) producedQuantity = 400;
            }
        }
    }

    protected void LateUpdate()
    {
        UpdateResourceDisplay();
    }

    private void UpdateResourceDisplay()
    {
        switch (mode)
        {
            case IndustryMode.PRODUCER:
                resourceDisplay.text = producedQuantity.ToString();
                break;
            case IndustryMode.CONSUMER:
                resourceDisplay.text = remainingConsumables.ToString();
                break;
            case IndustryMode.CONVERTER:
            default:
                resourceDisplay.text = remainingConsumables.ToString() + "/" + producedQuantity.ToString();
                break;
        }
            
    }

    public void OnTriggerStay(Collider other)
    {
        //Debug.Log("Element staying in trigger " + other.gameObject.name);
        //If it is a train car that's great. If not I don't care.
        if(other.gameObject.TryGetComponent<RayCarriage>(out RayCarriage traincar))
        {
            //Debug.Log("Element is a traincar and stopped = " + traincar.IsStopped());
            if (traincar.IsStopped())
            {
                if(Produces() && producedResource == traincar.resourceNum)
                {
                    //Load the car as much as possible
                    producedQuantity -= traincar.LoadAsMuchAsPossible(producedQuantity);
                }
                else if(Consumes() && consumedResource == traincar.resourceNum)
                {
                    //Unload the car as much as possible
                    remainingConsumables += traincar.UnloadAsMuchAsPossible();
                }
            }
        }
    }

    public bool Produces()
    {
        return mode == IndustryMode.CONVERTER || mode == IndustryMode.PRODUCER;
    }

    public bool Consumes()
    {
        return mode == IndustryMode.CONVERTER || mode == IndustryMode.CONSUMER;
    }
}

public enum IndustryMode
{
    PRODUCER = 1, CONVERTER = 3, CONSUMER = 2
}
