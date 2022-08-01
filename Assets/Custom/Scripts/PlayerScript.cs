using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
    float speed = 40f;
    Rigidbody rb;
    Vector2 turn;
    public Vector2 velocity;
    public TextMesh playerNameText;
    public GameObject floatingInfo;

    private int selectedWeaponLocal = 1;
    public GameObject[] weaponArray;

    private Material playerMaterialClone;

    private SceneScript sceneScript;

    private Weapon activeWeapon;
    private float weaponCooldownTime;

    public InputField inputFieldMessage;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [SyncVar(hook = nameof(OnColorChanged))]
    public Color playerColor = Color.white;
    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced = 1;

    public override void OnStartLocalPlayer()
    {

        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);

        floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.0f);
        floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        string name = "Player" + Random.Range(100, 999);
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        CmdSetupPlayer(name, color);
        sceneScript.playerScript = this;

        rb = GetComponent<Rigidbody>();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Awake()
    {
        sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;

        //disable all weapons
        foreach (var item in weaponArray)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }

        if (selectedWeaponLocal < weaponArray.Length && weaponArray[selectedWeaponLocal] != null)
        {
            activeWeapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();
            sceneScript.UIAmmo(activeWeapon.weaponAmmo);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) 
        {
            floatingInfo.transform.LookAt(Camera.main.transform); //makes floating info always look towards local player
            return; //prevents players from controlling eachother
        }

        turn.x = Input.GetAxis("Mouse X") * .5f;
        turn.y = Input.GetAxis("Mouse Y") * .5f;

        Vector2 moveX = new Vector2(Input.GetAxisRaw("Horizontal") * transform.right.x, Input.GetAxisRaw("Horizontal") * transform.right.z);
        Vector2 moveZ = new Vector2(Input.GetAxisRaw("Vertical") * transform.forward.x, Input.GetAxisRaw("Vertical") * transform.forward.z);

        velocity = (moveX + moveZ).normalized * speed * Time.deltaTime;

        rb.transform.Rotate(0, turn.x, 0);
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y) * speed;

        if (Input.GetButtonDown("Fire2"))
        {
            selectedWeaponLocal += 1;
            if (selectedWeaponLocal > weaponArray.Length)
            {
                selectedWeaponLocal = 1;
            }
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (activeWeapon && Time.time > weaponCooldownTime && activeWeapon.weaponAmmo > 0)
            {
                weaponCooldownTime = Time.time + activeWeapon.weaponCooldown;
                activeWeapon.weaponAmmo -= 1;
                sceneScript.UIAmmo(activeWeapon.weaponAmmo);
                CmdShootRay();
            }
        }
    }

    void OnNameChanged(string _Old, string _New)
    {
        playerNameText.text = playerName;
    }

    void OnColorChanged(Color _Old, Color _New)
    {
        playerNameText.color = _New;
        playerMaterialClone = new Material(GetComponent<Renderer>().material);
        playerMaterialClone.color = _New;
        GetComponent<Renderer>().material = playerMaterialClone;
    }

    void OnWeaponChanged(int _Old, int _New)
    {
        //disable old weapon
        if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
        {
            weaponArray[_Old].SetActive(false);
        }

        //enable new weapon
        if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
        {
            weaponArray[_New].SetActive(true);
            activeWeapon = weaponArray[activeWeaponSynced].GetComponent<Weapon>();
            if (isLocalPlayer)
            {
                sceneScript.UIAmmo(activeWeapon.weaponAmmo);
            }
        }
    }
    [Command]
    void CmdShootRay()
    {
        RpcFireWeapon();
    }
    [ClientRpc]
    void RpcFireWeapon()
    {
        GameObject bullet = Instantiate(activeWeapon.weaponBullet, activeWeapon.weaponFirePosition.position, activeWeapon.weaponFirePosition.rotation); //spawns bullet 
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * activeWeapon.weaponSpeed; //edits bullets velocity to make it shoot out
        Destroy(bullet, activeWeapon.weaponLife); //supposed to delete shot after set timeds
    }
    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeaponSynced = newIndex; 
    }
    [Command]
    public void CmdSetupPlayer(string _name, Color _col)
    {
        playerName = _name;
        playerColor = _col;
        sceneScript.statusText = $"{playerName} joined";
    }
    
    [Command]
    public void CmdSendPlayerMessage(string _text)
    { //TODO: add custom messages. would include locally controlled editable text bar
        if (sceneScript)
            sceneScript.statusText = $"{playerName}: " + _text;
    }
}
