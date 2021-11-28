using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Trashbag : Interactable {

    [SerializeField] private DropTable dropTable = null;
    [SerializeField][Range(1, 10)] private int minDropAmount = 1, maxDropAmount= 5;
    [SerializeField] private Material highlightMat = null;

    [SerializeField] private List<Material[]> ogMats = new List<Material[]>();
    [SerializeField] private List<Material[]> newMats = new List<Material[]>();

    [Server]
    public override void OnServerInteract(NetworkIdentity client) {
        base.OnServerInteract(client);
        PlayerInventory inv = client.transform.GetComponent<PlayerInventory>();
        int dropAmount = UnityEngine.Random.Range(minDropAmount, maxDropAmount+1);
        for (int i = 0; i < dropAmount; i++) {
            GameObject g = Instantiate(dropTable.RollDrop(), transform.position + transform.up, Quaternion.identity);
            Item item = g.GetComponent<Item>();
            if (item == null) {
                Debug.LogError("Item "+g.name+" doesn't have item class");
                return;
            }
            NetworkServer.Spawn(g);
            bool success = inv.AddItem(client, item);
            if (success) {
                RpcSetEntityActive(g, false);
            }
            else {
                RpcThrowItem(g);
            }
        }
        RpcInteractionFinish(client);
    }

    [ClientRpc]
    private void RpcThrowItem(GameObject g) {
        g.GetComponent<Rigidbody>().AddForce(Vector3.up + transform.forward, ForceMode.Impulse);
    }

    [ClientRpc]
    protected override void RpcInteractionFinish(NetworkIdentity client) {
        SoundSystem.PlaySound("interact_trashbag", transform.position);
        CmdDestroyObject();
    }

    // TODO: Add authority
    [Command (requiresAuthority = false)]
    private void CmdDestroyObject() {
        NetworkServer.Destroy(gameObject);
    }

    public override void PlayerFocusEnter() {
        base.PlayerFocusEnter();
        SetHighLight(true);
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
