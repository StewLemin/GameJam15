using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // <-- so the new system
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    public float sprintSpeed = 10f;
    public float jumpForce = 7f;
    public float gravity = 10f;

    public float walkAcceleration = 0.1f;
    public float runAcceleration = 0.5f;

    public float horizontalDecceleration = 0.7f;
    
    
    
    private CharacterController ch;
    private Vector3 velocity;

    [Header("Camera")]
    public Transform cameraPivot;
    public CinemachineCamera vcam; // the CinemachineCamera parented under cameraPivot
    public Highlightable highlight; // the outline toggle on this capsule's mesh child
    public float lookSpeed = 2f;
    public float lookXLimit = 85f; // look up/down constraint

    private float rotationX = 0f;
    
    
    //StateMachines
    
    private enum HorizontalState{
        IDLE,
        WALK,
        RUN
    }

    private enum VerticalState
    {
        GROUNDED,
        JUMPING,
        FALLING
    }

    private enum MindTransferState
    {
        INACTIVE,
        TRANSITIONING,
        TRANSITIONED,
        COOLDOWN
    }

    private HorizontalState hState;
    private VerticalState vState;
    private MindTransferState mState;

    private Vector3 horizontalVelocity;

    private float currentSpeed = 0f;
    


    //QOL 
    public float coyoteTime = 0.1f;
    public float jumpBuffer = 0.1f;

    [Header("Possession")]
    [Tooltip("Only the currently-possessed capsule reads input and drives its vcam.")]
    public bool isActive = true;

    void Start()
    {
        ch = GetComponent<CharacterController>();

        if (isActive) Possess();
        else Unpossess();
        Cursor.lockState = CursorLockMode.Locked; // center cursor
        Cursor.visible = false; // hide cursor
        hState =  HorizontalState.IDLE;
        vState = VerticalState.FALLING;
        mState = MindTransferState.INACTIVE;
        horizontalVelocity = Vector3.zero;
    }

    void Update()
    {
        if (!isActive) return; // capsules we're not currently controlling ignore input entirely
                               // Will add NPC behavior here later

        
        coyoteTime = coyoteTime - Time.deltaTime;
        jumpBuffer = jumpBuffer - Time.deltaTime;
        
        if (ch.isGrounded)
        {
            vState = VerticalState.GROUNDED;
        }
        else if(!ch.isGrounded && vState != VerticalState.JUMPING)
        {
            vState = VerticalState.FALLING;
        }
        
        Vector3 direction = getHorizontalAxis();

        if (direction != Vector3.zero)
        {
            hState = HorizontalState.WALK;
        }

        else
        { 
            hState = HorizontalState.IDLE;
        }

            
        if (ch.isGrounded)
            {
                vState = VerticalState.GROUNDED;
            }
        else if(!ch.isGrounded && vState != VerticalState.JUMPING)
            {
                vState = VerticalState.FALLING;
            }

        if (Keyboard.current.leftShiftKey.isPressed && hState == HorizontalState.WALK)
            {
                hState = HorizontalState.RUN;
            }

        if (Keyboard.current.spaceKey.isPressed && vState == VerticalState.GROUNDED)
            {
                vState = VerticalState.JUMPING;
            }
        
        
        Debug.Log(hState);
        
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            rotationX -= mouseDelta.y * lookSpeed * 0.1f;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            cameraPivot.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Rotate body with camera
            transform.rotation *= Quaternion.Euler(0, mouseDelta.x * lookSpeed * 0.1f, 0);

            float h = getHorizontalAxis().x;
            float v = getHorizontalAxis().z;

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            direction = (forward * v + right * h);
        
        direction = direction.normalized;

        float targetSpeed = 0f;
        float rate = horizontalDecceleration;
        
        switch (hState)
        {
            case HorizontalState.WALK:
                targetSpeed = moveSpeed;
                rate = walkAcceleration;
                break;
            case HorizontalState.IDLE:
                targetSpeed = 0f;
                rate = horizontalDecceleration;
                break;
            case HorizontalState.RUN:
                targetSpeed = sprintSpeed;
                rate = runAcceleration;
                break;
        }

       
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate);
        Vector3 horizontalVelocity = currentSpeed * direction;
        
        
        switch (vState)
        {
            case VerticalState.GROUNDED:
                if (velocity.y < 0) velocity.y = -2f;
                if (Keyboard.current.spaceKey.isPressed)
                {
                    velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                    vState = VerticalState.JUMPING;
                }
                break;

            case VerticalState.JUMPING:
            case VerticalState.FALLING:
                velocity.y += gravity * Time.deltaTime;
                break;
        }

        Vector3 finalMove = horizontalVelocity + Vector3.up * velocity.y;
        ch.Move(finalMove * Time.deltaTime);
    }

    /// Called by CapsuleSwitcher when this capsule becomes the one you control.
    public void Possess()
    {
        isActive = true;

        if (vcam != null)
        {
            // Raise this vcam's priority above the others so CinemachineBrain blends to it.
            Debug.Log($"Possessing {name}, raising {vcam.Priority.Value} priority to 20");
            vcam.Priority = new PrioritySettings { Enabled = true, Value = 20 };
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// Called by CapsuleSwitcher when control moves to a different capsule.
    public void Unpossess()
    {
        isActive = false;
        velocity = Vector3.zero;

        if (vcam != null)
        {
            // Lower vcam priority
            Debug.Log($"Unpossessing {name}, lowering {vcam.Priority.Value} priority to 0");
            vcam.Priority = new PrioritySettings { Enabled = false, Value = 0 };
        }
    }

    private Vector3 getHorizontalAxis()
    {
        
        bool input_right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
        bool input_left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool input_forward = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed;
        bool input_back = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;

       
        float x = 0;
        float z = 0;

        if (input_forward)
        {
            z = +1;
        }

       
    
        if (input_right)
        {
            x = +1;
        }

        if (input_back)
        {
            z = - 1;
        }

      
        if (input_left)
        {
            x = -1;
        }

        Vector3 direction = new Vector3(x, 0, z);
        return direction;
    }
}