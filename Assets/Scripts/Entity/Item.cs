using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : Interactable {
    
    [ClientRpc]
    protected override void RcpInteractionFinish(NetworkIdentity client) {
        // Hide the object from everyone
        gameObject.SetActive(false);

        // Add the object to the player's inventory
        PlayerInventory inv = client.transform.GetComponent<PlayerInventory>();
        inv.AddItem(this);
    }
    
}
