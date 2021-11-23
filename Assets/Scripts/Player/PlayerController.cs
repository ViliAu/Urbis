using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    /* User data */
    [Header("Movement speeds")]
    [SerializeField] private float speed = 15;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float crouchMovementSpeed = 13f;

    [Header("Ground movement")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 10f;

    [Header("Crouch settings")]
    [SerializeField] private float crouchSpeed = 5f;
    [SerializeField] private float crouchHeight = 0.9f;

    [Header("Air settings")]
    [SerializeField] private float airAcceleration = 1f;
    [SerializeField] private float gravity = 5f;
    [SerializeField] private float maximumGravity = 120f;
    [SerializeField] private float jumpHeight = 15f;

    [Header("Ladder")]
    [Tooltip("How far the palyer can grab a ladder")]
    [SerializeField] private float ladderWidth = 0.1f;
    [Tooltip("How fast the player climbs ladders")]
    [SerializeField] private float climbSpeed = 1f;
    [SerializeField] private float ladderMaxAngle = 10f;
    [SerializeField] private float ladderJumpForce = 10f;

    [Header("Ground mask")]
    [SerializeField] private LayerMask groundMask = default;

    public bool IsGrounded {get; private set;}
    public bool CanUncrouch {get; private set;}
    public bool IsLaddered {get; private set;}

    private float standHeight = 2f;
    private float ladderLookModifier = 1f;

    /* Memory data */
    [HideInInspector] public Vector3 velocity;
    private Vector3 additionalVelocity = Vector3.zero;
    public CharacterController controller;
    private Transform head;
    private Collider[] ldrCols = null;
    private Transform ladder;

    /* Initialize vars */
    private void Start() {
        controller = transform.GetComponent<CharacterController>();
        standHeight = controller.height;
        velocity = Vector3.zero;
        head = transform.Find("Head");
    }

    private void Update() {
        StateCheck();
        Acceleration();
        Deceleration();
        ApplyGravity();
        Jump();
        Crouch();
        ApplyVelocity();
        if (transform.position.y < -100)
            transform.position = Vector3.zero;
    }

    /* Creates a velocity vector from given input and current speed */ 
    private void Acceleration() {
        Vector3 dir = EntityManager.LocalPlayer.Player_Input.input;
        // Rotate input
        dir = transform.rotation * EntityManager.LocalPlayer.Player_Input.input;
        if (!IsGrounded && !IsLaddered) {
            AirAcceleration(dir);
            return;
        }

        // Laddered
        if (IsLaddered) {
            ladderLookModifier = Vector3.SignedAngle(EntityManager.LocalPlayer.Player_Camera.head.forward, transform.forward, transform.right) / ladderMaxAngle;
            ladderLookModifier = DUtil.Clamp1Neg1(ladderLookModifier);
            ladderLookModifier = Vector3.Angle(ladder.forward, transform.forward) > 45 ? ladderLookModifier *= -1 : ladderLookModifier;
            dir = LadderClimb(dir);
        }

        // Check the correct speed (crouch, sprint or normal)
        // Crouching
        if (EntityManager.LocalPlayer.Player_Input.crouched) {
            velocity.x = Mathf.Lerp(velocity.x, crouchMovementSpeed * dir.x, acceleration * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, crouchMovementSpeed * dir.z, acceleration * Time.deltaTime);
        }
        // Sprinting
        else if (EntityManager.LocalPlayer.Player_Input.sprinting) {
            velocity.x = Mathf.Lerp(velocity.x, sprintSpeed * dir.x, acceleration * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, sprintSpeed * dir.z, acceleration * Time.deltaTime);
        }
        // Walking
        else {
            velocity.x = Mathf.Lerp(velocity.x, speed * dir.x, acceleration * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, speed * dir.z, acceleration * Time.deltaTime);
        }
        velocity += additionalVelocity;
    }

    private void AirAcceleration(Vector3 dir) {
        if (EntityManager.LocalPlayer.Player_Input.input == Vector3.zero) {
            return;
        }
        if (velocity.magnitude > speed) {
            if ((velocity + dir).magnitude > velocity.magnitude) {
                return;
            }
        }
        velocity.x = Mathf.Lerp(velocity.x, speed * dir.x, airAcceleration * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, speed * dir.z, airAcceleration * Time.deltaTime);
    }

    private Vector3 LadderClimb(Vector3 dir) {
        float vel = Vector3.Dot(dir, ladder.forward);
        velocity.y = vel * climbSpeed * ladderLookModifier;
        dir *= (1-vel);
        return dir;
    }

    /* Decelerates player */
    private void Deceleration() {
        if (!IsGrounded && !IsLaddered) {
            return;
        }
        velocity.x = Mathf.Lerp(velocity.x, additionalVelocity.x, deceleration * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, additionalVelocity.z, deceleration * Time.deltaTime);
    }

    /* Handles gravity */
    private void ApplyGravity() {
        if (!IsLaddered) {
            if (IsGrounded) {
                if (velocity.y < -gravity)
                    velocity.y = -gravity;
            }
            else {
                velocity.y = Mathf.Clamp(velocity.y - gravity * Time.deltaTime, -maximumGravity, 1000);
                if (velocity.y > 0 && !CanUncrouch)
                    velocity.y = 0;
            }
        }
    }

    /* Handles jumping */
    private void Jump() {
        if (EntityManager.LocalPlayer.Player_Input.jumped) {
            if (IsGrounded) {
                velocity.y = jumpHeight;
            }
            else if (IsLaddered) {
                velocity.y = jumpHeight;
                velocity += -ladder.forward * ladderJumpForce;
                IsLaddered = false;
            }
        }            
    }

    private void Crouch() {
        if (head == null) {
            Debug.LogError(" No player head found...");
            return;
        }
        // Crouching
        if (EntityManager.LocalPlayer.Player_Input.crouched) {
            controller.height = Mathf.Lerp(controller.height, crouchHeight, crouchSpeed * Time.deltaTime);
            if (DUtil.Approx(controller.height, standHeight, 0.01f)) {
                controller.height = crouchHeight;
            }
        }
        // Uncrouching
        else if (CanUncrouch) {
            controller.height = Mathf.Lerp(controller.height, standHeight, crouchSpeed * Time.deltaTime);
            // Smoothing.. maybe rework
            if (DUtil.Approx(controller.height, standHeight, 0.01f)) {
                controller.height = standHeight;
            }
            else {
                controller.Move(Vector3.up * controller.height * Time.deltaTime);
            }
        }
    }

    /* Applies velocity vector to char controller (moves the player) */
    private void ApplyVelocity() {
        controller.Move(velocity * Time.deltaTime);
    }

    //Ground check
    private void StateCheck() {
        RaycastHit hit;

        // Hullu
        float cMod = Mathf.Abs(standHeight-controller.height) * 0.5f;
        
        Vector3 radius = new Vector3(0,controller.radius,0);
        Vector3 upperPos = transform.position + Vector3.up * (controller.height + cMod) - radius;
        Vector3 lowerPos = transform.position + radius + Vector3.up * cMod;
        Debug.DrawLine(Vector3.zero, lowerPos, Color.red);
        Debug.DrawLine(Vector3.zero, upperPos, Color.green);

        // Check if we hit a ladder
        ldrCols = Physics.OverlapCapsule(lowerPos + Vector3.up * ladderWidth, upperPos, controller.radius + ladderWidth, LayerMask.GetMask("Ladder"), QueryTriggerInteraction.Collide);
        IsLaddered = ldrCols.Length != 0;
        if (IsLaddered)
            ladder = ldrCols[0].transform;

        //Check we're grounded
        IsGrounded = Physics.CapsuleCast(upperPos, lowerPos, controller.radius, -Vector3.up, out hit, controller.skinWidth + 0.005f, groundMask, QueryTriggerInteraction.Ignore);

        // Check if we can uncrouch
        float crouchCeiling = IsGrounded ? 2 * cMod : 0;
        CanUncrouch = !Physics.CapsuleCast(lowerPos, upperPos, controller.radius, Vector3.up, out hit, crouchCeiling + controller.skinWidth + 0.005f, groundMask, QueryTriggerInteraction.Ignore);
    }
    /*
    private void AdditionalMovementCheck(RaycastHit hit) {
        Mover m = null;
        if (IsGrounded && (m = hit.transform.GetComponent<Mover>()) != null) {
            velocity -= additionalVelocity;
            additionalVelocity = (new Vector3(m.moveVec.x, 0, m.moveVec.z));
        }
        else {
            additionalVelocity = Vector3.zero;
        }
    }

    private void LidCheck(RaycastHit hit) {
        Door door;
        if (!CanUncrouch && (door = hit.transform.GetComponent<Door>()) != null && !door.Open) {
            door.StopAllCoroutines();
        }
    } */
}