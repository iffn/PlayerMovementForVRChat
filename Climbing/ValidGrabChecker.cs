using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;

public abstract class ValidGrabChecker : UdonSharpBehaviour
{
    public abstract bool CheckValidGrabPoint(Vector3 worldPosition);
}
