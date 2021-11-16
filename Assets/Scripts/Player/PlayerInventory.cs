using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    
    [SerializeField] private int maxItemCount = 10;
    [SerializeField] private List<Item> itemList = new List<Item>();

    private void Update() {
        
    }

    public void AddItem(Item item) {
        if (itemList.Count == maxItemCount) {
            return;
        }
        itemList.Add(item);
    }

    private void DropItem() {

    }

}
