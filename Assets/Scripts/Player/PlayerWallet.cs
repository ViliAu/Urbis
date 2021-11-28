using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerWallet : NetworkBehaviour {

    [SyncVar (hook=nameof(UpdateUI))]
    [SerializeField] private float balance = 0f;

    public void AddMoney(float amount) {
        balance += amount;
    }

    private void UpdateUI(float oldVal, float newVal) {
        EntityManager.LocalPlayer.Player_UI.UpdateMoneyAmount(newVal);
    }

}
