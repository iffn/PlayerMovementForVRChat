
using Nessie.Udon.Movement;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[DefaultExecutionOrder(-1)]
public class NUTrampoline : AbstractMovementCollider
{
    public float velocityMultiplier = 1f;

    protected override void OnControllerTriggerEnter(NUMovement controller)
    {
        Vector3 currentVelocity = controller._GetVelocity();

        Debug.LogWarning(currentVelocity);

        if(currentVelocity.y < -0.1f)
        {
            controller._SetVelocity(new Vector3(currentVelocity.x, -currentVelocity.y * velocityMultiplier, currentVelocity.z));
        }
    }
}
