using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Inventory : NetworkBehaviour {
    
    [SerializeField] private int maxItemCount = 10;
    [SerializeField] private Item[] items = null;
    [SerializeField] private InventoryUI uiPrefab = null;

    protected InventoryUI ui = null;

    public Item[] Items {
        get {
            return items;
        }
    }
    
    public void Awake() {
        // TODO: Selvitä ja rework miks tulee null
        items = new Item[maxItemCount];
    }

    // Calculate if we can add an item to the inventory
    [Server]
    public virtual bool AddItem(NetworkIdentity client, Item item) {
        Inventory inv = client.GetComponent<Inventory>();
        // TODO: Append stack
        for (int i = 0; i < inv.Items.Length; i++) {
            if (inv.Items[i] == null) {
                // There's space therefore add item
                inv.Items[i] = item;
                RpcAddItem(client, item, i);
                return true;
            }
        }
        return false;
    }

    [ClientRpc]
    private void RpcAddItem(NetworkIdentity client, Item item, int index) {
        if (!client.isLocalPlayer) {
            return;
        }
        Inventory inv = client.GetComponent<Inventory>();
        inv.items[index] = item;
        inv.UpdateUI();
    }

    /// <summary>
    /// Adds an item to the given index if possible and disables it
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="index">Index in which the item is added</param>
    /// <returns>Return true or false depending on if the item got added to the inventory</returns>
    public virtual bool AddItem(Item item, int index) {
        // TODO: Append stack
        if (index > maxItemCount) {
            return false;
        }
        if (items[index] != null) {
            items[index] = item;
            UpdateUI();
            return true;
        }
        return false;
    }

    // Request from client to remove an item
    [Command]
    public virtual void RemoveItemClient(Item item) {
        RemoveItem(transform.GetComponent<NetworkIdentity>(), item);
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="item">Item to remove</param>
    [Server]
    public virtual void RemoveItem(NetworkIdentity client, Item item) {
        Inventory inv = client.GetComponent<Inventory>();
        Debug.Log("INVIN LENTTI "+inv.Items.Length);
        for (int i = 0; i < inv.Items.Length; i++) {
            if (inv.Items[i] == item) {
                inv.Items[i] = null;
                RcpRemoveItem(client, item, i);
            }
        }
    }

    [ClientRpc]
    protected virtual void RcpRemoveItem(NetworkIdentity client, Item item, int index) {
        Inventory inv = client.GetComponent<Inventory>();
        if (index > inv.Items.Length)
            return;
        if (client.isLocalPlayer) {
            inv.items[index] = null;
            inv.UpdateUI();
        }
    }

    // Wrapper functions for the ui
    protected void ToggleUI() {
        if (ui == null) {
            SetupUI();
            if (ui == null) {
                return;
            }
        }
        ui.gameObject.SetActive(!ui.gameObject.activeSelf);
        Cursor.visible = ui.gameObject.activeSelf;
        Cursor.lockState = Cursor.visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    protected void SetupUI() {
        if (uiPrefab != null) {
            ui = Instantiate<InventoryUI>(uiPrefab, Vector3.zero, Quaternion.identity, EntityManager.LocalPlayer.Player_UI.canvas);
            ui.SetupInventoryUI(this);
            ui.gameObject.SetActive(false);
        }
    }

    protected void UpdateUI() {
        if (ui == null) {
            SetupUI();
            if (ui == null) {
                return;
            }
        }
        ui.UpdateUI();
    }
}
