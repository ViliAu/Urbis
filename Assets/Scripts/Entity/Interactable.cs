using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour {
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void OnServerInteract(NetworkIdentity client) {
        RcpInteractionFinish(client);
    }

    [ClientRpc]
    public void RcpInteractionFinish(NetworkIdentity client) {
        if (!client.isLocalPlayer) {
            return;
        }
        
    } 
}
