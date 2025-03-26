using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Mirror.BouncyCastle.Crypto.Modes;
public class PlayerController : NetworkBehaviour
{
    PlayerInputAction playerInput;
    private SceneScript sceneScript;

    public TextMesh playerNameText;
    public GameObject floatingInfo;

    [SyncVar(hook = nameof(OnNameChange))]
    public string playerName;

    [SyncVar(hook = nameof(OnColorChanged))]
    public Color playerColor = Color.white;

    [Command]
    public void CmdSendPlayerMessage()
    {
        if (sceneScript)
            sceneScript.statusText = $"{playerName} say hello {Random.Range(10, 99)}";
    }

    private Material playerMaterialClone;

    [SerializeField]
    private Rigidbody rb;

    private float movementX;
    private float movementY;

    const float MOVE_FORCE = 1000f;

    void OnNameChange(string _old, string _new)
    {
        playerNameText.text = playerName;
    }

    #region Unity Callback
    private void Awake()
    {
        sceneScript = GameObject.FindObjectOfType<SceneScript>();

        foreach (var item in weaponArray)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }
    }

    void Update()
    {
        Vector3 pos = this.transform.position;
        if (!isLocalPlayer)
        {
            UpdateFloatingInfoPosition(pos);

            UpdateWeaponPosition();
            return;
        }

        Vector3 movement = new Vector3(movementX, 0f, movementY) * MOVE_FORCE * Time.deltaTime;
        rb.AddForce(movement);

        UpdateFloatingInfoPosition(pos);
        UpdateWeaponPosition();

        if (Input.GetButtonDown("Fire2"))
        {
            var weapon = selectedWeaponLocal + 1;

            if (weapon > weaponArray.Length)
            {
                selectedWeaponLocal = 1;
            }
            selectedWeaponLocal = weapon;
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }
        // Quaternion(4원수) : x, y, z, w
        // Vector3의 x, y, z 와 다름
    }

    void UpdateWeaponPosition()
    {
        weaponRoot.transform.position = this.transform.position;
        var rot = this.transform.rotation.eulerAngles;

        var currAngle = weaponRoot.transform.rotation.eulerAngles.y; // this.transform.rotation.eulerAngles.y;
        var nextAngle = Mathf.Atan2(movementX, movementY);
        var angle = Mathf.LerpAngle(currAngle, nextAngle, 3f * Time.deltaTime);
        weaponRoot.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        // Quaternion.Slerp(Quaternion.Euler(0f, currAngle * Mathf.Rad2Deg, 0f), Quaternion.Euler(0f, nextAngle * Mathf.Rad2Deg, 0f), Time.deltaTime);
        Debug.Log($"curr angle {currAngle}, next angel {nextAngle}");
    }

    #endregion

    #region other
    void OnColorChanged(Color oldColer, Color newColor)
    {
        Renderer currRender = this.GetComponent<Renderer>();
        playerMaterialClone = new Material(currRender.material);
        playerMaterialClone.color = newColor;
        currRender.material = playerMaterialClone;
    }
    public override void OnStartLocalPlayer()
    {
        Debug.Log("Start Local Player...");

        sceneScript.playerScript = this;

        Camera mainCam = Camera.main;
        var camController = mainCam.GetComponent<CameraController>();
        camController.SetupPlayer(this.gameObject);

        playerInput = new PlayerInputAction();
        playerInput.Player.Enable();
        playerInput.Player.Move.performed += ctx => this.OnMove(ctx);
        playerInput.Player.Move.canceled += ctx => this.OnMove(ctx);

        string name = $"Player {Random.Range(100, 1000)}";

        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        CmdSetupPlayer(name, color);

        floatingInfo.transform.parent = null;
    }


    public override void OnStartClient()
    {
        if (!isLocalPlayer)
        {
            floatingInfo.transform.parent = null;
            ClearWeaponRootParent();
        }
    }

    void ClearWeaponRootParent()
    {
        foreach (var item in weaponArray)
        {
            if (item != null)
            {
                item.transform.parent = null;
            }
        }
    }

    [Command]
    public void CmdSetupPlayer(string name, Color color)
    {
        playerName = name;
        playerColor = color;
        sceneScript.statusText = $"{playerName} joined...";
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer) return;

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
    #endregion

    #region Weapon
    int selectedWeaponLocal = 1;
    public GameObject[] weaponArray;
    public GameObject weaponRoot;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced = 1;

    void OnWeaponChanged(int _Old, int _New)
    {
        if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
            weaponArray[_Old].SetActive(false);

        if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
            weaponArray[_New].SetActive(true);
    }

    [Command]

    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeaponSynced = newIndex;
    }


    #endregion

    void UpdateFloatingInfoPosition(Vector3 pos)
    {
        floatingInfo.transform.position = pos + Vector3.up * 1.5f;
        floatingInfo.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}