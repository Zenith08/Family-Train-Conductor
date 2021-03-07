using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackContainer : Track
{
    public List<Track> tracks;

    protected override void Start()
    {
        //no
    }

    public override void SetTrackActive(bool active)
    {
        foreach(Track t in tracks)
        {
            t.SetTrackActive(active);
        }
    }
}
