using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInventory : Inventory {

    public float dropForce = 2f;

    private void Update() {
        CheckInput();    
    }

    private void CheckInput() {
        if (EntityManager.LocalPlayer.Player_Input.tabbed) {
            ToggleUI();
        }
    }

    /*
    [Server]
    public override void RemoveItem(NetworkIdentity client, Item item) {
        base.RemoveItem(client, item);
        item.transform.position = transform.position + transform.forward + transform.up;
        item.RcpSetEntityActive(item.gameObject, true);
        item.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up)*dropForce, ForceMode.Impulse);
    }*/

}
