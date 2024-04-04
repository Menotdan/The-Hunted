using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [HideInInspector] public Vector3 input_movement = Vector3.zero;

    // Handle Movement Events
    public void MovementEvent(InputAction.CallbackContext ctx)
    {
        Vector2 input_val = ctx.ReadValue<Vector2>();
        input_movement = new Vector3(input_val.x, 0f, input_val.y);
    }
}
