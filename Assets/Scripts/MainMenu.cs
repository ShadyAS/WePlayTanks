using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

public class MainMenu : NetworkBehaviour
{
    NetworkManager networkManager;

    [SerializeField] InputField hostAdress;
    [SerializeField] GameObject menu;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void Host()
    {
        networkManager.StartHost();
    }

    public void Join()
    {
        networkManager.StartClient();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
