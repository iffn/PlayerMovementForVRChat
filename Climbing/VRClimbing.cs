
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public enum VRClimbingStates
{
    idle,
    initialSelection,
    leftGrip,
    rightGrip
}

public class VRClimbing : GeneralClimbing
{
    [SerializeField] Transform leftGrabIndicator;
    [SerializeField] Transform rightGrabIndicator;

    [SerializeField] MeshRenderer leftHandRenderer;
    [SerializeField] MeshRenderer rightHandRenderer;

    VRClimbingStates currentState;
    bool prevHandUse = false;

    Vector3 leftHandPosiiton => localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
    Vector3 rightHandPosiiton => localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;

    bool validGrab;

    void Start()
    {
        Setup();

        if (!localPlayer.IsUserInVR())
        {
            gameObject.SetActive(false);
            return;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case VRClimbingStates.idle:
                break;
            case VRClimbingStates.initialSelection:
                leftGrabIndicator.position = leftHandPosiiton;
                rightGrabIndicator.position = leftHandPosiiton;
                break;
            case VRClimbingStates.leftGrip:
                PositionPlayer(leftHandPosiiton, leftGrabIndicator.position);
                HandleIndicator(rightHandPosiiton, rightGrabIndicator, rightHandRenderer);
                break;
            case VRClimbingStates.rightGrip:
                PositionPlayer(rightHandPosiiton, rightGrabIndicator.position);
                HandleIndicator(leftHandPosiiton, leftGrabIndicator, leftHandRenderer);
                break;
            default:
                break;
        }
    }

    void HandleIndicator(Vector3 handPosition, Transform indicator, MeshRenderer linkedRenderer)
    {
        validGrab = CheckValidGrabPoint(handPosition);
        indicator.position = handPosition;
        leftHandRenderer.sharedMaterial = validGrab ? validGrabMaterial : invalidGrabMaterial;
    }

    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        base.InputUse(value, args);

        switch (args.handType)
        {
            case HandType.RIGHT:
                if(currentState == VRClimbingStates.leftGrip)
                {
                    if (value && !prevHandUse)
                    {
                        if (validGrab)
                        {
                            currentState = VRClimbingStates.rightGrip;
                            rightGrabIndicator.position = rightHandPosiiton;
                            
                            prevHandUse = false;
                            return;
                        }
                    }
                    
                    prevHandUse = value;
                }
                break;
            case HandType.LEFT:
                break;
            default:
                break;
        }
    }
}
