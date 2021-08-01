using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerCarrier : MonoBehaviour
{
    public int uniqueId = 1; //Needs to be different from the other trains in the world
    public int activeRoute = -1;

    public List<Renderer> inputObjects;
    protected List<Material> trainRouteStrips;
    protected Color currentRouteColor;

    private void Start()
    {
        trainRouteStrips = new List<Material>(inputObjects.Count);
        inputObjects.ForEach(io => trainRouteStrips.Add(io.material) );
        UpdateMaterialColours(Color.black);
    }

    public void UpdateMaterialColours(Color col)
    {
        currentRouteColor = col;
        foreach(Material m in trainRouteStrips)
        {
            m.color = col;
        }
    } 

    public Color GetRouteColour()
    {
        return currentRouteColor;
    }
}
