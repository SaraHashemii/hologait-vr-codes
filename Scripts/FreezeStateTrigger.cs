using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreezeStateTrigger : MonoBehaviour
{
    public InputActionProperty inputAction;

    private TheObstacle ThePillarRef;

    public Transform HeadOfPlayerTransform;


    void OnEnable()
    {
        if (inputAction.reference != null) inputAction.reference.action.Enable();
        else inputAction.action.Enable();
    }

    void OnDisable()
    {
        if (inputAction.reference != null) inputAction.reference.action.Disable();
        else inputAction.action.Disable();
    }

    private void Start()
    {
        this.ThePillarRef = TheObstacle.CurrentActiveInstance;

        if (ThePillarRef == null)
        {
            Debug.LogWarning("ThePIllarRefIs NULL !!");
        }


        if (HeadOfPlayerTransform == null)
        {
            Debug.LogWarning("The PLayer Head IS NULL !!!!");
        }
    }
    void Update()
    {
        if (inputAction.action.WasPressedThisFrame()
//#if UNITY_EDITOR && ENABLE_LEGACY_INPUT_MANAGE
|| Input.GetKeyDown(KeyCode.P)
        //#endif
        )
        {
            Debug.Log("[VR Input] A button (Activate) was pressed!");

            // Creating a pathway from the player to around the pillar
            TheObstacle.CurrentActiveInstance.CreatePathWay(HeadOfPlayerTransform);
        }
    }
}
