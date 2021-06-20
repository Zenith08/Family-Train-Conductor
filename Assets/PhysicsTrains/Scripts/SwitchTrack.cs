using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrack : MonoBehaviour
{
    public static readonly bool STRAIGHT = false;
    public static readonly bool TURN = true;

    public Track straight;
    public Track turn;

    public bool invert = false;

    public bool direction = STRAIGHT;

    // Start is called before the first frame update
    void Start()
    {
        if(straight == null || turn == null)
        {
            Debug.LogError("Switch track not configured properly. Switch " + name + " functionality disabled");
            enabled = false;
        }
        else
        {
            StartCoroutine(UpdateNextFrame());
        }
    }

    private IEnumerator UpdateNextFrame()
    {
        yield return null;
        UpdateTracks();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            direction = !direction;
            UpdateTracks();
        }*/
    }

    public void DoSwitch()
    {
        direction = !direction;
        UpdateTracks();
    }

    private void UpdateTracks()
    {
        straight.SetTrackActive(direction == STRAIGHT);
        turn.SetTrackActive(direction == TURN);
    }
}
