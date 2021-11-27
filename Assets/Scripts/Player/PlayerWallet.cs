using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerWallet : NetworkBehaviour {

    [SyncVar]
    [SerializeField] private float balance = 0f;

    public void AddMoney(float amount) {
        balance += amount;
    }

}
