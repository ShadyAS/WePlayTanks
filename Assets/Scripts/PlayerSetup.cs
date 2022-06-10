using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] GameObject playerUIprefab;
    GameObject playerUIinstance;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            foreach (Behaviour bh in componentsToDisable)
            {
                bh.enabled = false;
            }
        }
        else
        {
            playerUIinstance = Instantiate(playerUIprefab);
            playerUIinstance.GetComponent<UI>().owner = GetComponent<TankController>();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        RegisterPlayerOnStart();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        RegisterPlayerOnStart();
    }

    private void RegisterPlayerOnStart()
    {
        GameManager.Instance.RegisterPlayer(GetComponent<TankController>());
    }

    private void OnDisable()
    {
        Destroy(playerUIinstance);

        int id = int.Parse(transform.name.Substring(6));

        //GameManager.Instance.UnregisterPlayer(id);        
    }
}
