using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    public string remoteLayer = "RemotePlayer";

    [SerializeField]
    private Behaviour[] componentsToDisable;

    private Camera worldCamera;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            worldCamera = Camera.main;
            if(worldCamera != null)
            {
                //temporary - will be moved to a different location at a future date
                Camera.main.gameObject.SetActive(false);
            }
        }
    }

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
        if(worldCamera != null)
        {
            //temporary - will be moved to a different location at a future date
            Camera.main.gameObject.SetActive(true);
        }
    }
}
