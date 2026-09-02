using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // <-- so the new system

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 5f;
    public float gravity = -9.81f;
    private CharacterController ch;
    private Vector3 velocity;

    [Header("Camera Direction")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f; // loop up constraint

    private float rotationX = 0f;

    void Start()
    {
        ch = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // center cursor
        Cursor.visible = false; // hide cursor
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
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

        if (Mouse.current != null && playerCamera != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            rotationX -= mouseDelta.y * lookSpeed * 0.1f;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Rotate body with camera
            transform.rotation *= Quaternion.Euler(0, mouseDelta.x * lookSpeed * 0.1f, 0);
        }
    }
}
