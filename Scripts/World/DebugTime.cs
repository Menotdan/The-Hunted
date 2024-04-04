using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTime : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private float time;

    void Update()
    {
        time += Time.deltaTime;
        _light.transform.rotation = Quaternion.Euler(time*5f, 0, 0);
    }
}
