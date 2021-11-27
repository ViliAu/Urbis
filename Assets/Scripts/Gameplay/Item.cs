using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : Interactable {
    
    [SerializeField] private float price = 1;

    public float Price {
        get {
            return price;
        }
    }

    [Server]
    public override void OnServerInteract(NetworkIdentity client) {
        PlayerInventory inv = client.transform.GetComponent<PlayerInventory>();
        bool success = inv.AddItem(client, this);
        RpcSetEntityActive(gameObject, !success);
        RpcInteractionFinish(client);
    }
    
}
