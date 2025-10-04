
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NormalValidGrabChecker : ValidGrabChecker
{
    [SerializeField] protected float rayDistance = 0.1f;
    [SerializeField] protected float normalOffsetAngleDegThreshold = 10f;

    public override bool CheckValidGrabPoint(Vector3 worldPosition)
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
