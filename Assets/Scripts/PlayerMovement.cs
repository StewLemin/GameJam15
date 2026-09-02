using UnityEngine;
using UnityEngine.InputSystem; // <-- so the new system
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;
    private CharacterController ch;
    private Vector3 velocity;

    [Header("Camera")]
    public Transform cameraPivot;
    public CinemachineCamera vcam; // the CinemachineCamera parented under cameraPivot
    public Highlightable highlight; // the outline toggle on this capsule's mesh child
    public float lookSpeed = 2f;
    public float lookXLimit = 85f; // look up/down constraint

    private float rotationX = 0f;

    [Header("Possession")]
    [Tooltip("Only the currently-possessed capsule reads input and drives its vcam.")]
    public bool isActive = true;

    void Start()
    {
        ch = GetComponent<CharacterController>();

        if (isActive) Possess();
        else Unpossess();
    }

    void Update()
    {
        if (!isActive) return; // capsules we're not currently controlling ignore input entirely
                               // Will add NPC behavior here later

        if (Keyboard.current != null)
        {
            // Sprinting
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                moveSpeed = 8f;
            }
            else
            {
                moveSpeed = 5f;
            }

            // WASD Movement
            float x = 0f;
            float z = 0f;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) z = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) z = -1f;

            // Move in direction of camera
            Vector3 move = (transform.forward * z) + (transform.right * x);
            ch.Move(move * moveSpeed * Time.deltaTime);

            // Jumping
            if (ch.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // was originally 0f but this seems to help the character stick to the floor better
            }

            if (Keyboard.current.spaceKey.isPressed && ch.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // adding the gravity and moving the character
            velocity.y += gravity * Time.deltaTime;
            ch.Move(velocity * Time.deltaTime);
        }

        if (Mouse.current != null && cameraPivot != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            rotationX -= mouseDelta.y * lookSpeed * 0.1f;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            cameraPivot.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Rotate body with camera
            transform.rotation *= Quaternion.Euler(0, mouseDelta.x * lookSpeed * 0.1f, 0);
        }
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
}