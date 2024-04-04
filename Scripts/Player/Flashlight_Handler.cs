using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Flashlight_Handler : NetworkBehaviour
{
    [SerializeField, Self] public Transform flashlight_transform;
    [SerializeField, Child] private Light flashlight_light;
    public NetworkVariable<bool> flashlight_enabled = new NetworkVariable<bool>(true, writePerm: NetworkVariableWritePermission.Owner);

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Start()
    {
        flashlight_enabled.OnValueChanged += Changed;
    }

    public void Toggle_Flashlight()
    {
        flashlight_enabled.Value = !flashlight_enabled.Value;
        flashlight_light.enabled = flashlight_enabled.Value;
    }

    public void Changed(bool old_value, bool new_value)
    {
        flashlight_light.enabled = new_value;
    }

    public void Move_Flashlight(Quaternion applied_rotation)
    {
        flashlight_transform.rotation = applied_rotation;
        flashlight_transform.localEulerAngles = 
            new Vector3(flashlight_transform.localEulerAngles.x - 082f,
                                                                  187f,
                            flashlight_transform.localEulerAngles.z);
    }
}
