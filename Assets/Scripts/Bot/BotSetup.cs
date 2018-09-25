using UnityEngine;
using UnityEngine.Networking;

public class BotSetup : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] componentsToDisable;

    private void Start()
    {
        DisableComponents();
        AssignRemoteLayer();
        this.GetComponentInChildren<BotInventoryScript>().SetupInventory();
    }

    /// <summary>
    /// When the client first joins the game this method is called, and registers with the game manager
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager.RegisterBot(this.GetComponent<NetworkIdentity>().netId.ToString(), this.GetComponent<BotManager>());
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
        gameObject.layer = LayerMask.NameToLayer(Constants.REMOTE_PLAYER_LAYER);
    }

    private void OnDisable()
    {
        GameManager.DeRegisterBot(this.GetComponent<NetworkIdentity>().netId.ToString());
    }

    /// <summary>
    /// Temporary helper method to get spawn transform for bots
    /// without having to use network behavior in the manager.
    /// </summary>
    public Transform GetSpawnPoint()
    {
        return NetworkManager.singleton.GetStartPosition();
    }
}
