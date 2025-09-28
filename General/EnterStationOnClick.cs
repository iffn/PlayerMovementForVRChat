
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class EnterStationOnClick : UdonSharpBehaviour
{
    [SerializeField] VRCStation linkedStation;

    public override void Interact()
    {
        linkedStation.UseStation(Networking.LocalPlayer);
    }
}
