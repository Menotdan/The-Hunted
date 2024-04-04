using Assets._Scripts.PlayerController.Inventory;
using Assets.Scripts.Utility;
using KBCore.Refs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : NetworkBehaviour
{
    public LayerMask mask;
    public ItemState held_item;
    public int inventory_slot_held = 0; // Planning to limit this to 1-3 (0-2 index)
    [SerializeField, Anywhere] GameObject item_hand;

    private Camera main_camera;
    private Inventory player_inventory;

    private const float inventory_grab_range = 5f;
    private const float throw_force = 2f;
    private Vector3 player_hand_position = new Vector3(0.403f, 0.302f, 0.737f);
    private Vector3 player_hand_rotation = new Vector3(351.538f, 0f, 0f);

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        main_camera = Camera.main;
        player_inventory = new Inventory();

        // Called when our inventory updates
        player_inventory.inventory_update += Inventory_Update;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Spawn_Item_Hand_ServerRpc(OwnerClientId);
        }

        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = true)]
    public void Spawn_Item_Hand_ServerRpc(ulong client_id)
    {
        item_hand = Instantiate(item_hand);
        NetworkObject item_hand_NO = item_hand.GetComponent<NetworkObject>();
        item_hand_NO.Spawn(true);

        item_hand_NO.TrySetParent(gameObject.GetComponent<NetworkObject>());
        item_hand.transform.localPosition = player_hand_position;
        item_hand.transform.localEulerAngles = player_hand_rotation;
    }

    [ServerRpc(RequireOwnership = true)]
    public void Spawn_Held_Item_ServerRpc(int item_slot, ulong client_id, int stack_amount)
    {

        SO_Item item_type = SO_Item_List.Instance.items_list[item_slot];
        GameObject item_holding = Instantiate(item_type.item_prefab, item_hand.transform);

        item_holding.transform.localPosition = Vector3.zero;
        item_holding.transform.localEulerAngles = Vector3.zero;

        // Make sure to instantiate the object on the other clients.
        NetworkObject item_NO = item_holding.GetComponent<NetworkObject>();
        NetworkObject item_hand_NO = item_hand.gameObject.GetComponent<NetworkObject>();
        item_NO.Spawn(true);
        ItemState grabbed_item_state = item_NO.GetComponent<ItemState>();
        grabbed_item_state.stack_amount_network.Value = stack_amount;
        grabbed_item_state.is_held_network.Value = true;

        item_NO.TrySetParent(item_hand_NO);
        item_NO.ChangeOwnership(client_id);

        ClientRpcParams client_rpc_params = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { client_id }
            }
        };
        Set_Held_Item_ClientRpc(true, new NetworkObjectReference(item_NO), client_rpc_params);
    }

    [ServerRpc(RequireOwnership = true)]
    public void Despawn_Grabbed_Item_ServerRpc(NetworkObjectReference object_reference)
    {
        NetworkObject item_NO = null;
        if (!object_reference.TryGet(out item_NO))
            Debug.LogError("Failed to Despawn Requested Object. Malicious Client?");

        item_NO.Despawn();
        Destroy(item_NO.gameObject);
    }

    [ServerRpc(RequireOwnership = true)]
    public void Throw_Item_ServerRpc(NetworkObjectReference object_reference, int throw_count, ulong client_id)
    {
        NetworkObject thrown_object_NO = null;
        if (!object_reference.TryGet(out thrown_object_NO))
            Debug.LogError("Failed to Throw Requested Object. Malicious Client?");

        ItemState thrown_item_state = thrown_object_NO.GetComponent<ItemState>();
        SO_Item item_data = thrown_item_state.item_type;

        int stack_amount = thrown_item_state.stack_amount_network.Value;
        thrown_item_state.stack_amount_network.Value -= throw_count;
        
        // If value is less than 1, set it to 1 so that the object that we
        // drop from our inventory has the right stack amount
        thrown_item_state.stack_amount_network.Value = Mathf.Max(thrown_item_state.stack_amount_network.Value, 1);

        // If the amount we are throwing is lower than the stack amount,
        // create a new object that gets thrown so you still hold the extra items
        if (throw_count < stack_amount)
        {
            GameObject thrown_prefab = Instantiate(item_data.item_prefab);

            Vector3 position = thrown_object_NO.transform.position;
            Vector3 rotation = thrown_object_NO.transform.eulerAngles;

            thrown_object_NO = thrown_prefab.GetComponent<NetworkObject>();
            thrown_object_NO.Spawn();
            thrown_object_NO.GetComponent<ItemState>().stack_amount_network.Value = throw_count;

            thrown_object_NO.transform.position = position;
            thrown_object_NO.transform.eulerAngles = rotation;
        } else // Otherwise tell the client to delete its held item state
        {
            ClientRpcParams client_rpc_params = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { client_id }
                }
            };
            Set_Held_Item_ClientRpc(false, object_reference, client_rpc_params);
        }

        // Try to claim the thrown object, if its new or if its the old one
        thrown_object_NO.RemoveOwnership();
        thrown_object_NO.TryRemoveParent();
        thrown_object_NO.GetComponent<ItemState>().is_held_network.Value = false;

        // Apply force to the object
        Rigidbody thrown_rigidbody = thrown_object_NO.GetComponent<Rigidbody>();
        thrown_rigidbody.isKinematic = false;
        thrown_rigidbody.useGravity = true;
        thrown_rigidbody.AddForce(thrown_rigidbody.transform.forward * throw_force, ForceMode.VelocityChange);
    }

    [ClientRpc]
    public void Set_Held_Item_ClientRpc(bool keep_old_item, NetworkObjectReference object_reference, ClientRpcParams client_rpc_params = default)
    {
        // If the server is requesting we delete our old item
        if (!keep_old_item)
        {
            held_item = null;
            return;
        }

        NetworkObject new_held_item;
        if (!object_reference.TryGet(out new_held_item))
            return;

        // Set rigidbody to not use physics while you are holding it
        Rigidbody item_holding_RB = new_held_item.GetComponent<Rigidbody>();
        item_holding_RB.isKinematic = true;
        item_holding_RB.useGravity = false;

        // Set the new held item
        held_item = new_held_item.GetComponent<ItemState>();
    }

    public void GrabItem(GameObject item)
    {
        GameObject real_object = item;

        ItemState item_state;

        // Loop upwards until we find the GameObject with ItemState attached.
        while (!real_object.TryGetComponent(out item_state))
        {
            real_object = real_object.transform.parent.gameObject;
        }

        if (item_state.is_held_network.Value)
        {
            return;
        }

        player_inventory.Add_New_Item(item_state);
        Despawn_Grabbed_Item_ServerRpc(new NetworkObjectReference(real_object.GetComponent<NetworkObject>()));
    }

    public void Pickup_Pressed(InputAction.CallbackContext ctx)
    {
        // Don't run if this isn't a button down, OR if we dont own this instance of the behaviour.
        if (!ctx.performed || !IsOwner)
            return;

        RaycastHit hit;
        Ray ray = main_camera.ScreenPointToRay(new Vector2((Screen.width / 2), (Screen.height / 2)));

        if (Physics.Raycast(ray, out hit, inventory_grab_range, mask)) // Do a Raycast with only layer 8 (Items)
        {
            Transform hit_transform = hit.transform;
            GrabItem(hit_transform.gameObject);
        }
    }

    public void Throw_Pressed(InputAction.CallbackContext ctx)
    {
        // Don't run if this isn't a button down, or if we dont
        // own this instance of the behaviour, or if there is
        // no currently held item.
        if (!ctx.performed || !IsOwner || held_item == null)
            return;

        int throw_count = 1;
        NetworkObject throwing_item_NO = held_item.GetComponent<NetworkObject>();

        // Always subtract the inventory
        ItemState.ItemStateData item_data_state = player_inventory.GetSlot(inventory_slot_held).item;
        item_data_state.stack_amount -= throw_count;
        
        if (item_data_state.stack_amount <= 0)
        {
            player_inventory.GetSlot(inventory_slot_held).DeleteItem();
        }


        Throw_Item_ServerRpc(new NetworkObjectReference(throwing_item_NO), throw_count, OwnerClientId);
    }

    // Update the item the player is holding when the slot is updated.
    public void Inventory_Update(int slot_updated)
    {
        if (slot_updated != inventory_slot_held)
            return;

        // If the player was already holding something here.
        if (held_item != null)
        {
            GameObject held_item_GO = held_item.gameObject;
            Despawn_Grabbed_Item_ServerRpc(new NetworkObjectReference(held_item_GO.GetComponent<NetworkObject>()));

            held_item = null;
        }

        ItemState.ItemStateData data = player_inventory.GetSlot(slot_updated).item;
        if (data == null)
            return;

        if (data.item_type.is_holdable)
        {
            Spawn_Held_Item_ServerRpc(SO_Item_List.Instance.items_list.IndexOf(data.item_type), OwnerClientId, data.stack_amount);
        }
    }
}
