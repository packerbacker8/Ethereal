﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    //public GameObject[] weapons;

    #region combat keycodes
    [Header("Combat Keys")]
    public KeyCode primaryAction = KeyCode.Mouse0;
    public KeyCode secondaryAction = KeyCode.Mouse1;
    public KeyCode reload = KeyCode.R;
    public KeyCode melee = KeyCode.F;
    public KeyCode quickGrenade = KeyCode.G;
    #endregion

    [Header("Debug Variables")]
    public bool showSprayLines = true;

    private Camera cam;
    private Weapon weapon;
    private WeaponManager weaponManager;
    private ObjectPool hitEffectPool = null;
    private ObjectPoolManager allPools;
    private PlayerMotor motor;

    private bool reloading = false;
    private bool tryReload = false;
    private bool shooting = false;

    private float timeSinceLastShot = 0;
    private float reloadTime = 0;
    private float spraySettleTime = 0;

    private int currentAmmo;
    private int sprayIndex = 0;

    [SerializeField]
    private LayerMask mask;

    private void Start()
    {
        cam = this.GetComponentInChildren<Camera>();
        if (cam == null)
        {
            Debug.LogError("Player shoot: no camera referenced");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
        allPools = GameObject.FindGameObjectWithTag(Constants.OBJECT_POOLS_TAG).GetComponent<ObjectPoolManager>();
        StartCoroutine(GetHitEffectPool());
        motor = this.GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        //if weapon is null we don't want to keep doing weapon related stuff
        if (PauseMenu.isPaused || weapon == null || PlayerUIScript.IsInEconomyMenu)
        {
            return;
        }
        shooting = Input.GetKey(primaryAction);
        tryReload = Input.GetKeyDown(reload);

        if (tryReload && !reloading && currentAmmo < weapon.ammo && isLocalPlayer)
        {
            StartReload();
        }
        else if (reloading)
        {
            reloadTime += Time.deltaTime;
            if (reloadTime >= weapon.timeToReload)
            {
                FinishReload();
            }
        }

        if(shooting && timeSinceLastShot != 0)
        {
            motor.AddShootingMotionX(new Vector3(0, weapon.kickDir.x * weapon.settleAmount * -1, 0));
            motor.AddShootingMotionY(weapon.kickDir.y * weapon.settleAmount * -1);
        }
        if (shooting && !reloading && timeSinceLastShot == 0 && currentAmmo > 0)
        {
            motor.AddShootingMotionX(new Vector3(0, weapon.kickDir.x, 0));
            motor.AddShootingMotionY(weapon.kickDir.y);
            Shoot();
        }
        else if (!shooting || currentAmmo == 0 || reloading)
        {
            if (sprayIndex != 0)
            {
                spraySettleTime += Time.deltaTime;
                if (spraySettleTime > weapon.settleTime)
                {
                    sprayIndex = sprayIndex <= 0 ? 0 : sprayIndex - 1;
                    spraySettleTime = 0;
                    
                }

            }
            motor.AddShootingMotionX(Vector3.zero);
            motor.AddShootingMotionY(0);
        }
        timeSinceLastShot -= Time.deltaTime;
        timeSinceLastShot = timeSinceLastShot <= 0 ? 0 : timeSinceLastShot;
    }

    /// <summary>
    /// A player has shot. Tell the clients to play the effects.
    /// </summary>
    [Command]
    private void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    /// <summary>
    /// All players are being updated to play shoot effect
    /// </summary>
    [ClientRpc]
    private void RpcDoShootEffect()
    {
        if (weapon == null)
        {
            //WeaponChanged();
            return;
        }
        weapon.weaponGraphics.GetComponentInChildren<WeaponGraphics>().muzzleFlash.Play();
    }

    /// <summary>
    /// Called on server when something is hit.
    /// </summary>
    /// <param name="hitPos"></param>
    /// <param name="normalPos"></param>
    [Command]
    private void CmdOnHitEffect(Vector3 hitPos, Vector3 normalPos)
    {
        RpcDoHitEffect(hitPos, normalPos);
    }

    /// <summary>
    /// Called on all clients and spawns hit effect.
    /// </summary>
    /// <param name="hitPos"></param>
    /// <param name="normalPos"></param>
    [ClientRpc]
    private void RpcDoHitEffect(Vector3 hitPos, Vector3 normalPos)
    {
        if (hitEffectPool == null)
        {
            Debug.LogError("Effect pool was null");
            return;
        }
        GameObject effect = hitEffectPool.GetComponent<ObjectPool>().GetPooledObject();
        effect.transform.position = hitPos;
        effect.transform.rotation = Quaternion.LookRotation(normalPos);
        effect.SetActive(true);
    }

    /// <summary>
    /// Client shoot only runs on the client.
    /// </summary>
    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer || weapon == null)
        {
            return;
        }
        //we are shooting, tell server to do the shoot effects
        CmdOnShoot();

        timeSinceLastShot = weapon.GetRateOfFireTime();
        currentAmmo--;
        weapon.currentAmmo = currentAmmo;
        Vector3 startPos = cam.transform.position;
        Vector3 sprayVal;
        if (motor.isMoving)
        {
            sprayVal = new Vector3(weapon.MovingSprayPatternX[sprayIndex], weapon.MovingSprayPatternY[sprayIndex], 0);
        }
        else
        {
            sprayVal = new Vector3(weapon.SprayPatternX[sprayIndex], weapon.SprayPatternY[sprayIndex], 0);
        }
        Vector3 dir = cam.transform.forward + sprayVal;
        if (showSprayLines)
        {
            Debug.DrawLine(startPos, dir * weapon.range, new Color(0.0333f * (sprayIndex + 1), 0, 0, 1), 10);
        }
        RaycastHit hit;
        if (Physics.Raycast(startPos, dir, out hit, weapon.range, mask))
        {
            if (hit.collider.tag == Constants.PLAYER_TAG)
            {
                CmdShootPlayer(hit.collider.name, weapon.damage, this.name, weapon.killValue, weapon.teamValue);
            }
            //Hit something, call on hit to draw effect for all players
            CmdOnHitEffect(hit.point, hit.normal);

            //TODO: make this not client side only
            if(hit.collider.tag == Constants.WORLD_OBJECT_TAG)
            {
                hit.transform.gameObject.GetComponent<Rigidbody>()?.AddForceAtPosition(hit.point * weapon.damage, hit.point);
            }
        }
        spraySettleTime = 0;
        sprayIndex++;
        if (sprayIndex > weapon.SprayPatternX.Length - 1)
        {
            sprayIndex = weapon.SprayPatternX.Length - 1;
        }
    }

    [Command]
    private void CmdShootPlayer(string idOfShot, int damageDone, string shooterId, int killVal, int teamVal)
    {
        PlayerManager playerShot = GameManager.GetPlayer(idOfShot);
        playerShot.RpcTakeDamage(damageDone, shooterId, killVal, teamVal);
    }

    private void StartReload()
    {
        if (weapon == null)
        {
            return;
        }
        if (weapon.totalAmmo <= 0)
        {
            //play an out of ammo sound
            return;
        }
        reloading = true;
        reloadTime = 0;
    }

    private void FinishReload()
    {
        reloading = false;
        weapon.totalAmmo -= (weapon.ammo - currentAmmo);
        currentAmmo = weapon.ammo;
        weapon.currentAmmo = weapon.ammo;
        sprayIndex = 0;
    }

    /// <summary>
    /// Attempt to get the object pool containing the hit effects quickly, but keep trying if it doesn't happen immediately.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetHitEffectPool()
    {
        int tries = 0;
        float timeToWait = 0.1f;
        GameObject hitEffectObj;
        while (hitEffectPool == null && tries < 10)
        {
            hitEffectObj = allPools.GetObjectPoolByName(Constants.HIT_EFFECT_POOL_NAME);
            if (hitEffectObj != null)
            {
                hitEffectPool = hitEffectObj.GetComponent<ObjectPool>();
            }
            yield return new WaitForSeconds(timeToWait);
            timeToWait += 0.5f;
            tries++;
        }
        yield return null;
    }

    /// <summary>
    /// Returns the weapon the player currently has equipped.
    /// </summary>
    /// <returns></returns>
    public Weapon GetCurrentWeapon()
    {
        return weapon;
    }

    public void WeaponChanged()
    {
        if (weaponManager != null)
        {
            Weapon tempWeapon = weaponManager.GetCurrentWeapon();
            //we have no weapons
            if (tempWeapon == null)
            {
                weapon = null;
                return;
            }
            switch (tempWeapon.gameObject.name)
            {
                case ("AK47"):
                    weapon = tempWeapon as AK47Script;
                    break;
                case ("UMP45"):
                    weapon = tempWeapon as UMP45Script;
                    break;
                default:
                    weapon = tempWeapon as AK47Script;
                    break;
            }
            weapon.SetupWeapon();
            currentAmmo = weapon.currentAmmo;
            sprayIndex = 0;
        }
    }
}
