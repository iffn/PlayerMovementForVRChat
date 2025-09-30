
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum DesktopClimbingStates
{
    idle,
    initialSelection,
    movingPlayer,
    movingConnector
}

public class DesktopClimbing : UdonSharpBehaviour
{
    [SerializeField] Transform currentGrabIndicator;
    [SerializeField] Transform nextGrabIndicator;

    [SerializeField] MeshRenderer nextGrabRenderer;
    [SerializeField] Material validGrabMaterial;
    [SerializeField] Material invalidGrabMaterial;

    [SerializeField] float rayDistance = 0.1f;
    [SerializeField] float normalOffsetAngleDegThreshold = 10f;

    VRCPlayerApi localPlayer;
    float armLength;
    float currentGrabDistance;
    float currentPlayerOffset;

    Vector3 teleportLocation;

    DesktopClimbingStates currentState;

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
                    break;
                case DesktopClimbingStates.initialSelection:
                    currentGrabIndicator.gameObject.SetActive(false);
                    nextGrabIndicator.gameObject.SetActive(true);
                    break;
                case DesktopClimbingStates.movingPlayer:
                    currentGrabIndicator.gameObject.SetActive(true);
                    nextGrabIndicator.gameObject.SetActive(false);
                    teleportLocation = localPlayer.GetPosition();
                    break;
                case DesktopClimbingStates.movingConnector:
                    currentGrabIndicator.gameObject.SetActive(true);
                    nextGrabIndicator.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            currentState = value;
        }
    }

    void Start()
    {
        localPlayer = Networking.LocalPlayer;

        RecalculateArmLenght();
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
                TeleportPlayer();
                break;
            case DesktopClimbingStates.movingConnector:
                CheckGrabConnection();
                PositionIndicator();
                TeleportPlayer();
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

    void TeleportPlayer()
    {
        localPlayer.TeleportTo(teleportLocation, localPlayer.GetRotation());
    }

    void MovePlayer()
    {
        // Moving player
        Vector3 interactPosition = HeadOffsetPosition(currentGrabDistance);

        teleportLocation += (nextGrabIndicator.position - currentGrabIndicator.position);

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
