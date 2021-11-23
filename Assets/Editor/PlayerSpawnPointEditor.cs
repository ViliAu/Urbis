using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(NetworkManagerUrbis))]
public class PlayerSpawnPointEditor : Editor {

    private static Vector3 spawnPos;

    void OnSceneGUI() {
        Event guiEvent = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
                NetworkManagerUrbis netman = FindObjectOfType<NetworkManagerUrbis>();
                if (netman != null) {
                    UpdateSpawnPosition(hit.point);
                }
            }

            HandleUtility.AddDefaultControl(0);
            HandleUtility.Repaint();
        }

    }

    void OnEnable() {
        PlayerSpawnPoint spawn = GetSceneSpawn();
        if (spawn != null) {
            spawnPos = spawn.playerSpawnPos;
        }
    }

    void UpdateSpawnPosition(Vector3 newPos) {
        // Try to find PlayerSpawnPoint SO for the opened scene
        string sceneName = SceneManager.GetActiveScene().name;
        string path = "ScriptableObjects/Spawn Points/";
        PlayerSpawnPoint spawnPoint = GetSceneSpawn();


        // No spawn point found, create new one
        if (spawnPoint == null) {
            spawnPoint = ScriptableObject.CreateInstance<PlayerSpawnPoint>();
            AssetDatabase.CreateAsset(spawnPoint, "Assets/Resources/" + path + sceneName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Update spawn point properties
        spawnPoint.sceneName = sceneName;
        spawnPoint.playerSpawnPos = newPos;
        spawnPos = spawnPoint.playerSpawnPos;

        EditorUtility.SetDirty(spawnPoint);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    PlayerSpawnPoint GetSceneSpawn() {
        PlayerSpawnPoint[] spawns = Resources.LoadAll<PlayerSpawnPoint>("ScriptableObjects/Spawn Points");
        string sceneName = SceneManager.GetActiveScene().name;
        // Try to find matching spawn point
        foreach (PlayerSpawnPoint spawn in spawns) {
            if (spawn.sceneName == sceneName) {
                return spawn;
            }
        }

        return null;
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    public static void DrawGizmos(NetworkManagerUrbis src, GizmoType gizmoType) {
        Gizmos.DrawIcon(spawnPos, "jere_wasted", true);
    }

}