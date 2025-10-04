
using Nessie.Udon.Movement;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NUClimbingInformer : ClimbingActivationInformer
{
    [SerializeField] NUMovement linkedNUMovement;

    public override void ClimbingStart()
    {
        linkedNUMovement._ControllerDisable();
    }

    public override void ClimbingStop()
    {
        linkedNUMovement._ControllerEnable();
    }
}
