
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{ 
    public Rigidbody rig;
    public Collider col;
    public float speed = 10;
    public float jumpforce = 10;
    public LayerMask floorlayer;

    private Vector2 moveInput;

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        rig.linearVelocity = new(moveInput.x * speed, rig.linearVelocity.y, moveInput.y * speed);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector3 p1 = new(col.bounds.max.x, col.bounds.max.y, col.bounds.max.z);
            Vector3 p2 = new(col.bounds.min.x, col.bounds.max.y, col.bounds.min.z);
            Vector3 p3 = new(col.bounds.min.x, col.bounds.max.y, col.bounds.max.z);
            Vector3 p4 = new(col.bounds.max.x, col.bounds.max.y, col.bounds.min.z);
            Vector3 p5 = new(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);

            if (Physics.Raycast(p1, Vector3.down, col.bounds.size.y * 1.1f, floorlayer) ||
                Physics.Raycast(p2, Vector3.down, col.bounds.size.y * 1.1f, floorlayer) ||
                Physics.Raycast(p3, Vector3.down, col.bounds.size.y * 1.1f, floorlayer) ||
                Physics.Raycast(p4, Vector3.down, col.bounds.size.y * 1.1f, floorlayer) ||
                Physics.Raycast(p5, Vector3.down, col.bounds.size.y * 1.1f, floorlayer))
            {
                rig.linearVelocity = new(rig.linearVelocity.x, jumpforce, rig.linearVelocity.z);
            }
        }
    }
}