using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : Interactable {
    
    [ClientRpc]
    protected override void RcpInteractionFinish(NetworkIdentity client) {
        gameObject.SetActive(false);

        if (client.isLocalPlayer) {
            PlayerInventory inv = client.transform.GetComponent<PlayerInventory>();
            inv.AddItem(this);
        }
    }

}
