using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class InventorySlot : MonoBehaviour {

    [SerializeField] private Image icon = null;

    private Inventory inventory = null;
    private Item item = null;
    private RectTransform slotPanel = null;
    private Button button = null;

    public RectTransform RectTransformData {
        get {
            if (slotPanel == null) {
                slotPanel = GetComponent<RectTransform>();
            }
            return slotPanel;
        }
    }

    public void SetupSlot(Inventory inv) {
        inventory = inv;
        // Fetch icon using the item name. If icon not found => fallback to default icon
        if (icon == null) {
            icon = transform.GetChild(0).GetChild(1).GetComponent<Image>();
            if (icon == null) {
                Debug.LogError("Gameobject "+gameObject.name+" icon variable is null");
                return;
            }
        }
        button = transform.GetChild(0).GetComponent<Button>();
        if (button == null) {
            Debug.LogError("Gameobject "+gameObject.name+" button variable is null");
                return;
        }
    }

    public void SetItem(Item newItem) {
        // If this position doesn't contain an item, clear the slot
        if (newItem == null) {
            ClearSlot();
            return;
        }

        if (icon == null) {
            Debug.LogError("Gameobject "+gameObject.name+" icon variable is null");
            return;
        }
        item = newItem;
        if (icon.sprite == null) {
            icon.sprite = Database.GetIcon(newItem.entityName);
            if (icon.sprite == null) {
                icon.sprite = Database.GetIcon("icon_default");
            }
        }
        icon.enabled = true;
    }

    public void ClearSlot() {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnClick() {
        if (this.item == null)
            return;
        inventory.RemoveItemClient(this.item);
    }

}
