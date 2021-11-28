using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerUrbis : NetworkManager {
    
    [SerializeField] private bool fastPlay = true;

    public delegate void OnClientConnected();
    public OnClientConnected clientConnected;
    
    private static NetworkManagerUrbis _instance;
    public static NetworkManagerUrbis Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<NetworkManagerUrbis>();
            }
            return _instance;
        }
    }

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        InvokeOnClientConnected();
    }

    private void InvokeOnClientConnected() {
        if(clientConnected != null)
            Instance.clientConnected.Invoke();
    }

    public override void Start() {
        GetSpawnablePrefabs();
        base.Start();
        if (fastPlay && Application.isEditor)
            StartHost();
    }

    private void GetSpawnablePrefabs() {
        NetworkIdentity[] netID = Resources.LoadAll<NetworkIdentity>("Prefabs");
        foreach(NetworkIdentity n in netID) {
            if (n.gameObject.name != "Player")
                spawnPrefabs.Add(n.gameObject);
        }
    }

}
