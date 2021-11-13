using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerWeapon : NetworkBehaviour {
    /*[Header("Weapons data")]
    [SerializeField] private Weapon[] weapons = null;
    [SerializeField] private Transform gunhold = null;
    [HideInInspector] public int equippedWeaponIndex = -1;

    [Header("Sway")]
    [SerializeField] private float swayAmount = 0.2f;
    [SerializeField] private float swaySpeed = 6f;
    [SerializeField] private Vector2 swayBounds = new Vector2(0.1f, 0.1f);

    private Vector3 initialPos = default;

    // Properties
    public Weapon EquippedWeapon {
        get {
            return equippedWeaponIndex < 0 || equippedWeaponIndex > weapons.Length-1 ? null : weapons[equippedWeaponIndex];
        }
    }

    private void Start() {
        if (gunhold != null) {
            initialPos = gunhold.localPosition;
        }
        else {
            Debug.LogWarning("No gunhold assigned to player weapon. Please assign it.");
        }
    }

    public override void OnStartClient() {
        if (!isLocalPlayer)
            return;
        CmdSetupWeapons(GetComponent<NetworkIdentity>());
        EquipWeapon(1);
    }

    private void Update() {
        Sway();
        CheckInput();
    }

    private void Sway() {
        Vector2 swayPos = EntityManager.LocalPlayer.Player_Input.mouseInput * swayAmount;
        swayPos.x = Mathf.Clamp(swayPos.x, -swayBounds.x, swayBounds.x);
        swayPos.y = Mathf.Clamp(swayPos.y, -swayBounds.y, swayBounds.y);
        gunhold.localPosition = Vector3.Lerp(gunhold.localPosition, initialPos - (Vector3)swayPos, swaySpeed * Time.deltaTime);
    }

    private void CheckInput() {
        // Input check priorities left mouse input before right mouse input. It checks first if it is held and only after that if it was clicked.
        // Left mouse
        if (EntityManager.LocalPlayer.Player_Input.leftMouseDown) {
            if (EquippedWeapon == null) {
                Debug.LogError("Equipped weapon is null!");
            }
            // Clicked
            if (!EquippedWeapon.fireModes[0].autoFire && EntityManager.LocalPlayer.Player_Input.leftMouse == 1) {
                EquippedWeapon.Shoot(EquippedWeapon.fireModes[0]);
            }
            // Held
            else if (EquippedWeapon.fireModes[0].autoFire) {
                EquippedWeapon.Shoot(EquippedWeapon.fireModes[0]);
            }
        }
        // Right mouse
        else if (EntityManager.LocalPlayer.Player_Input.rightMouseDown) {
            if (EquippedWeapon == null) {
                Debug.LogError("Equipped weapon is null!");
            }
            // Clicked
            if (!EquippedWeapon.fireModes[1].autoFire && EntityManager.LocalPlayer.Player_Input.rightMouse == 1) {
                EquippedWeapon.Shoot(EquippedWeapon.fireModes[1]);
            }
            // Held
            else if (EquippedWeapon.fireModes[1].autoFire) {
                EquippedWeapon.Shoot(EquippedWeapon.fireModes[1]);
            }
        }
        if (EntityManager.LocalPlayer.Player_Input.pressedNum != -1) {
            EquipWeapon(EntityManager.LocalPlayer.Player_Input.pressedNum);
        }
    }

    // TODO: Hide the spawned wep from others
    [Command]
    private void CmdSetupWeapons(NetworkIdentity netID) {
        PlayerWeapon owner = netID.GetComponent<PlayerWeapon>();
        for (int i = 0; i < weapons.Length; i++) {
            GameObject weapon = Instantiate(weapons[i].gameObject);
            NetworkServer.Spawn(weapon, netID.gameObject);
            RpcUpdateWeapon(netID, weapon.GetComponent<NetworkIdentity>(), i);
        }
    }
    
    [ClientRpc]
    private void RpcUpdateWeapon(NetworkIdentity target, NetworkIdentity weaponNetID, int i) {
        weapons[i] = weaponNetID.GetComponent<Weapon>();
        Transform wep = weaponNetID.transform;
        PlayerWeapon owner = target.GetComponent<PlayerWeapon>();
        wep.position = owner.gunhold.position + weapons[i].positionOffset;
        wep.rotation = Quaternion.Euler(weapons[i].rotationOffset);
        wep.parent = owner.gunhold;
        wep.gameObject.SetActive(false);
    }

    [Command]
    private void CmdEquipWeapon(NetworkIdentity netId, int index) {
        RpcEquipWeapon(netId, index);
    }

    [ClientRpc]
    private void RpcEquipWeapon(NetworkIdentity netId, int index) {
        // Disable old weapon from view and call the unequip function
        if (EquippedWeapon != null) {
            EquippedWeapon.Unequip();
            EquippedWeapon.gameObject.SetActive(false);
        }
        equippedWeaponIndex = index;
        // Equip the new weapon
        EquippedWeapon.gameObject.SetActive(true);

        if (isLocalPlayer) {
            EquippedWeapon.Equip();
        }
    }


    private void EquipWeapon(int index) {
        index = index == 0 ? 9 : --index;
        // Check if the equip goes out of bounds
        if (index < 0 || index > weapons.Length-1) {
            return;
        }

        CmdEquipWeapon(GetComponent<NetworkIdentity>(), index);
    }*/

}
