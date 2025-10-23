using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheObstacle : MonoBehaviour
{
    private int PathwayActivationIntervalInMs = 550; // in ms
    [SerializeField] private AudioSource PathwayAppearingAudioSource;
    [SerializeField] private AudioClip PathwayAppearingClip;


    public GameObject PathwayPrefab;
    public GameObject DetectionPrefab;



    static public TheObstacle CurrentActiveInstance;

    //these objects are the very right and left of the pillar where a straight line from center of it hits the wall.
    private GameObject BoundryLeft;
    private GameObject BoundryRight;

    private GameObject[] PathwayObjects = new GameObject[60];

    private const float rayDistance = 10f;

    private int LayerMask;
    private int indexOfLastUsedPathwayObject;

    // holds the time between reappearing of pathways object
    private WaitForSeconds wait;

    private Coroutine activationRoutine;

    //curveStep is the variable defining the distance in between pathways in the curve situation
    private float curvePathwayStepDistance = 0.45f;
    private float StraightPathwayDistance = 0.5f;
    private float StraightPatwayDistanceOffset = 0.05f;

    // Start is called before the first frame update

    private void Awake()
    {
        Debug.Log("PathwayActivationInterval is: " + PathwayActivationIntervalInMs / 1000f);
        wait = new WaitForSeconds(PathwayActivationIntervalInMs / 1000f);

        if (PathwayAppearingAudioSource)
        {
            PathwayAppearingAudioSource.playOnAwake = false;
            PathwayAppearingAudioSource.loop = false;
            if (PathwayAppearingClip) PathwayAppearingAudioSource.clip = PathwayAppearingClip;
        }


        DontDestroyOnLoad(this.gameObject); // Optional, if you want it to persist
    }
    void Start()
    {
        float curveStep = 0.45f;
        //Creating a number of pathways like an object pool
        for (int i = 0; i < PathwayObjects.Length; i++)
        {
            PathwayObjects[i] = Instantiate(PathwayPrefab, Vector3.zero, Quaternion.identity);
            PathwayObjects[i].SetActive(false);
            PathwayObjects[i].gameObject.transform.parent = null;
        }
        LayerMask = UnityEngine.LayerMask.GetMask("Default");

        this.curvePathwayStepDistance = ToCM(GameSettings.Instance.PathwayDistance);
        this.StraightPathwayDistance = ToCM(GameSettings.Instance.PathwayDistance + StraightPatwayDistanceOffset);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreatePathWay(Transform Head)
    {
        if (Physics.Raycast(this.transform.position, this.transform.right, out RaycastHit hitRight, rayDistance))
        {
            BoundryRight = Instantiate(DetectionPrefab, hitRight.point, Quaternion.identity);
            BoundryRight.transform.SetParent(null);
            Debug.LogWarning("RayCastRighttSucceded");
            //Debug.DrawLine(ThePillarRef.transform.position, hitRight.transform.position, new Color(255, 0, 0), 10);
            Debug.DrawRay(this.transform.position, this.transform.right, new Color(255, 0, 0), 20);
        }
        else
        {
            Debug.LogWarning("RayCastRightFailed");
        }

        if (Physics.Raycast(this.transform.position, -this.transform.right, out RaycastHit hitLeft, rayDistance))
        {
            Debug.Log("hitLeft is: " + hitLeft.collider.gameObject.name);
            BoundryLeft = Instantiate(DetectionPrefab, hitLeft.point, Quaternion.identity);
            BoundryLeft.transform.SetParent(null);
            Debug.LogWarning("RayCastLeftSucceded");
            //Debug.DrawLine(ThePillarRef.transform.position, hitLeft.transform.position, new Color(255, 255, 0), 10);
            Debug.DrawRay(this.transform.position, -this.transform.right, new Color(255, 255, 0), 20);
        }
        else
        {
            Debug.LogWarning("RayCastLeftFailed");
        }

        Vector3 headGround = GetGroundPosition(Head.position);
        Vector3 pillarGround = GetGroundPosition(transform.position);
        Vector3 leftGround = GetGroundPosition(BoundryLeft.transform.position);
        Vector3 rightGround = GetGroundPosition(BoundryRight.transform.position);

        float distanceToPillar = Vector3.Distance(headGround, pillarGround);
        HideAllPathways();
        if (distanceToPillar >= 3.5f)
        {
            Vector3 corridorRight = (rightGround - leftGround).normalized;
            Vector3 corridorForward = Vector3.Cross(Vector3.up, corridorRight).normalized;

            if (Vector3.Dot(corridorForward, (pillarGround - headGround)) < 0)
                corridorForward = -corridorForward;

            Vector3 straightEnd = headGround + corridorForward * 3.5f;

            Vector3? lastStraightPoint = GenerateStraightPathway(headGround, corridorForward, 3.5f, pillarGround);
            Vector3 curveStart = lastStraightPoint ?? headGround; // fallback if no straight segment
            GenerateCurvedPathway(curveStart, transform.position, leftGround, rightGround);

        }
        else
        {

            GenerateCurvedPathway(headGround, transform.position, leftGround, rightGround);
        }
        StartActivatingPathways();
    }

    Vector3 GetGroundPosition(Vector3 worldPos)
    {
        return Physics.Raycast(worldPos, Vector3.down, out RaycastHit hit, 10f, LayerMask)
            ? hit.point
            : worldPos;
    }

    Vector3? GenerateStraightPathway(Vector3 startPos, Vector3 direction, float maxDistanceToPillar, Vector3 pillarGround)
    {
        Vector3? lastPos = null;

        for (int i = 0; indexOfLastUsedPathwayObject < PathwayObjects.Length; i++)
        {
            Vector3 point = startPos + direction * (StraightPathwayDistance * i);
            float distanceToPillar = Vector3.Distance(point, pillarGround);

            if (distanceToPillar <= maxDistanceToPillar)
                break;

            PathwayObjects[indexOfLastUsedPathwayObject].transform.position = point;
            //PathwayObjects[indexOfLastUsedPathwayObject].SetActive(true);
            lastPos = point;
            indexOfLastUsedPathwayObject++;
        }

        return lastPos;
    }



    void GenerateCurvedPathway(Vector3 startPos, Vector3 pillarWorld, Vector3 leftGround, Vector3 rightGround)
    {

        Vector3 pillarGround = GetGroundPosition(pillarWorld);
        Vector3 pillarForward = transform.forward;
        //Calculating the curve end point, which is a meter infornt of the pillar on the ground
        Vector3 curveEnd = GetGroundPosition(pillarGround + pillarForward * 1f);

        float distLeft = Vector3.Distance(pillarGround, leftGround);
        float distRight = Vector3.Distance(pillarGround, rightGround);

        Vector3 controlOffsetDir = (distLeft > distRight)
            ? (leftGround - pillarGround).normalized
            : (rightGround - pillarGround).normalized;

        //based on which distance is farther to the pillar, we should consider end point of the curve .5 meter toward that side
        // Determine left or right in world space
        Vector3 rightDir = transform.right;

        // Check which axis rightDir is mostly aligned with
        bool rightIsX = Mathf.Abs(rightDir.x) > Mathf.Abs(rightDir.z);

        if (distLeft > distRight)
        {

            if (rightIsX)
            {
                curveEnd.x = (curveEnd.x + BoundryLeft.transform.position.x) / 2;
            }
            else
            {
                curveEnd.z = (curveEnd.z + BoundryLeft.transform.position.z) / 2;
            }

        }
        else
        {
            if (rightIsX)
            {
                curveEnd.x = (curveEnd.x + BoundryRight.transform.position.x) / 2;
            }
            else
            {
                curveEnd.z = (curveEnd.z + BoundryRight.transform.position.z) / 2;
            }
        }

        Vector3 controlPoint = pillarGround + controlOffsetDir * Mathf.Max(distLeft, distRight) * 0.5f;

        float totalDist = Vector3.Distance(startPos, curveEnd);
        int segmentCount = Mathf.FloorToInt(totalDist / curvePathwayStepDistance);

        Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 startXZ = new Vector3(startPos.x, 0f, startPos.z);

        // Replace only the loop with this:
        for (int i = 0; i <= segmentCount && indexOfLastUsedPathwayObject < PathwayObjects.Length; i++)
        {
            float t = (segmentCount > 0) ? i / (float)segmentCount : 0f;

            // Point on the curve (use its X — and Y if you like)
            Vector3 curvePoint = CalculateQuadraticBezierPoint(t, startPos, controlPoint, curveEnd);

            // Z marches uniformly along rig-forward on the ground
            Vector3 uniformXZ = startXZ + fwd * (i * curvePathwayStepDistance);

            float x = curvePoint.x;      // X from curve
            float y = curvePoint.y;      // keep your current Y (or ground it if you prefer)
            float z = uniformXZ.z;       // Z from uniform forward steps

            PathwayObjects[indexOfLastUsedPathwayObject].transform.position = new Vector3(x, y, z);
            indexOfLastUsedPathwayObject++;
        }
        PathwayObjects[indexOfLastUsedPathwayObject].transform.position = PathwayObjects[indexOfLastUsedPathwayObject].transform.position + PathwayObjects[indexOfLastUsedPathwayObject].transform.forward / 3;
    }

    public void HideAllPathways()
    {
        Debug.Log("HideAllPathways() called");
        for (int i = 0; i < indexOfLastUsedPathwayObject; ++i)
        {
            PathwayObjects[i].SetActive(false);
        }
        indexOfLastUsedPathwayObject = 0;
    }
    private IEnumerator ActivatePathwaysCoroutine()
    {
        Debug.Log("ActivePathwaysCoroutine is called");
        for (int i = 0; i < indexOfLastUsedPathwayObject; i++)
        {
            if (PathwayObjects[i] && !PathwayObjects[i].activeSelf)
            {
                if (AudioManager.Instance == null)
                {
                    Debug.Log(" No AudioManager instance exists");
                }
                else
                {
                    //AudioManager.Instance.PlayHarmonicMelodies();
                    AudioManager.Instance.PlaySpatialHarmonicMelodies(PathwayObjects[i].transform.position);
                    PathwayObjects[i].SetActive(true);
                    Debug.Log($"PathwayObject[{i}] gets activated");
                }

            }
            // restart sound each time
            if (PathwayAppearingAudioSource && PathwayAppearingAudioSource.clip)
            {
                PathwayAppearingAudioSource.Stop();
                PathwayAppearingAudioSource.time = 0f;
                PathwayAppearingAudioSource.Play();
            }

            yield return wait; // no allocation here
        }
    }
    void StartActivatingPathways()
    {

        if (activationRoutine != null) StopCoroutine(activationRoutine);
        activationRoutine = StartCoroutine(ActivatePathwaysCoroutine());
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 +
               2 * (1 - t) * t * p1 +
               Mathf.Pow(t, 2) * p2;
    }

    //If a variable is set in a way that each whole number means a centimeter, ToCM can adjust it in a way that it become compatible with Unity system
    private float ToCM(float value)
    {

        return value / 100f;
    }

    private float ToCM(int value)
    {
        return value / 100f;
    }
}


