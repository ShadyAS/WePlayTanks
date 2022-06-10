using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;

public class PauseMenu : NetworkBehaviour
{
    public static bool isOn = false;

    NetworkManager networkManager;
    [SerializeField] UI playerUi;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void LeaveRoomButton()
    {
        Destroy(playerUi.owner.gameObject);
    }
}
