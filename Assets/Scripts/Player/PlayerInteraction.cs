using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInteraction : NetworkBehaviour {

    public float range = 3f;
    public LayerMask interactionMask = default;
    public Rigidbody interactRig;

    private Camera cam;
    private RaycastHit hit;

    public Interactable interactable {get; private set;}
    public Vector3 interactionPoint {get; private set;}
    public Ray ray {get; private set;}

    private void Update() {
        CastInteractRay();
    }

    private void CastInteractRay() {
        ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, range, interactionMask, QueryTriggerInteraction.Collide)) {
            Interactable intera = null;
            interactionPoint = hit.point;
            // Get intera for interacting / focusing
            if ((intera = hit.transform.GetComponent<Interactable>()) != null) {
                /*if (intera.isGrabbable) {
                    interactRig = intera.rig;
                }*/

                // If the interactable thing exists and we currently are not focusing on an interactable object
                if (interactable == null) {
                    interactable = intera;
                    interactable.PlayerFocusEnter();
                }

                // If the interactable thing is same as ours
                else if (intera == interactable) {
                    if (EntityManager.LocalPlayer.Player_Input.interacted)
                        CmdInteraction();
                }

                // If we lose focus and are focusing on a new interactable
                else if (intera != interactable) {
                    interactable.PlayerFocusExit();
                    interactable = intera;
                    interactable.PlayerFocusEnter();
                }
            }
            // We aren't focusing on anything interactable
            else {
                LoseFocus();
            }
        }
        // We aren't hitting or focusing on anything
        else {
            LoseFocus();
        }
    }
    
    private void LoseFocus() {
        /*EntityManager.LocalPlayer.Player_UI.SetCrosshairDarkness(0.75f);
        EntityManager.LocalPlayer.Player_UI.SetCrosshair("crosshair_dot");
        EntityManager.LocalPlayer.Player_UI.SetFocusText("");*/
        if (interactable != null)
            interactable.PlayerFocusExit();
        interactable = null;
        interactRig = null;
    }

    /// <summary>
    /// Used for example when changing while looking at a food table
    /// </summary>
    public void UpdateFocus() {
        if (interactable != null)
            interactable.PlayerFocusEnter();
    }

    /*[Command]
    private void CmdEnterFocus(NetworkIdentity client, NetworkIdentity interactedObject) {
        Interactable intera = interactedObject.GetComponent<Interactable>();
        intera.OnServerHighlight(client);
    }*/

    [Command]
    private void CmdInteraction() {
        //Interactable intera = interactedObject.GetComponent<Interactable>();
        interactable.OnServerInteract(transform.GetComponent<NetworkIdentity>());
    }

}
