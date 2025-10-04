
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleGameObject : UdonSharpBehaviour
{
    [SerializeField] GameObject[] linkedObjects;

    public override void Interact()
    {
        base.Interact();

        foreach(GameObject go in linkedObjects)
        {
            go.SetActive(!go.activeSelf);

            Debug.Log($"{go.name}: {go.activeSelf}");
        }
    }
}
