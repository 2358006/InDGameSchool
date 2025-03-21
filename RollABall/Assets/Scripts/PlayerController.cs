using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
public class PlayerController : NetworkBehaviour
{
    PlayerInputAction playerInput;

    [SyncVar(hook = nameof(OnColorChanged))]
    public Color playerColor = Color.white;

    Material playerMaterialClone;
    [SerializeField]
    Rigidbody rb;

    float movementX;
    float movementY;

    const float MOVE_FORCE = 1000f;

    void OnColorChanged(Color oldColor, Color newColor)
    {
        Renderer currRender = this.GetComponent<Renderer>();
        playerMaterialClone = new Material(currRender.material);
        playerMaterialClone.color = newColor;
        currRender.material = playerMaterialClone;
    }

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

        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        CmdSetupPlayer(color);
    }

    [Command]
    public void CmdSetupPlayer(Color color)
    {
        playerColor = color;
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