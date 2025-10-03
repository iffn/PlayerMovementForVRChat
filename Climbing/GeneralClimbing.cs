using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

public abstract class GeneralClimbing : UdonSharpBehaviour
{
    [SerializeField] protected Material validGrabMaterial;
    [SerializeField] protected Material invalidGrabMaterial;
    [SerializeField] protected Material idleMaterial;

    [SerializeField] protected float rayDistance = 0.1f;
    [SerializeField] protected float normalOffsetAngleDegThreshold = 10f;

    [SerializeField] protected VRCStation linkedStation;

    protected VRCPlayerApi localPlayer;
    protected Transform stationMover;
    bool isUsingStation = false;

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
        }
    }

    protected void Setup()
    {
        localPlayer = Networking.LocalPlayer;
        stationMover = linkedStation.transform;
    }

    protected void PositionPlayer(Vector3 handPosition, Vector3 targetPosition)
    {
        stationMover.position += (targetPosition - handPosition);
    }

    protected bool CheckValidGrabPoint(Vector3 worldPosition)
    {
        Ray ray = new Ray(worldPosition, Vector3.down);

        if (Physics.Raycast(ray, out var hitInfo, rayDistance))
        {
            if (Vector3.Angle(hitInfo.normal, Vector3.up) <= normalOffsetAngleDegThreshold)
                return true;
        }

        return false;
    }
}
