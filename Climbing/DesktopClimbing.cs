
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon;

public enum DesktopClimbingStates
{
    idle,
    initialSelection,
    movingPlayer,
    movingConnector
}

public class DesktopClimbing : GeneralClimbing
{
    [SerializeField] Transform currentGrabIndicator;
    [SerializeField] Transform nextGrabIndicator;

    [SerializeField] MeshRenderer nextGrabRenderer;
    
    float armLength;
    float currentGrabDistance;

    DesktopClimbingStates currentState = DesktopClimbingStates.initialSelection;

    DesktopClimbingStates CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            switch (value)
            {
                case DesktopClimbingStates.idle:
                    currentGrabIndicator.gameObject.SetActive(false);
                    nextGrabIndicator.gameObject.SetActive(false);
                    IsUsingStation = false;
                    break;
                case DesktopClimbingStates.initialSelection:
                    currentGrabIndicator.gameObject.SetActive(false);
                    nextGrabIndicator.gameObject.SetActive(true);
                    IsUsingStation = false;
                    break;
                case DesktopClimbingStates.movingPlayer:
                    currentGrabIndicator.gameObject.SetActive(true);
                    nextGrabIndicator.gameObject.SetActive(false);
                    teleportLocation = localPlayer.GetPosition();
                    IsUsingStation = true;
                    break;
                case DesktopClimbingStates.movingConnector:
                    currentGrabIndicator.gameObject.SetActive(true);
                    nextGrabIndicator.gameObject.SetActive(true);
                    IsUsingStation = true;
                    break;
                default:
                    break;
            }

            currentState = value;
        }
    }

    void Start()
    {
        Setup();

        if (localPlayer.IsUserInVR())
        {
            gameObject.SetActive(false);
            return;
        }

        RecalculateArmLength();
        currentGrabDistance = armLength;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case DesktopClimbingStates.idle:
                // No need to do anything
                break;
            case DesktopClimbingStates.initialSelection:
                PositionIndicator();
                CheckGrabConnection();
                break;
            case DesktopClimbingStates.movingPlayer:
                PositionIndicator();
                MovePlayer();
                break;
            case DesktopClimbingStates.movingConnector:
                CheckGrabConnection();
                PositionIndicator();
                break;
            default:
                break;
        }
    }

    void PositionIndicator()
    {
        currentGrabDistance += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
        currentGrabDistance = Mathf.Clamp(currentGrabDistance, armLength * 0.1f, armLength);

        nextGrabIndicator.position = HeadOffsetPosition(currentGrabDistance);
    }

    void CheckGrabConnection()
    {
        bool validGrabConnection = CheckValidGrabPoint(nextGrabIndicator.position);

        nextGrabRenderer.sharedMaterial = validGrabConnection ? validGrabMaterial : invalidGrabMaterial;

        if (validGrabConnection)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                CurrentState = DesktopClimbingStates.movingPlayer;

                currentGrabIndicator.position = nextGrabIndicator.position;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CurrentState = DesktopClimbingStates.movingPlayer;
        }
    }

    void MovePlayer()
    {
        // Moving player
        Vector3 interactPosition = HeadOffsetPosition(currentGrabDistance);

        stationMover.position += (currentGrabIndicator.position - interactPosition);

        // Handle mouse buttons
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CurrentState = DesktopClimbingStates.movingConnector;
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            CurrentState = DesktopClimbingStates.initialSelection;
        }
    }

    Vector3 HeadOffsetPosition(float offset)
    {
        VRCPlayerApi.TrackingData head = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        return localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation * (offset * Vector3.forward)
            + localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        
        return localPlayer.GetBoneRotation(HumanBodyBones.Head) * (offset * Vector3.forward)
            + localPlayer.GetBonePosition(HumanBodyBones.Head);

        return head.rotation * (offset * Vector3.forward) + head.position;
    }

    public override void OnAvatarChanged(VRCPlayerApi player)
    {
        base.OnAvatarChanged(player);

        RecalculateArmLength();
    }

    public override void OnAvatarEyeHeightChanged(VRCPlayerApi player, float prevEyeHeightAsMeters)
    {
        base.OnAvatarEyeHeightChanged(player, prevEyeHeightAsMeters);

        RecalculateArmLength();
    }

    void RecalculateArmLength()
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

        currentGrabDistance = Mathf.Clamp(currentGrabDistance, armLength * 0.1f, armLength);
    }

    public override void InputLookHorizontal(float value, UdonInputEventArgs args)
    {
        base.InputLookHorizontal(value, args);

        // ToDo: If angle between head and station > Threshold (~90°: Rotate station)
        /*
        if(isUsingStation)
        {
            stationMover.Rotate(Vector3.up, value);
        }
        */
    }
}
