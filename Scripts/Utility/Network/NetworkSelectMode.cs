using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class NetworkSelectMode : MonoBehaviour
{
    [SerializeField, Self] private NetworkApprovalManager connection_approval_manager;
    NetworkManager _net_manager;

    private void Awake()
    {
        _net_manager = GetComponent<NetworkManager>();
    }

    public void Select_Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += connection_approval_manager.ApprovalCheck;
        //Debug.Log($"[Netcode] Current Thread: {Thread.CurrentThread.Name}");
        _net_manager.StartHost();
    }

    public void Select_Client()
    {
        _net_manager.StartClient();
    }
}
