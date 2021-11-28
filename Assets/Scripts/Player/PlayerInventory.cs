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
            if (ui != null)
                EntityManager.LocalPlayer.Player_UI.SetMoneyTextVisibility(ui.gameObject.activeSelf);
        }
    }

    [ClientRpc]
    protected override void RcpRemoveItem(NetworkIdentity client, Item item, int index) {
        base.RcpRemoveItem(client, item, index);
        if (item != null) {
            item.transform.position = transform.position + transform.forward + transform.up;
            item.gameObject.SetActive(true);
            item.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up)*client.GetComponent<PlayerInventory>().dropForce, ForceMode.Impulse);
        }
    }

}
