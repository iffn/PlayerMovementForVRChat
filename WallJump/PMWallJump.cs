
using Nessie.Udon.Movement;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PMWallJump : UdonSharpBehaviour
{
    [SerializeField] NUMovement linkedNUMovement;

    [SerializeField] float detectDistance = 0.4f;
    [SerializeField] float rayRadius = 0.2f;

    [SerializeField] float verticalJumpMultiplier = 1f;
    [SerializeField] float noramlJumpMultiplier = 1f;

    public override void InputJump(bool value, UdonInputEventArgs args)
    {
        base.InputJump(value, args);

        if (linkedNUMovement._IsPlayerGrounded())
            return;

        if (value)
        {
            TryWallJump();
        }
    }

    void TryWallJump()
    {
        Vector3 currentVelocity = linkedNUMovement._GetVelocity();

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
        }
        else
        {
            Debug.LogWarning("Unable to wall jump");
        }
    }
}
