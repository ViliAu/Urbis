using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Inventory : Interactable {
    
    [SerializeField] private int maxItemCount = 10;
    [SerializeField] private Item[] items = null;
    [SerializeField] private InventoryUI uiPrefab = null;

    private InventoryUI ui = null;

    public Item[] Items {
        get {
            return items;
        }
    }

    private void Awake() {
        // TODO: Selvitä ja rework miks tulee null
        if (EntityManager.LocalPlayer == null) {
            Invoke("Awake", 0.1f);
            return;
        }
        items = new Item[maxItemCount];
        if (uiPrefab != null) {
            ui = Instantiate<InventoryUI>(uiPrefab, Vector3.zero, Quaternion.identity, EntityManager.LocalPlayer.Player_UI.canvas);
            ui.SetupInventoryUI(this);
            ui.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Adds an item to the first free stack or slot available and disables it
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns>Return true or false depending on if the item got added to the inventory</returns>
    public virtual bool AddItem(Item item) {
        // TODO: Append stack
        for (int i = 0; i < items.Length; i++) {
            if (items[i] == null) {
                items[i] = item;
                item.gameObject.SetActive(false);
                return true;
            }
        }
        return false;
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
            item.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    protected void ToggleUI() {
        if (ui != null)
            ui.gameObject.SetActive(!ui.gameObject.activeSelf);
    }

    [ClientRpc]
    protected override void RcpInteractionFinish(NetworkIdentity client) {
        if (client.isLocalPlayer) {
            //client
        }
    }

}
