using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelItemParent : NetworkBehaviour
{
    public bool spawned = false;
    public Action parent_spawned;

    public override void OnNetworkSpawn()
    {
        if (parent_spawned != null)
            parent_spawned();

        spawned = true;
        base.OnNetworkSpawn();
    }
}
