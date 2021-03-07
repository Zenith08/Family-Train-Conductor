using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public GameObject leftRail;
    public GameObject rightRail;

    protected Collider localCollider;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        localCollider = GetComponent<Collider>();
        if(leftRail == null || rightRail == null || localCollider == null)
        {
            Debug.LogError("Something is null for track " + name);
        }
    }

    public virtual void SetTrackActive(bool active)
    {
        leftRail.SetActive(active);
        rightRail.SetActive(active);
        localCollider.enabled = active;
    }
}
