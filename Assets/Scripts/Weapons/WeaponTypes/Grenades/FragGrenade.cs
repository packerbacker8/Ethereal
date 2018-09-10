using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FragGrenade : Grenades
{
    public GameObject explosionEffect;

    //public float explosionForce;

    [Command]
    public void CmdExplode()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, damageRadius);
        Dictionary<string, PlayerManager> playersHit = new Dictionary<string, PlayerManager>();
        foreach (Collider col in cols)
        {
            if (col.GetComponent<PlayerManager>() != null)
            {
                playersHit.Add(col.gameObject.name, col.GetComponent<PlayerManager>());
            }
        }

        foreach(PlayerManager player in playersHit.Values)
        {
            float percentDmg = 1 - (Mathf.Abs(Vector3.Distance(this.transform.position.normalized, player.transform.position.normalized)));
            float adjustedDmg = damage * percentDmg;
            player.RpcTakeDamage((int)adjustedDmg, player.name, killValue, teamValue);
        }
    }

    public override void SetupWeapon(GameObject player)
    {
        base.SetupWeapon(player);
    }
}
