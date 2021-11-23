using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerUrbis : NetworkManager {
    
    [SerializeField] private bool fastPlay = true;
    
    private static NetworkManagerUrbis _instance;
    public static NetworkManagerUrbis Instance {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<NetworkManagerUrbis>();
            return _instance;
        }
    }

    public override void Start() {
        base.Start();
        if (fastPlay && Application.isEditor)
            StartHost();
    }

}
