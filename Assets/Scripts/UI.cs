using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] public GameObject pauseMenu;

    [SerializeField] GameObject crosshair;

    public TankController owner;

    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.isOn = false;
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameLaunched)
        {
            // Pause Menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenuPause();
            }
        }
    }

    public void ToggleMenuPause()
    {
        crosshair.SetActive(!crosshair.activeSelf);
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }
}
