using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera worldCamera;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            foreach(Behaviour component in componentsToDisable)
            {
                component.enabled = false;
            }
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

    private void OnDisable()
    {
        if(worldCamera != null)
        {
            //temporary - will be moved to a different location at a future date
            Camera.main.gameObject.SetActive(true);
        }
    }
}
