
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DesktopClimbing : UdonSharpBehaviour
{
    [SerializeField] Transform currentGrabIndicator;
    [SerializeField] Transform nextGrabIndicator;

    [SerializeField] float rayDistance = 0.1f;
    [SerializeField] float normalOffsetAngleDegThreshold = 10f;

    VRCPlayerApi localPlayer;
    float armLength;
    float currentGrabDistance;
    float currentPlayerOffset;

    DesktopClimingStates currentState;
    enum DesktopClimingStates
    {
        idle,
        initialSelection,
        movingPlayer,
        movingConnector
    }

    void Start()
    {
        localPlayer = Networking.LocalPlayer;

        RecalculateArmLenght();
    }

    private void Update()
    {
        switch (currentState)
        {
            case DesktopClimingStates.idle:
                // No need to do anything
                break;
            case DesktopClimingStates.initialSelection:
                PositionIndicator();
                break;
            case DesktopClimingStates.movingPlayer:
                break;
            case DesktopClimingStates.movingConnector:
                PositionIndicator();
                break;
            default:
                break;
        }
    }

    void PositionIndicator()
    {
        currentGrabDistance += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
        currentGrabDistance = Mathf.Clamp(currentPlayerOffset, armLength * 0.1f, armLength);

        nextGrabIndicator.position = HeadOffsetPosition(currentGrabDistance);
    }

    void CheckGrabConnection()
    {
        bool validGrabConnection = CheckValidGrabPoint(nextGrabIndicator.position);

    }

    Vector3 HeadOffsetPosition(float offset)
    {
        VRCPlayerApi.TrackingData head = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        return head.rotation * (offset * Vector3.forward) + head.position;
    }

    bool CheckValidGrabPoint(Vector3 worldPosition)
    {
        Ray ray = new Ray(worldPosition, Vector3.down);

        if (Physics.Raycast(ray, out var hitInfo, rayDistance))
        {
            if (Vector3.Angle(hitInfo.normal, Vector3.up) <= normalOffsetAngleDegThreshold)
                return true;
        }

        return false;
    }

    public override void OnAvatarChanged(VRCPlayerApi player)
    {
        base.OnAvatarChanged(player);
    }

    public override void OnAvatarEyeHeightChanged(VRCPlayerApi player, float prevEyeHeightAsMeters)
    {
        base.OnAvatarEyeHeightChanged(player, prevEyeHeightAsMeters);
    }

    void RecalculateArmLenght()
    {
        Vector3 chest = localPlayer.GetBonePosition(HumanBodyBones.Chest);
        Vector3 shoulder = localPlayer.GetBonePosition(HumanBodyBones.LeftShoulder);
        Vector3 upperArm = localPlayer.GetBonePosition(HumanBodyBones.LeftUpperArm);
        Vector3 lowerArm = localPlayer.GetBonePosition(HumanBodyBones.LeftLowerArm);
        Vector3 hand = localPlayer.GetBonePosition(HumanBodyBones.LeftHand);

        armLength = (chest - shoulder).magnitude
            + (shoulder - upperArm).magnitude
            + (upperArm - lowerArm).magnitude
            + (lowerArm - hand).magnitude;
    }
}
