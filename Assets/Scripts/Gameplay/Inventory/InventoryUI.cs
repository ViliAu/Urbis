using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    
    private Inventory inventory = null;
    [SerializeField] private InventorySlot slotPrefab = null;

    [Header("Grid settings")]
    [SerializeField][Range(1, 10)] int columns = 2;
    [SerializeField] private Vector2 gridStartPoint = default;
    [SerializeField] private Vector2 gridGaps = default;
    
    [Tooltip("Where the inv is spawned")]
    public Vector2 spawnOffset = Vector2.zero;

    [SerializeField] private InventorySlot[] slots = null;
    private Vector2 slotPrefabDimensions = Vector2.zero;
    private RectTransform slotParent = null;
    private RectTransform uiPanel = null;

    public void SetupInventoryUI(Inventory inv) {
        uiPanel  = transform.GetComponent<RectTransform>();
        uiPanel.anchoredPosition = Vector3.zero;
        inventory = inv;
        SpawnInventorySlots();
    }

    private void SpawnInventorySlots() {
        if (slotPrefab == null) {
            Debug.LogError("No slot prefab assigned for "+gameObject.name);
            return;
        }
        slots = new InventorySlot[inventory.Items.Length];
        slotPrefabDimensions = slotPrefab.GetComponent<RectTransform>().sizeDelta;
        slotParent = new GameObject("Slots").AddComponent<RectTransform>();
        slotParent.SetParent(transform);
        slotParent.anchoredPosition = Vector3.zero;

        for (int i = 0; i < inventory.Items.Length; i++) {
            InventorySlot clone = Instantiate<InventorySlot>(slotPrefab, Vector3.zero, Quaternion.identity, slotParent);
            clone.SetupSlot(inventory);

            // Calculate slot position in grid
            int currentColumn = Mathf.FloorToInt(i % (columns));
            int currentRow = Mathf.FloorToInt(i / columns);
            Vector2 pos = new Vector2(currentColumn * slotPrefabDimensions.x + currentColumn * gridGaps.x,
                currentRow * -slotPrefabDimensions.y + currentRow * -gridGaps.y);
            pos += slotParent.anchoredPosition + gridStartPoint;
            clone.RectTransformData.anchoredPosition = pos;

            slots[i] = clone;
        }

        int rows = Mathf.CeilToInt((float)inventory.Items.Length / (float)columns);
        // Move slosts so that they are centered
        foreach (InventorySlot s in slots) {
            s.RectTransformData.anchoredPosition += new Vector2(
                - slotPrefabDimensions.x * Mathf.CeilToInt((columns - 1) / 2)
                - (columns % 2 == 0 ? slotPrefabDimensions.x / 2 : 0)
                - gridGaps.x * Mathf.CeilToInt((columns - 1) / 2)
                - (columns % 2 == 0 ? gridGaps.x / 2 : 0),

                slotPrefabDimensions.y * Mathf.CeilToInt((rows - 1) / 2)
                + (rows % 2 == 0 ? slotPrefabDimensions.y / 2 : 0)
                + gridGaps.y * Mathf.CeilToInt((rows - 1) / 2)
                + (rows % 2 == 0 ? gridGaps.y / 2 : 0));
        }
        UpdateUI();
    }
    
    public void UpdateUI(int index, Item item) {
        slots[index].SetItem(item);
    }

    public void UpdateUI() {
        for (int i = 0; i < inventory.Items.Length; i++) {
            slots[i].SetItem(inventory.Items[i]);
        }
    }

}
