
using Nessie.Udon.Movement;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class NUWallAndMultiJump : UdonSharpBehaviour
{
    [SerializeField] NUMovement linkedNUMovement;

    [SerializeField] float detectDistance = 0.4f;
    [SerializeField] float rayRadius = 0.2f;

    [SerializeField] float verticalJumpMultiplier = 1.5f;
    [SerializeField] float noramlJumpMultiplier = 2f;

    [SerializeField] int airJumps = 2;

    int currentAirJumpsRemaining;

    private void Update()
    {
        if (linkedNUMovement._IsPlayerGrounded())
        {
            currentAirJumpsRemaining = airJumps;
        }
    }

    public override void InputJump(bool value, UdonInputEventArgs args)
    {
        base.InputJump(value, args);

        if (linkedNUMovement._IsPlayerGrounded())
            return;

        if (value)
        {
            if (!TryWallJump())
                MultiJump();
        }
    }

    void MultiJump()
    {
        if (currentAirJumpsRemaining <= 0)
            return;

        Vector3 currentVelocity = linkedNUMovement._GetVelocity();
        
        currentVelocity.y = linkedNUMovement._GetJumpImpulse();

        linkedNUMovement._SetVelocity(currentVelocity);

        currentAirJumpsRemaining--;
    }

    bool TryWallJump()
    {
        Vector3 lookDirection = linkedNUMovement._GetRotation() * Vector3.forward;

        Vector3 currentPosition = linkedNUMovement._GetPosition();

        Ray ray = new Ray(currentPosition, lookDirection);

        if (Physics.SphereCast(ray, rayRadius, out RaycastHit hit, detectDistance))
        {
            Debug.LogWarning("Wall jump");
            Vector3 tangentVelocity = Vector3.ProjectOnPlane(lookDirection, hit.normal);

            Vector3 outputVelocity = tangentVelocity
                + linkedNUMovement._GetJumpImpulse() * noramlJumpMultiplier * hit.normal;

            outputVelocity.y = linkedNUMovement._GetJumpImpulse() * verticalJumpMultiplier;

            linkedNUMovement._SetVelocity(outputVelocity);

            return true;
        }
        else
        {
            Debug.LogWarning("Unable to wall jump");
            return false;
        }
    }
}
