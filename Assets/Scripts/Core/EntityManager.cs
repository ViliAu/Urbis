using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityManager {

    public static List<Player> players = new List<Player>();

    private static Player _localPlayer;
    public static Player LocalPlayer { 
        get {
            if (_localPlayer == null) {
                Debug.LogError("Local player was null. This should not be the case... Maybe the player " + 
                "was not registered successfully?");
            }
            return _localPlayer;
        }
    }

    public delegate void PlayerRegisteredHandler();
    public static event PlayerRegisteredHandler OnPlayerRegistered;

    public static int PlayerCount {
        get {
            if (players == null)
                return 0;
            return players.Count;
        }
    }

    public static void RegisterPlayer(Player player, bool isLocalPlayer) {
        // Set the player as localplayer if, the client owned the player gameobject
        //player.gameObject.name = "Player" + player.netId; 
        if (isLocalPlayer) {
            _localPlayer = player;
            OnPlayerRegistered?.Invoke();
        }

        if (!players.Contains(player)) {
            players.Add(player);
        }
    }

    /// <summary>
    /// Changes layer of the gameobject and all of it's children
    /// </summary>
    /// <param name="go">Gameobject to be affected</param>
    /// <param name="layer">Layer to change to</param>
    public static void ChangeLayer(GameObject go, int layer) {
        go.layer = layer;
        foreach (Transform child in go.transform) {
            ChangeLayer(child.gameObject, layer);
        }
    }
}