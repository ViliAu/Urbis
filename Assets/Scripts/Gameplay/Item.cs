using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : Interactable {
    
    [Server]
    public override void OnServerInteract(NetworkIdentity client) {
        PlayerInventory inv = client.transform.GetComponent<PlayerInventory>();
        bool success = inv.AddItem(client, this);
        RpcSetEntityActive(gameObject, !success);
        RpcInteractionFinish(client);
    }
    
}
