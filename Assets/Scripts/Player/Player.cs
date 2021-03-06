using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Player : MonoBehaviour {
    
    private PlayerController pc;
    public PlayerController Player_Controller {
        get {
            if (pc == null) {
                pc = transform.GetComponent<PlayerController>();
            }
            return pc;
        }
    }

    private PlayerInput pi;
    public PlayerInput Player_Input {
        get {
            if (pi == null) {
                pi = transform.GetComponent<PlayerInput>();
            }
            return pi;
        }
    }

    private PlayerCamera pcam;
    public PlayerCamera Player_Camera {
        get {
            if (pcam == null) {
                pcam = transform.GetComponent<PlayerCamera>();
            }
            return pcam;
        }
    }

    private PlayerWeapon pwep;
    public PlayerWeapon Player_Weapon {
        get {
            if (pwep == null) {
                pwep = transform.GetComponent<PlayerWeapon>();
            }
            return pwep;
        }
    }

    private NetworkIdentity netid;
    public NetworkIdentity Network_Identity {
        get {
            if (netid == null) {
                netid = transform.GetComponent<NetworkIdentity>();
            }
            return netid;
        }
    }

    private PlayerUI pui;
    public PlayerUI Player_UI {
        get {
            if (pui == null) {
                pui = transform.GetComponent<PlayerUI>();
            }
            return pui;
        }
    }

    private PlayerWallet pwallet;
    public PlayerWallet Player_Wallet {
        get {
            if (pwallet == null) {
                pwallet = transform.GetComponent<PlayerWallet>();
            }
            return pwallet;
        }
    }

    void Start() {
        MoveToSpawnPosition();
    }

    void MoveToSpawnPosition() {
        // TODO : Ehkä tää kuulus jonnekkii muualle mut ei nyt täl hetjkel kummiskaa
        PlayerSpawnPoint[] spawns = Resources.LoadAll<PlayerSpawnPoint>("ScriptableObjects/Spawn Points");
        string sceneName = SceneManager.GetActiveScene().name;
        foreach (PlayerSpawnPoint spawn in spawns) {
            if (spawn.sceneName == sceneName) {
                transform.position = spawn.playerSpawnPos; 
            }
        }
    }
}