using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SoldItem : Interactable {
    [SerializeField] private Item itemToSell = null;
    [SerializeField] private float price = 5f;
    [SerializeField] private float restockTime = 30f;
    
    [Server]
    public override void OnServerInteract(NetworkIdentity client) {
        base.OnServerInteract(client);
        PlayerInventory inv = client.GetComponent<PlayerInventory>();
        if (EntityManager.LocalPlayer.Player_Wallet.Balance < price) {
            return;
        }
        EntityManager.LocalPlayer.Player_Wallet.AddMoney(-price);
        GameObject g = Instantiate(itemToSell.gameObject, transform.position, Quaternion.identity);
        NetworkServer.Spawn(g);
        bool success = inv.AddItem(client, g.GetComponent<Item>());
        if (success) {
            RpcSetEntityActive(g, false);
        }
        else {
            RpcThrowItem(g);
        }
        RpcSetEntityActive(gameObject, false);
        Invoke("ActivateSoldItem", restockTime);
        PlayFX();
    }

    [Server]
    private void ActivateSoldItem() {
        RpcSetEntityActive(gameObject, true);
    }

    [ClientRpc]
    private void RpcThrowItem(GameObject g) {
        if (g.GetComponent<Rigidbody>() != null)
            g.GetComponent<Rigidbody>().AddForce(Vector3.up + transform.forward, ForceMode.Impulse);
    }

    [ClientRpc]
    private void PlayFX() {
        SoundSystem.PlaySound("transaction_success", transform.position);
    }
}
