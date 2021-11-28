using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Trashcan : Interactable {

    [SerializeField] private DropTable dropTable = null;
    [SerializeField] private Vector2 resetTimeRange = Vector2.up * 10;
    [SerializeField] private Material highlightMat = null;

    [SyncVar]
    private bool canLoot = true;

    [SerializeField] private List<Material[]> ogMats = new List<Material[]>();
    [SerializeField] private List<Material[]> newMats = new List<Material[]>();

    [Server]
    public override void OnServerInteract(NetworkIdentity client) {
        if (!canLoot) {
            return;
        }
        canLoot = false;
        base.OnServerInteract(client);
        GameObject g = Instantiate(dropTable.RollDrop(), transform.position + transform.up, Quaternion.identity);
        Item item = g.GetComponent<Item>();
        if (item == null) {
            Debug.LogError("Item "+g.name+" doesn't have item class");
            return;
        }
        NetworkServer.Spawn(g);
        PlayerInventory inv = client.transform.GetComponent<PlayerInventory>();
        bool success = inv.AddItem(client, item);
        if (success) {
            RpcSetEntityActive(g, false);
        }
        else {
            RpcThrowItem(g);
        }
        //RpcSetHighLight(false);
        SetHighLight(false);
        Invoke("ResetInteract", UnityEngine.Random.Range(resetTimeRange.x, resetTimeRange.y));
        RpcInteractionFinish(client);
    }

    [Server]
    private void ResetInteract() {
        canLoot = true;
        //RpcSetHighLight(true);
    }

    [ClientRpc]
    private void RpcThrowItem(GameObject g) {
        g.GetComponent<Rigidbody>().AddForce(Vector3.up + transform.forward, ForceMode.Impulse);
    }

    [ClientRpc]
    protected override void RpcInteractionFinish(NetworkIdentity client) {
        SoundSystem.PlaySound("interact_trashcan", transform.position);
    }

    /*[ClientRpc]
    private void RpcSetHighLight(bool isHighLighted) {
        MeshRenderer[] rend = transform.GetComponentsInChildren<MeshRenderer>();
        if (rend == null || highlightMat == null) {
            return;
        }
        for (int i = 0; i < rend.Length; i++) {
            rend[i].materials = isHighLighted ? newMats[i] : ogMats[i];
        }
    }*/

    public override void PlayerFocusEnter() {
        base.PlayerFocusEnter();
        SetHighLight(canLoot);
    }

    public override void PlayerFocusExit() {
        base.PlayerFocusExit();
        SetHighLight(false);
    }

    private void SetHighLight(bool highlighted) {
        if (highlightMat == null)
            return;
        if (ogMats.Count == 0) {
            foreach(MeshRenderer m in transform.GetComponentsInChildren<MeshRenderer>()) {
                ogMats.Add(m.materials);
                Material[] mats = new Material[m.materials.Length+1];
                m.materials.CopyTo(mats, 0);
                mats[m.materials.Length] = highlightMat;
                newMats.Add(mats);
            }
        }
        MeshRenderer[] rend = transform.GetComponentsInChildren<MeshRenderer>();
        if (rend == null || highlightMat == null) {
            return;
        }
        for (int i = 0; i < rend.Length; i++) {
            rend[i].materials = highlighted ? newMats[i] : ogMats[i];
        }
    }

}
