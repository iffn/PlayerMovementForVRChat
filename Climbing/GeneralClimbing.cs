using System.Collections;
using System.Collections.Generic;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

public abstract class GeneralClimbing : UdonSharpBehaviour
{
    [SerializeField] protected Material validGrabMaterial;
    [SerializeField] protected Material invalidGrabMaterial;
    [SerializeField] protected Material idleMaterial;

    [SerializeField] ValidGrabChecker validGrabChecker;

    [SerializeField] protected VRCStation linkedStation;

    [SerializeField] protected TextMeshPro debugOutput;

    [SerializeField] ClimbingActivationInformer[] climbingActivationInformers;

    protected VRCPlayerApi localPlayer;
    protected Transform stationMover;
    bool isUsingStation = false;

    public virtual string DebugString
    {
        get
        {
            string returnString = "";

            returnString += $"{nameof(IsUsingStation)}: {IsUsingStation}\n";
            returnString += $"{nameof(stationMover)}.position: {stationMover.position}\n";

            return returnString;
        }
    }

    public bool IsUsingStation
    {
        get
        {
            return isUsingStation;
        }
        protected set
        {
            if (value == isUsingStation)
                return;

            linkedStation.gameObject.SetActive(value);

            if (value)
            {
                stationMover.SetPositionAndRotation(localPlayer.GetPosition(), localPlayer.GetRotation());
                linkedStation.UseStation(localPlayer);
            }
            else
            {
                linkedStation.ExitStation(localPlayer);
            }

            isUsingStation = value;
            
            foreach(ClimbingActivationInformer informer in climbingActivationInformers)
            {
                if (value)
                    informer.ClimbingStart();
                else
                    informer.ClimbingStop();
            }
        }
    }

    protected void Setup()
    {
        localPlayer = Networking.LocalPlayer;
        stationMover = linkedStation.transform;
        linkedStation.gameObject.SetActive(false);
    }

    protected void PositionPlayer(Vector3 handPosition, Vector3 targetPosition)
    {
        stationMover.position += (targetPosition - handPosition);
    }

    protected bool CheckValidGrabPoint(Vector3 worldPosition)
    {
        return validGrabChecker.CheckValidGrabPoint(worldPosition);
    }
}
