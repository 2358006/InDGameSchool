using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.Assertions;
//using Random
public class PlayerController : NetworkBehaviour
{
    //private PlayerInputAction Player
    PlayerInputAction playerInput;
    private SceneScript sceneScript;

    public TextMesh playerNameText;
    public GameObject floatingInfo;

    private Weapon activeWeapon;
    private float weaponCooldownTime;

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
        
        //sceneScript = GameObject.FindObjectOfType<SceneScript>();
        
        foreach (var item in weaponArray)
        {
            if (item != null)
                item.SetActive(false);
        }
        
        sceneScript = GameObject.Find("SceneRefer").GetComponent<SceneRefer>().sceneScript;
        Debug.Log(sceneScript);

        Assert.IsNotNull(sceneScript);

        activeWeapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();

        if(selectedWeaponLocal<weaponArray.Length &&weaponArray[selectedWeaponLocal] != null)
        {
            sceneScript.UIAmmo(activeWeapon.weaponAmmo);
        }
        
    }
    public void UpdateFloatingInfoPosition(Vector3 pos)
    {
        //Vector3 pos = this.transform.position;
        floatingInfo.transform.position = pos + Vector3.up * 1.5f;
        floatingInfo.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

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

        if(Input.GetButtonDown("Fire2"))
        {
            var weapon = selectedWeaponLocal + 1;

            if (weapon > weaponArray.Length)
                weapon = 1;

            selectedWeaponLocal = weapon;

            CmdChangeActiveWeapon(selectedWeaponLocal);

        }
        if(Input.GetButtonDown("Fire1"))
        {
            if(activeWeapon&&Time.time>weaponCooldownTime && activeWeapon.weaponAmmo>0)
            {
                weaponCooldownTime = Time.time + activeWeapon.weaponCooldown;
                activeWeapon.weaponAmmo -= 1;
                sceneScript.UIAmmo(activeWeapon.weaponAmmo);
                CmdShootRay();
            }
        }
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

       // Assert.IsNotNull

        sceneScript.playerScript = this;

        Camera mainCam = Camera.main;

        //Debug.LogError(mainCam);

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
        ClearWeaponRootParent();
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
        WeaponRoot.transform.parent = null;
       
    }
    [Command]
    public void CmdSetupPlayer(string name, Color color)
    {
        playerName = name;
        playerColor = color;
        sceneScript.statusText = $"{playerName} joined...";
    }

    [Command]
    void CmdShootRay()
    {
        RpcFireWeapon();
    }

    [ClientRpc]
    void RpcFireWeapon()
    {
        GameObject bullet = Instantiate(
            activeWeapon.weaponBullet, 
            activeWeapon.weaponBulletPos.position,
            activeWeapon.weaponBulletPos.rotation);
        bullet.GetComponent<Rigidbody>().velocity = 
            bullet.transform.forward * activeWeapon.weaponSpeed;
        Destroy(bullet, activeWeapon.weaponLife);
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

    private int selectedWeaponLocal = 1;


    public GameObject WeaponRoot;
    public GameObject[] weaponArray;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced = 1;

    void OnWeaponChanged(int _Old, int _New)
    {
        Debug.Log("tst");

        if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
            weaponArray[_Old].SetActive(false);

        if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
        {
            weaponArray[_New].SetActive(true);
            activeWeapon = weaponArray[activeWeaponSynced].GetComponent<Weapon>();
            if(isLocalPlayer) {
                sceneScript.UIAmmo(activeWeapon.weaponAmmo);
            }
        }
    }

    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeaponSynced = newIndex;
    }

    #endregion

    void UpdateWeaponPosition()
    {
        WeaponRoot.transform.position = this.transform.position;
        var rot = this.transform.rotation.eulerAngles * Mathf.Rad2Deg;
        //WeaponRoot.transform.rotation = Qu
        var currAngle = WeaponRoot.transform.rotation.eulerAngles.y;
        var nextAngle = Mathf.Atan2(movementX, movementY)*Mathf.Rad2Deg;
        var angle = Mathf.LerpAngle(currAngle, nextAngle,3f* Time.deltaTime);
        WeaponRoot.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        /*
        WeaponRoot.transform.rotation = Quaternion.Slerp(
            Quaternion.Euler(0f, currAngle, 0f), 
            Quaternion.Euler(0f, nextangle, 0f), 
            Time.deltaTime);
        */
        //WeaponRoot.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f);
    }
}
