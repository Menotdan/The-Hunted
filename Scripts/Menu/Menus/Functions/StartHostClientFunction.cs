using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartHostClientFunction : MenuFunction
{
    [SerializeField] private NetworkApprovalManager connection_approval_manager;
    [SerializeField] private Button start_host_button;
    [SerializeField] private Button start_client_button;

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += connection_approval_manager.ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public override void Function_Loaded()
    {
        base.Function_Loaded();
        start_host_button.onClick.AddListener(StartHost);
        start_client_button.onClick.AddListener(StartClient);
    }
}
