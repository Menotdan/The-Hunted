using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkApprovalManager : MonoBehaviour
{
    [SerializeField, Anywhere] private Transform spawn_area_transform;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var client_id = request.ClientNetworkId;
        var connection_data = request.Payload;

        // Approve the Request by Default
        response.Approved = true;
        response.CreatePlayerObject = true;

        // Use Default NetworkManger Player Prefab
        response.PlayerPrefabHash = null;

        // Set the Player Spawn
        response.Position = spawn_area_transform.position;
        response.Rotation = spawn_area_transform.rotation;

        // Default Reason Message (We Never Decline Connection, So This Shouldn't Matter)
        response.Reason = "Connection Approved.";
        
        // Set Pending to False in Case the Default is True
        response.Pending = false;
    }
}
