using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DebugSpawnOrder : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log($"{name} Spawn()ed.");
        base.OnNetworkSpawn();
    }
}
