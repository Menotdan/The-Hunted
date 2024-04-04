using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelItemSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject LevelItemPrefab;
    [SerializeField] private bool Spawn_In_Parent = true;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            base.OnNetworkSpawn();
            gameObject.SetActive(false);
            return;
        }

        if (!Spawn_In_Parent)
        {
            Spawn_Request();

            base.OnNetworkSpawn();
            return;
        }

        LevelItemParent parent_object = GetComponentInParent<LevelItemParent>();
        if (parent_object == null)
        {
            Debug.LogWarning("No Parent!");
        } else
        {
            if (parent_object.spawned)
            {
                Spawn_Request();
                base.OnNetworkSpawn();

                return;
            } else
            {
                parent_object.parent_spawned += Spawn_Request;
            }
        }

        base.OnNetworkSpawn();
    }

    public void Spawn_Request()
    {
        GameObject spawned_item = Instantiate(LevelItemPrefab);
        NetworkObject spawned_item_NO = spawned_item.GetComponent<NetworkObject>();
        spawned_item_NO.Spawn();

        bool base_parenting_successful = spawned_item_NO.TrySetParent(NetworkObject);
        if (!base_parenting_successful)
            Debug.LogWarning($"Base parenting for spawner {name} failed!");

        spawned_item.transform.localPosition = Vector3.zero;
        spawned_item.transform.localEulerAngles = Vector3.zero;

        NetworkObject parent_object_NO;
        if (Spawn_In_Parent)
        {
            parent_object_NO = gameObject.transform.parent.GetComponent<NetworkObject>();
        } else
        {
            parent_object_NO = null;
        }

        bool parenting_successful = spawned_item_NO.TrySetParent(parent_object_NO);
        if (!parenting_successful)
            Debug.LogWarning($"Parenting for spawner {name} failed! Attempted parent: {gameObject.transform.parent.name}.");

        gameObject.SetActive(false);
    }
}
