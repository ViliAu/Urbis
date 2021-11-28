using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    public Transform head = null;
    public bool locked = false;
    public Camera cam;
    private Vector2 camEuler = default;

    private void Awake() {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camEuler.x = head.transform.eulerAngles.x;
        camEuler.y = transform.eulerAngles.y;
    }

    private void Update() {
        UpdateCamera();
    }

    private void UpdateCamera() {
        if (locked) {
            return;
        }
        // Get vars
        camEuler.x -= EntityManager.LocalPlayer.Player_Input.mouseInput.y * EntityManager.LocalPlayer.Player_Input.sensitivity * Time.deltaTime;
        camEuler.y += EntityManager.LocalPlayer.Player_Input.mouseInput.x * EntityManager.LocalPlayer.Player_Input.sensitivity * Time.deltaTime;

        // Clamp vertical look
        camEuler.x = Mathf.Clamp(camEuler.x, -89, 89);

        // Apply rotation
        head.transform.rotation = Quaternion.Euler(new Vector3(camEuler.x, head.eulerAngles.y, head.eulerAngles.z));
        transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, camEuler.y, transform.eulerAngles.z));

        // Check zoom
        cam.fieldOfView = EntityManager.LocalPlayer.Player_Input.zoom ? 20 : 90;
    }
}