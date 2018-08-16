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
    private bool tryReload = false;
    private bool shooting = false;

    private float timeSinceLastShot = 0;
    private float reloadTime = 0;
    private float spraySettleTime = 0;

    private int currentAmmo;
    private int currentWeaponIndex = 0;
    private int sprayIndex = 0;

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
        tryReload = Input.GetKeyDown(reload);

        if(tryReload && !reloading && currentAmmo < weapon.ammo)
        {
            StartReload();
        }
        else if (reloading)
        {
            reloadTime += Time.deltaTime;
            if(reloadTime >= weapon.timeToReload)
            {
                FinishReload();
            }
        }

        if (shooting && !reloading && timeSinceLastShot == 0 && currentAmmo > 0)
        {
            Shoot();
        }
        else if (!shooting)
        {
            if(sprayIndex != 0)
            {
                spraySettleTime += Time.deltaTime;
                if (spraySettleTime > weapon.settleTime)
                {
                    sprayIndex = sprayIndex <= 0 ? 0 : sprayIndex - 1;
                }
            }
        }
        timeSinceLastShot -= Time.deltaTime;
        timeSinceLastShot = timeSinceLastShot <= 0 ? 0 : timeSinceLastShot;
    }

    private void Shoot()
    {
        timeSinceLastShot = weapon.GetRateOfFireTime();
        currentAmmo--;
        RaycastHit hit;
        Vector3 startPos = cam.transform.position;
        Debug.Log("Spray index: " + sprayIndex);
        Vector3 dir = cam.transform.forward + new Vector3(weapon.SprayPatternX[sprayIndex], weapon.SprayPatternY[sprayIndex], 0);
        Debug.DrawLine(startPos, dir * weapon.range, new Color(0.0333f * (sprayIndex + 1), 0, 0, 1), 10);
        if (Physics.Raycast(startPos, dir, out hit, weapon.range, mask))
        {
            Debug.Log("We hit " + hit.collider.name);
        }
        spraySettleTime = 0;
        sprayIndex++;
    }

    private void StartReload()
    {
        reloading = true;
        reloadTime = 0;
    }

    private void FinishReload()
    {
        reloading = false;
        weapon.totalAmmo -= (weapon.ammo - currentAmmo);
        currentAmmo = weapon.ammo;
        sprayIndex = 0;
    }
}
