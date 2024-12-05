using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    void Update()
    {
        // Handle movement relative to the player's view
        Vector3 movement = Vector3.zero;

        if (Keyboard.current.upArrowKey.isPressed)
            movement += transform.forward; // Move forward
        if (Keyboard.current.downArrowKey.isPressed)
            movement -= transform.forward; // Move backward
        if (Keyboard.current.leftArrowKey.isPressed)
            movement -= transform.right;  // Move left
        if (Keyboard.current.rightArrowKey.isPressed)
            movement += transform.right;  // Move right

        // Prevent movement along the Y-axis
        movement.y = 0;

        movement *= moveSpeed * Time.deltaTime;
        transform.position += movement; // Apply movement

        // Handle camera rotation
        float rotationY = 0f;
        if (Keyboard.current.aKey.isPressed) rotationY -= 1; // Rotate left
        if (Keyboard.current.dKey.isPressed) rotationY += 1; // Rotate right

        float rotationX = 0f;
        if (Keyboard.current.wKey.isPressed) rotationX -= 1; // Look up
        if (Keyboard.current.sKey.isPressed) rotationX += 1; // Look down

        // Apply rotation to the camera (or player object)
        transform.Rotate(Vector3.up, rotationY * rotationSpeed * Time.deltaTime, Space.World); // Y-axis rotation
        transform.Rotate(Vector3.right, rotationX * rotationSpeed * Time.deltaTime, Space.Self); // X-axis rotation
    }
}