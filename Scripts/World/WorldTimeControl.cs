using Assets._Scripts.World;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class WorldTimeControl : NetworkBehaviour
{
    [SerializeField] private Material skybox_material;
    [SerializeField] private Transform sun_transform;
    [SerializeField] private Light sun_light;
    [SerializeField] private List<TimeCycle.TimeSection> time_cycle_data = new List<TimeCycle.TimeSection>();

    private TimeCycle time_cycle;

    private const float sun_rotation_min = -8f;
    private const float sun_rotation_max = 192f;

    [SerializeField] private NetworkVariable<float> world_time = new NetworkVariable<float>(0f);

    private void Awake()
    {
        time_cycle = new TimeCycle();
        time_cycle.sections = time_cycle_data;
        time_cycle.Configure_Max_Time();
    }

    private void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        if (IsServer)
        {
            world_time.Value += Time.deltaTime;
            if (world_time.Value >= time_cycle.max_time)
            {
                world_time.Value = 0f;
            }
        }

        if (!IsClient)
        {
            return;
        }

        time_cycle.Set_Properties_For_Time(world_time.Value, skybox_material, sun_transform, sun_light);
    }
}
