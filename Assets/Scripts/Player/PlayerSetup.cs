using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour
{
    public string remoteLayer = "RemotePlayer";

    [SerializeField]
    private Behaviour[] componentsToDisable;

    [SerializeField]
    private GameObject playerUIPrefab;
    private GameObject playerUI;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
            playerUI = null;
        }
        else
        {
            //create player ui
            playerUI = Instantiate(playerUIPrefab);
            playerUI.name = "PlayerUI";            
            this.GetComponent<PlayerManager>().SetupPlayer();
            playerUI.GetComponent<PlayerUIScript>().SetPlayer(this.gameObject);
        }

    }

    /// <summary>
    /// When the client first joins the game this method is called, and registers with the game manager
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager.RegisterPlayer(this.GetComponent<NetworkIdentity>().netId.ToString(), this.GetComponent<PlayerManager>());
    }

    /// <summary>
    /// Components in this list are turned off, so local client doesnt affect their networked clones.
    /// </summary>
    private void DisableComponents()
    {
        foreach (Behaviour component in componentsToDisable)
        {
            component.enabled = false;
        }
    }

    private void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayer);
    }

    private void OnDisable()
    {
        Destroy(playerUI);
        if (isLocalPlayer)
        {
            GameManager.instance.SetWorldCameraActive(true);
        }
        GameManager.DeRegisterPlayer(this.GetComponent<NetworkIdentity>().netId.ToString());
    }


    public GameObject GetPlayerUI()
    {
        return playerUI;
    }
}
