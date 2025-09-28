
using Nessie.Udon.Movement;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(VRCPickup))]
public class NUThrowTeleport : UdonSharpBehaviour
{
    Vector3 origin;

    Rigidbody linkedRigidbody;
    Collider linkedCollider;
    bool teleportOnImpact = false;

    [SerializeField] NUMovement linkedNUMovement;

    void Start()
    {
        linkedRigidbody = GetComponent<Rigidbody>();
        linkedCollider = GetComponent<Collider>();
        origin = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!teleportOnImpact)
            return;

        Vector3 normal = collision.GetContact(0).normal;

        /*
        // Angle check
        if(Vector3.Angle(normal, Vector3.up) > 45)
            return;
        */

        /*
        // Enough space check
        linkedCollider.enabled = false;
        // Player height: 1.65m, radius = 0.4m
        bool invalid = Physics.CheckCapsule(
            transform.position + 0.4f * Vector3.up,
            transform.position + 1.25f * Vector3.up,
            0.4f);
        linkedCollider.enabled = true;

        if (invalid)
            return;
        */

        linkedNUMovement._TeleportTo(transform.position);

        transform.position = origin;
        linkedRigidbody.velocity = Vector3.zero;
        linkedRigidbody.angularVelocity = Vector3.zero;

        teleportOnImpact = false;
    }

    public override void OnPickup() // Fired when this object is picked up by the local player.
    {

    }

    public override void OnDrop() // Fired when the local player drops this object after being held.
    {
        teleportOnImpact = true;
    }
}
