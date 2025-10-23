using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    private const float DistanceToEndPoint = 5f;

    public GameObject PathPrefab;

    public GameObject DetectionPrefab;

    public GameObject pillarPrefab;
    public GameObject cubePrefab;
    public GameObject dummyPillarPrefab;
    public GameObject dummyCubePrefab;

    public Transform PlayerHead;
    public float spawnForward = 4f;
    public float corridorHalfWidth = 1f;

    //private GameObject current;

    private GameObject pillarRef;
    private GameObject cubeRef;

    private TheObstacle pillarCodeRef;
    private TheObstacle cubeCodeRef;
    private float ThePillarScaleX = .5f;
    private const float ThePillarScaleY = 1.5f;
    private float ThePillarScaleZ = .51f;

    private float TheCubeScaleX = .5f;
    private const float TheCubeScaleY = .5f;
    private float TheCubeScaleZ = .5f;

    private float EndPointX;
    private float EndPointZ;

    private Vector3 Relocationposition;

    private GameObject ObstacleRight;
    private GameObject ObstacleLeft;

    //This boolean defines whether the current active obstacle is 
    private bool IsCurrentObstaclePillar;

    private GameObject currentObstacle
    {
        get { return IsCurrentObstaclePillar ? pillarRef : cubeRef; }
    }

    private TheObstacle GetCurrentCodeRef()
    {
        return IsCurrentObstaclePillar ? pillarCodeRef : cubeCodeRef;
    }

    private bool isFirst = true;

    //In awake the pillar and the cube obstacle creates.
    private void Awake()
    {
        if (!pillarPrefab || !cubePrefab || !dummyPillarPrefab || !dummyCubePrefab || !PlayerHead)
        {
            Debug.Log("[PillarSpawner] Missing references!");
            enabled = false;
            return;
        }
        pillarRef = Instantiate(pillarPrefab, new Vector3(-20, -20, -20), Quaternion.identity);
        pillarRef.SetActive(false);
        cubeRef = Instantiate(cubePrefab, new Vector3(-30, -30, -30), Quaternion.identity);
        cubeRef.SetActive(false);
    }

    void Start()
    {
        //Ensuring they have no parent, because this can cause issue in the transfer and position of them later on.
        pillarRef.transform.SetParent(null, true);
        pillarRef.transform.rotation = Quaternion.identity;
        pillarRef.GetComponent<ObstacleChallenge>().spawner = this;

        cubeRef.transform.SetParent(null, true);
        cubeRef.transform.rotation = Quaternion.identity;
        cubeRef.GetComponent<ObstacleChallenge>().spawner = this;

        //Updating the distance between different pillars
        spawnForward = GameSettings.Instance.PillarDistance;

        //Getting the position of the end point
        //EndPointX = EndPointLogic.Instance.transform.position.x;
        EndPointZ = EndPointLogic.Instance.transform.position.z;

        pillarCodeRef = pillarRef.GetComponent<TheObstacle>();

        if (pillarCodeRef == null)
        {
            Debug.LogWarning("pillarCodeRef is null!!!");
        }

        cubeCodeRef = cubeRef.GetComponent<TheObstacle>();

        if (cubeCodeRef == null)
        {
            Debug.LogWarning("cubeCodeRef is null!!!");
        }

        IsCurrentObstaclePillar = UnityEngine.Random.Range(0, 2) == 0;

        isFirst = true;

        RelocateObstacle();
    }

    public void OnPillarComplete(bool success)
    {
        Debug.Log(success ? "[PillarSpawner] Passed!" : "[PillarSpawner] Hit!");
        GetCurrentCodeRef().HideAllPathways();
        RelocateObstacle();
    }

    void RelocateObstacle()
    {
        Vector3 currentPos = currentObstacle.transform.position;

        //Check if the pillar is not passing the end point, if the pillar passed the end point, then there should be no other obstacle re-activation.
        if (Mathf.Abs((currentPos.z - EndPointZ)) <= DistanceToEndPoint)
        {
            //EndGameEvent!!
            Debug.LogWarning("ReachedTheEnd");
            DeactivateCurrentObstacle();
            return;
            //Deactivating the pillar
        }

        //This code should only run once, because if its the first time there is no need for deactivation logics
        if (!isFirst)
        {
            Instantiate(GetDummyPrefab(), currentPos, Quaternion.identity);
            DeactivateCurrentObstacle();
            //This line randomly chooses the next type of obstacle
            IsCurrentObstaclePillar = UnityEngine.Random.Range(0, 2) == 0;
        }
        else
        {
            isFirst = false;
        }





        // fixed forward
        Relocationposition = PlayerHead.position + Vector3.forward * spawnForward;
        //pos.x += x;   
        // whatever the half‑height of the current obstacle is. This way the obstacle stays exactly on the ground.
        Relocationposition.y = currentObstacle.transform.localScale.y / 2;
        currentObstacle.transform.position = Relocationposition;


        //A method to update the Scale variables.
        //Getting values from another software..
        //Updating ThePillarX & ThePillarZ values by the 3rd party software
        if (IsCurrentObstaclePillar)
        {
            currentObstacle.transform.localScale = new Vector3(ThePillarScaleX, ThePillarScaleY, ThePillarScaleZ);
        }
        else
        {
            currentObstacle.transform.localScale = new Vector3(TheCubeScaleX, TheCubeScaleY, TheCubeScaleZ);
        }


        var col = currentObstacle.GetComponent<Collider>();
        if (col) col.enabled = true;
        currentObstacle.SetActive(true);
        TheObstacle.CurrentActiveInstance = GetCurrentCodeRef();

        //ResetObstacles();

    }

    private GameObject GetDummyPrefab()
    {
        return IsCurrentObstaclePillar ? dummyPillarPrefab : dummyCubePrefab;
    }

    void DeactivateCurrentObstacle()
    {
        var col = currentObstacle.GetComponent<Collider>();
        if (col) col.enabled = false;
        currentObstacle.SetActive(false);
    }

    void ActivateThePillar()
    {
        var col = pillarRef.GetComponent<Collider>();
        if (col) col.enabled = true;
        pillarRef.SetActive(true);
    }

    void DeactivateThePillar()
    {
        var col = pillarRef.GetComponent<Collider>();
        if (col) col.enabled = false;
        pillarRef.SetActive(false);
    }

    //public void ResetObstacles()
    //{
    //    if (ObstacleRight) Destroy(ObstacleRight);

    //    if (ObstacleLeft) Destroy(ObstacleLeft);

    //    if (Physics.Raycast(ThePillarRef.transform.position, ThePillarRef.transform.right, out RaycastHit hitRight, rayDistance))
    //    {
    //        ObstacleRight = Instantiate(ObstaclePrefab, hitRight.point, Quaternion.identity);
    //        ObstacleRight.transform.SetParent(null);
    //        Debug.LogWarning("RayCastRighttSucceded");
    //        //Debug.DrawLine(ThePillarRef.transform.position, hitRight.transform.position, new Color(255, 0, 0), 10);
    //        Debug.DrawRay(ThePillarRef.transform.position, ThePillarRef.transform.right, new Color(255, 0, 0), 20);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("RayCastRightFailed");
    //    }

    //    if (Physics.Raycast(ThePillarRef.transform.position, -ThePillarRef.transform.right, out RaycastHit hitLeft, rayDistance))
    //    {
    //        Debug.Log("hitLeft is: " + hitLeft.collider.gameObject.name);
    //        ObstacleLeft = Instantiate(ObstaclePrefab, hitLeft.point, Quaternion.identity);
    //        ObstacleLeft.transform.SetParent(null);
    //        Debug.LogWarning("RayCastLeftSucceded");
    //        //Debug.DrawLine(ThePillarRef.transform.position, hitLeft.transform.position, new Color(255, 255, 0), 10);
    //        Debug.DrawRay(ThePillarRef.transform.position, -ThePillarRef.transform.right, new Color(255, 255, 0), 20);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("RayCastLeftFailed");
    //    }
    //}
}
