using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SellPoint : Interactable {

    [SerializeField][Range(0, 1)] private float returnModifier = 0.5f;

    [Server]
    public override void OnServerInteract(NetworkIdentity client) {
        base.OnServerInteract(client);
        PlayerInventory inv = client.transform.GetComponent<PlayerInventory>();
        float totalMoney = 0;
        for (int i = 0; i < inv.Items.Length; i++) {
            if (inv.Items[i] != null) {
                Item item = inv.Items[i];
                totalMoney += inv.Items[i].Price * returnModifier;
                NetworkServer.Destroy(item.gameObject);
                inv.RemoveItem(client, item);
            }
        }
        EntityManager.LocalPlayer.Player_Wallet.AddMoney(totalMoney);
        RpcPlayFX();
    }

    [ClientRpc]
    public void RpcPlayFX() {
        SoundSystem.PlaySound("ui_transaction", transform.position);
    }

}
