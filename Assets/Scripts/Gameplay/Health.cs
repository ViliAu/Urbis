using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour {

    [SerializeField] private float maxHealth = 100;

    [SyncVar]
    [SerializeField] private float health = 0;

    // Start is called before the first frame update
    private void Start() {
        health = maxHealth;
    }

    // Call this on server only!
    public void TakeDamage(float amount) {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health == 0) {
            Die();
        }
        print("Ukko teki damaa: "+amount+"Täl hetkel: "+health);
    }

    [ClientRpc]
    private void Die() {
        Destroy(gameObject);
    }
}
