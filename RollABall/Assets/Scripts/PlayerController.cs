using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
public class PlayerController : NetworkBehaviour
{
    PlayerInputAction playerInput;

    [SerializeField]
    Rigidbody rb;

    float movementX;
    float movementY;

    const float MOVE_FORCE = 100f;

    public override void OnStartLocalPlayer()
    {
        Debug.Log("S S S Start Local Player");
        Camera mainCam = Camera.main;
        var camController = mainCam.GetComponent<CameraController>();
        camController.SetupPlayer(this.gameObject);

        playerInput = new PlayerInputAction();
        playerInput.Player.Enable();
        playerInput.Player.Move.performed += ctx => this.OnMovement(ctx);
        playerInput.Player.Move.canceled += ctx => this.OnMovement(ctx);
    }


    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0f, movementY) * MOVE_FORCE * Time.deltaTime;
        rb.AddForce(movement);
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (ctx.performed)
        {
            Vector2 movementVector = ctx.ReadValue<Vector2>();
            movementX = movementVector.x;
            movementY = movementVector.y;
            Debug.Log($"{movementX}, {movementY}");
        }

        if (ctx.canceled)
        {
            movementX = 0.0f;
            movementY = 0.0f;
        }
    }
}