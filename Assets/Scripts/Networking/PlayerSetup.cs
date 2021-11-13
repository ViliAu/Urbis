using UnityEngine;
using Mirror;
 
public class PlayerSetup : NetworkBehaviour {
 
    [SerializeField] private Behaviour[] componentsToDisable = null;
    [SerializeField] private GameObject[] gameObjectsToDisable = null;
 
    [Header("Self objects")]
    [SerializeField] private GameObject[] selfObjectToDisable = null;
 
    void Start() {
        // Disable all listed components for other players
        MeshRenderer[] rends = GetComponentsInChildren<MeshRenderer>();
        if (!isLocalPlayer) {
            DisableComponents();
        }
 
        else {
            DisableSelfObjects();
        }
 
        // Register player to GameManager
        EntityManager.RegisterPlayer(GetComponent<Player>(), isLocalPlayer);
    }
 
    void DisableComponents() {
        foreach (Behaviour b in componentsToDisable) {
            b.enabled = false;
        }
 
        foreach (GameObject go in gameObjectsToDisable) {
            go.SetActive(false);
        }
    }
 
    void DisableSelfObjects() {
        foreach (GameObject go in selfObjectToDisable) {
            go.SetActive(false);
        }
    }
}