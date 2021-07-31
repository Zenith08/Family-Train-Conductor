using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager;

public class SwitchTrack : MonoBehaviour
{
    public static readonly bool STRAIGHT = false;
    public static readonly bool TURN = true;

    public List<SwitchTrack> linkedSwitches;

    public Track straight;
    public Track turn;

    public ClearanceChecker switchClearance;

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

    public void DoSwitch()
    {
        if (CanSwitch())
        {
            if(linkedSwitches.Count == 0)
            {
                direction = !direction;
                UpdateTracks();
                AudioManager.AudioManager.m_instance.PlaySFX(AudioManager.AudioManager.SwitchToggle);
            }
            else
            {
                if (linkedSwitches.TrueForAll(link => link.CanSwitch()))
                {
                    linkedSwitches.ForEach(link => link.SwitchFromLink());
                    SwitchFromLink();
                }
            }
        }
    }

    private void SwitchFromLink()
    {
        if (CanSwitch())
        {
            direction = !direction;
            UpdateTracks();
            AudioManager.AudioManager.m_instance.PlaySFX(AudioManager.AudioManager.SwitchToggle);
        }
    }

    private void UpdateTracks()
    {
        straight.SetTrackActive(direction == STRAIGHT);
        turn.SetTrackActive(direction == TURN);
    }

    protected bool CanSwitch()
    {
        return switchClearance.IsSwitchClear();
    }
}
