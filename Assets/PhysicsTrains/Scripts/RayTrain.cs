using UnityEngine;

public class RayTrain : MonoBehaviour
{
    public GameObject rayStart;
    private Rigidbody trainRigidbody;

    public RayTrain trainAhead;
    public RayTrain trainBehind;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        trainRigidbody = GetComponent<Rigidbody>();
        if (trainBehind != null)
        {
            trainBehind.NotifyBeingPulled(this);
        }
    }

    protected virtual void NotifyBeingPulled(RayTrain puller)
    {
        if (trainAhead == null)
        {
            trainAhead = puller;
        }
    }

    protected virtual void FixedRotationMovement(float speed)
    {
        //Get rotation from track
        bool rayDown = Physics.Raycast(rayStart.transform.position, -rayStart.transform.up, out RaycastHit downHit, 0.5f);
        Debug.DrawRay(rayStart.transform.position, -rayStart.transform.up, Color.blue, 0.5f);
        if (!rayDown)
        {
            Debug.LogError("Not on a track or something because down failed.");
        }

        float newY = downHit.collider.transform.rotation.eulerAngles.y;
        float currentY = transform.rotation.eulerAngles.y;
        float delta = Mathf.Abs(newY - currentY);
        if (delta > 180.0f)
        {
            delta -= 360.0f;
            delta = Mathf.Abs(delta);
        }
        if (delta > 90.0f)
        {
            newY += 180.0f;
        }

        trainRigidbody.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, newY, transform.rotation.eulerAngles.z));

        //TODO get horizontal position from sides
        bool rayLeft = Physics.Raycast(rayStart.transform.position, -rayStart.transform.right, out RaycastHit leftHit, 0.5f);
        Debug.DrawRay(rayStart.transform.position, -rayStart.transform.right, Color.red, 0.5f);
        bool rayRight = Physics.Raycast(rayStart.transform.position, rayStart.transform.right, out RaycastHit rightHit, 0.5f);
        Debug.DrawRay(rayStart.transform.position, rayStart.transform.right, Color.green, 0.5f);

        if (!rayRight || !rayLeft)
        {
            Debug.LogWarning("Derailed side rays did not work");
        }

        trainRigidbody.MovePosition(transform.position + (transform.right * (rightHit.distance - leftHit.distance) / 2) + (transform.forward * speed * 0.1f));
    }

    public virtual void PullingFixedUpdate(float engineSpeed)
    {
        //Default behaviour is do nothing
    }

    /*
    private void LineFollowMovement()
    {
        bool rayLeft = Physics.Raycast(rayStart.transform.position, -rayStart.transform.right, out RaycastHit leftHit, 2f);
        Debug.DrawRay(rayStart.transform.position, -rayStart.transform.right, Color.red, 2f);
        bool rayRight = Physics.Raycast(rayStart.transform.position, rayStart.transform.right, out RaycastHit rightHit, 2f);
        Debug.DrawRay(rayStart.transform.position, rayStart.transform.right, Color.green, 2f);

        if (!rayLeft || !rayRight)
        {
            Debug.LogError("SOMETHING FAILED BECAUSE WE DIDN'T HIT ANYTHING left: " + rayLeft + " right " + rayRight);
            return;
        }
        else
        {
            float leftDistance = leftHit.distance;
            float rightDistance = rightHit.distance;
            //Debug.Log("Rays both hit with left distance " + leftDistance + " and right distance " + rightDistance);
            float delta = Mathf.Abs(leftDistance - rightDistance);
            Debug.Log("Ray delta is " + delta);
            delta = 1 + delta * 10;

            if (leftDistance < rightDistance && speed > 0)
            {
                trainRigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + RIGHT * (delta)));
            }
            else if (rightDistance < leftDistance && speed > 0)
            {
                trainRigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles - RIGHT * (delta)));
            }
            else if (leftDistance < rightDistance && speed < 0)
            {
                trainRigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles - RIGHT * (delta)));
            }
            else if (rightDistance < leftDistance && speed < 0)
            {
                trainRigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + RIGHT * (delta)));
            }
        }

        trainRigidbody.MovePosition(transform.position + transform.forward * speed * 0.01f);
    }
    */
    private float Lowpass(float input, float threshold)
    {
        return input < threshold ? 0 : input;
    }
}
