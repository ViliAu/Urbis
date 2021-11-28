using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : Entity {
    
    [Server]
    public virtual void OnServerInteract(NetworkIdentity client) {
        
    }

    [ClientRpc]
    protected virtual void RpcInteractionFinish(NetworkIdentity client) {
        
    }

    // Local methods
    public virtual void PlayerFocusEnter() {

    }

    public virtual void PlayerFocusExit() {

    }
}
