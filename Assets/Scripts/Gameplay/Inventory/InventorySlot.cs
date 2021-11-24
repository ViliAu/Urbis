using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    [SerializeField] private Image icon = null;
    private Item item = null;

    private RectTransform slotPanel = null;

    public RectTransform RectTransformData {
        get {
            if (slotPanel == null) {
                slotPanel = GetComponent<RectTransform>();
            }
            return slotPanel;
        }
    }

    public void AddItem(Item newItem) {
        item = newItem;
        // Fetch icon using the item name. If icon not found => fallback to default icon
        if (icon != null && icon.sprite == null) {
            Database.GetIcon(newItem.entityName);
            if (icon.sprite == null) {
                Database.GetIcon("icon_default");
            }
            icon.enabled = true;
        }
    }

    public void ClearSlot() {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

}
