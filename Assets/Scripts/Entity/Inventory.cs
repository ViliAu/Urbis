using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Inventory : Interactable {
    
    [SerializeField] private int maxItemCount = 10;
    [SerializeField] private List<Item> itemList = new List<Item>();

    private void Update() {
        
    }

    public virtual void AddItem(Item item) {
        if (itemList.Count == maxItemCount) {
            return;
        }
        itemList.Add(item);
    }

    [ClientRpc]
    protected override void RcpInteractionFinish(NetworkIdentity client) {
        if (client.isLocalPlayer) {
            //client
        }

    }

}
