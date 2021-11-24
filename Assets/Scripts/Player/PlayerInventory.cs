using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInventory : Inventory {

    private void Update() {
        CheckInput();    
    }

    private void CheckInput() {
        if (EntityManager.LocalPlayer.Player_Input.tabbed) {
            ToggleUI();
        }
    }

}
