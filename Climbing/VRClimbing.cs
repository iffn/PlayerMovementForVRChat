
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
    bool prevLeftHandUse = false;
    bool prevRightHandUse = false;

    Vector3 leftHandPosiiton => localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
    Vector3 rightHandPosiiton => localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;

    bool validLeftGrab;
    bool validRightGrab;

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
                HandleIndicator(rightHandPosiiton, rightGrabIndicator, rightHandRenderer, ref validRightGrab);
                break;
            case VRClimbingStates.rightGrip:
                PositionPlayer(rightHandPosiiton, rightGrabIndicator.position);
                HandleIndicator(leftHandPosiiton, leftGrabIndicator, leftHandRenderer, ref validLeftGrab);
                break;
            default:
                break;
        }
    }

    void HandleIndicator(Vector3 handPosition, Transform indicator, MeshRenderer linkedRenderer, ref bool validGrab)
    {
        validGrab = CheckValidGrabPoint(handPosition);
        indicator.position = handPosition;
        leftHandRenderer.sharedMaterial = validGrab ? validGrabMaterial : invalidGrabMaterial;
    }

    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        base.InputUse(value, args);

        bool onDown = false;
        bool onUp = false;

        switch (args.handType)
        {
            case HandType.RIGHT:
                onDown = value && !prevRightHandUse;
                onUp = !value && prevRightHandUse;
                prevRightHandUse = value;
                break;
            case HandType.LEFT:
                onDown = value && !prevLeftHandUse;
                onUp = !value && prevLeftHandUse;
                prevLeftHandUse = value;
                break;
            default:
                break;
        }

        if (onDown)
        {
            switch (currentState)
            {
                case VRClimbingStates.idle:
                    break;
                case VRClimbingStates.initialSelection:
                    CheckGrab(args.handType);
                    break;
                case VRClimbingStates.leftGrip:
                    if(args.handType == HandType.RIGHT)
                        CheckGrab(args.handType);
                    break;
                case VRClimbingStates.rightGrip:
                    if (args.handType == HandType.LEFT)
                        CheckGrab(args.handType);
                    break;
                default:
                    break;
            }
        }
        else if (onUp)
        {
            switch (currentState)
            {
                case VRClimbingStates.idle:
                    break;
                case VRClimbingStates.initialSelection:
                    break;
                case VRClimbingStates.leftGrip:
                    if (args.handType == HandType.LEFT)
                        Drop();
                    break;
                case VRClimbingStates.rightGrip:
                    if (args.handType == HandType.RIGHT)
                        Drop();
                    break;
                default:
                    break;
            }
        }
    }

    void Drop()
    {
        currentState = VRClimbingStates.initialSelection;
        IsUsingStation = false;
    }

    void CheckGrab(HandType grabHand)
    {
        bool validGrab = grabHand == HandType.LEFT ? validLeftGrab : validRightGrab;

        if (!validGrab)
            return;

        currentState = grabHand == HandType.LEFT ? VRClimbingStates.leftGrip : VRClimbingStates.rightGrip;
        IsUsingStation = true;

        MeshRenderer idleRenderer = grabHand == HandType.LEFT ? leftHandRenderer : rightHandRenderer;
        idleRenderer.sharedMaterial = idleMaterial;
    }
}
