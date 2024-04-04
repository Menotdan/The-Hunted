using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemState : NetworkBehaviour
{
    public class ItemStateData
    {
        public SO_Item item_type;
        public int stack_amount = 1;
    }
    ItemStateData item_data = new ItemStateData();
    public int stack_amount = 1;
    public NetworkVariable<bool> is_held_network = new NetworkVariable<bool>(false);
    public NetworkVariable<int> stack_amount_network = new NetworkVariable<int>(1);
    public SO_Item item_type;

    public ItemStateData GetData()
    {
        return item_data;
    }

    public SO_Item GetItemType()
    {
        return item_data.item_type;
    }

    public int GetStackAmount()
    {
        return item_data.stack_amount;
    }

    private void Awake()
    {
        // These are synced to their backend counterparts so you can set data for these
        // in the inspector, making level design easier.
        item_data.item_type = item_type;
        item_data.stack_amount = stack_amount;
    }

    public override void OnNetworkSpawn()
    {
        // Register the update for all parties.
        stack_amount_network.OnValueChanged += Update_Stack_Amount;
        if (!IsOwner)
            return;

        stack_amount_network.Value = stack_amount;
        base.OnNetworkSpawn();
    }

    // This should run on all clients and the server.
    public void Update_Stack_Amount(int old_value, int new_value)
    {
        stack_amount = new_value;
        item_data.stack_amount = new_value;
    }
}
