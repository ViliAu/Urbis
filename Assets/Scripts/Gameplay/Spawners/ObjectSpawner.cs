using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectSpawner : NetworkBehaviour {
    [Header("Spawn settings")]
    [SerializeField] private DropTable dropTable = null;
    [SerializeField] private Vector2 bounds = new Vector2(100, 100);
    [SerializeField] private Vector2 center = Vector2.zero;
    [SerializeField] float minHeight = -10f, maxHeight = 10f;
    [SerializeField] float minTime = 10f, maxTime = 30f;

    [Header("Debug")]
    [SerializeField] private bool drawBounds = true;

    [Server]
    private void Start() {
        if (dropTable == null) {
            Debug.LogError("Object spawner "+gameObject.name+" has no droptable!");
            return;
        }
        SpawnItem();
    }

    [Server]
    private void SpawnItem() {
        // Get spawn position
        Vector3 spawnPos = GetRandomPoint();
        if (spawnPos != Vector3.zero) {
            GameObject g = Instantiate(dropTable.RollDrop(), spawnPos, Quaternion.identity);
            NetworkServer.Spawn(g);
        }
        Invoke("SpawnItem", Random.Range(minTime, maxTime));
    }

    private Vector3 GetRandomPoint() {
        RaycastHit hit;
        Vector3 centerPoint = DUtil.Vec2ToVec3XZ(center);
        for (int i = 0; i < 100; i++){
            centerPoint = centerPoint + new Vector3(Random.Range(-bounds.x, bounds.x), 200, Random.Range(-bounds.y, bounds.y));
            if (Physics.Raycast(centerPoint, -Vector3.up, out hit, 300, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore)){
                if (DUtil.Between(hit.point.y, minHeight, maxHeight))
                    return hit.point;
            }
        }
        return Vector3.zero;
    }

    private void OnDrawGizmos() {
        Vector3 c = DUtil.Vec2ToVec3XZ(center);
        if (drawBounds) {
            Vector3 p1 = c + new Vector3(-bounds.x,0,bounds.y);
            Vector3 p2 = c + new Vector3(bounds.x,0,bounds.y);
            Vector3 p3 = c + new Vector3(bounds.x,0,-bounds.y);
            Vector3 p4 = c + new Vector3(-bounds.x,0,-bounds.y);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }
    }
}
