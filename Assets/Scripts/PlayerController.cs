using UnityEngine;
using UnityEngine.InputSystem;

public class Example : MonoBehaviour
{
    // These variables are to hold the Action references
    InputAction moveAction;
    InputAction jumpAction;
    public float moveSpeed = 5f;  
    public float jumpHeight = 2f; 

    private void Start()
    {
        // Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        // Read the "Move" action value, which is a 2D vector
        // and the "Jump" action state, which is a boolean value

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveValue.x, 0, moveValue.y) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        if (jumpAction.IsPressed())
        {
            transform.position += Vector3.up * jumpHeight;
        }
    }
}