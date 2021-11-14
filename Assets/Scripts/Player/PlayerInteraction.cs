using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInteraction : NetworkBehaviour {

    [SerializeField] private float range = 1.5f;
    [SerializeField] private LayerMask hitMask = default;

    RaycastHit hit;

    private void Update() {
        CheckInput();
    }

    private void CheckInput() {
        if (EntityManager.LocalPlayer.Player_Input.interacted) {
            CastRay();
        }
    }

    private void CastRay() {
        // Check if we hit something
        if (Physics.Raycast(EntityManager.LocalPlayer.Player_Camera.head.position, EntityManager.LocalPlayer.Player_Camera.head.forward, out hit, range, hitMask)) {
            // Check if it was interactable
            if (hit.transform.GetComponent<Interactable>()){
                // Check if it has a network identity component 
                if (hit.transform.GetComponent<NetworkIdentity>()) {
                    CmdInteractionServer(transform.GetComponent<NetworkIdentity>(), hit.transform.GetComponent<NetworkIdentity>());
                }
            }
        }
    }

    [Command]
    private void CmdInteractionServer(NetworkIdentity client, NetworkIdentity netID) {
        Interactable intera = netID.GetComponent<Interactable>();
        intera.OnServerInteract(client);
    }

}
