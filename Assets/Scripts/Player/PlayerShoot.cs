using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    public GameObject[] weapons;

    #region combat keycodes
    [Header("Combat Keys")]
    public KeyCode primaryAction = KeyCode.Mouse0;
    public KeyCode secondaryAction = KeyCode.Mouse1;
    public KeyCode interact = KeyCode.E;
    public KeyCode reload = KeyCode.R;
    public KeyCode melee = KeyCode.F;
    public KeyCode quickSwitch = KeyCode.Q;
    public KeyCode quickGrenade = KeyCode.G;
    #endregion

    private Camera cam;
    private Weapon weapon;

    private bool reloading = false;
    private bool shooting = false;

    private float timeSinceLastShot = 0;

    private int currentAmmo;
    private int currentWeaponIndex = 0;

    [SerializeField]
    private LayerMask mask;

    private void Start()
    {
        cam = this.GetComponentInChildren<Camera>();
        if(cam == null)
        {
            Debug.LogError("Player shoot: no camera referenced");
            this.enabled = false;
        }
        weapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        currentAmmo = weapon.ammo;
    }

    private void Update()
    {
        shooting = Input.GetKey(primaryAction);

        if (shooting && timeSinceLastShot == 0 && currentAmmo > 0)
        {
            Shoot();
        }
        timeSinceLastShot -= Time.deltaTime;
        timeSinceLastShot = timeSinceLastShot <= 0 ? 0 : timeSinceLastShot;
    }

    private void Shoot()
    {
        timeSinceLastShot = weapon.rateOfFire;
        currentAmmo--;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            Debug.Log("We hit " + hit.collider.name);
        }
    }
}
