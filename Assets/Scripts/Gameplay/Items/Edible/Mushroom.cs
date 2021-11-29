using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mushroom : Item {
    
    [SerializeField] private float minTime = 0.5f, maxTime = 1f;
    [SerializeField] private Vector3 rayOffset = Vector3.up * 0.2f;
    [SerializeField] private float coneAngle = 60;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private int maxSpawnTries = 3;

    private RaycastHit hit;

    [Server]
    private void OnEnable() {
        Invoke("SpawnMushroom", Random.Range(minTime, maxTime));
    }

    [Server]
    private void SpawnMushroom() {
        if (!gameObject.activeSelf)
            return;
        // Calculate spawn pos
        bool found = false;
        for (int i = 0; i < maxSpawnTries; i++) {
            if (Physics.Raycast(transform.position + transform.rotation * rayOffset,
                transform.rotation * Quaternion.Euler(Random.Range(-coneAngle, coneAngle), 0,Random.Range(-coneAngle, coneAngle)) * -transform.up,
                out hit, maxDistance, ~LayerMask.GetMask("item"), QueryTriggerInteraction.Ignore)) {
                    found = true;
                    break;
            }
        }
        if (found) {
            GameObject g = Instantiate(Database.GetEntity(entityName).gameObject, hit.point,
                Quaternion.FromToRotation(Vector3.up, hit.normal));
            NetworkServer.Spawn(g);
            RpcPlayFX(g);
        }
        Invoke("SpawnMushroom", Random.Range(minTime, maxTime));
    }

    [ClientRpc]
    public void RpcPlayFX(GameObject g) {
        SoundSystem.PlaySound("mushroom_grow", transform.position);
    }
    
}
