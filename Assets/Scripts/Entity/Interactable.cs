using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : Entity {

    [Server]
    public virtual void OnServerInteract(NetworkIdentity client) {
        RcpInteractionFinish(client);
    }

    [ClientRpc]
    protected virtual void RcpInteractionFinish(NetworkIdentity client) {
        
    } 
}
