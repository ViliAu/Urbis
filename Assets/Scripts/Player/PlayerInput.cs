using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    public float sensitivity = 1f;
    [HideInInspector] public Vector3 input = Vector3.zero;
    [HideInInspector] public Vector2 mouseInput = Vector2.zero;
    
    [HideInInspector] public int pressedNum = -1;
    [HideInInspector] public int leftMouse = 0;
    [HideInInspector] public int rightMouse = 0;
    [HideInInspector] public int scroll = 0;

    [HideInInspector] public bool leftMouseDown = false;
    [HideInInspector] public bool rightMouseDown = false;
    [HideInInspector] public bool jumped = false;
    [HideInInspector] public bool interacted = false;
    [HideInInspector] public bool dropped = false;
    [HideInInspector] public bool threw = false;
    [HideInInspector] public bool reload = false;
    [HideInInspector] public bool tabbed = false;

    [HideInInspector] public bool crouched = false;
    [HideInInspector] public bool zoom = false;
    [HideInInspector] public bool sprinting = false;
    [HideInInspector] public bool magnet = false;

    private void Update() {
        GetInput();
    }

    private void GetInput() {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        jumped = Input.GetKeyDown(KeyCode.Space);
        interacted = Input.GetKeyDown(KeyCode.E);
        dropped = Input.GetKeyDown(KeyCode.G);
        threw = Input.GetKeyDown(KeyCode.Mouse1);
        reload = Input.GetKeyDown(KeyCode.R);
        tabbed = Input.GetKeyDown(KeyCode.Tab);

        leftMouseDown = Input.GetKey(KeyCode.Mouse0);
        rightMouseDown = Input.GetKey(KeyCode.Mouse1);
        zoom = Input.GetKey(KeyCode.C);
        magnet = Input.GetKey(KeyCode.E);
        crouched = Input.GetKey(KeyCode.LeftControl);
        sprinting = Input.GetKey(KeyCode.LeftShift);

        // Simplifies mouse wheel return value: Either -1, 0 or 1.
        scroll = Input.GetAxisRaw("Mouse ScrollWheel") > 0 ? 1 : Input.GetAxisRaw("Mouse ScrollWheel") < 0 ? -1 : 0;

        // Returns 1 when you press the according mouse button, -1 when you release it. Otherwise 0
        leftMouse = Input.GetKeyDown(KeyCode.Mouse0) ? 1 : Input.GetKeyUp(KeyCode.Mouse0) ? -1 : 0;
        rightMouse = Input.GetKeyDown(KeyCode.Mouse1) ? 1 : Input.GetKeyUp(KeyCode.Mouse1) ? -1 : 0;

        GetPressedNum();
    }

    public void GetPressedNum() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            pressedNum = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            pressedNum = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            pressedNum = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            pressedNum = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            pressedNum = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            pressedNum = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            pressedNum = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            pressedNum = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            pressedNum = 9;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) {
            pressedNum = 0;
        }
        else pressedNum = -1;
    }
}