using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;

public abstract class ClimbingActivationInformer : UdonSharpBehaviour
{
    public abstract void ClimbingStart();
    public abstract void ClimbingStop();
}
