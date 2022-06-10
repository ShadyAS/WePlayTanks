using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TankController : NetworkBehaviour
{
    [Header("Projectile")]
    [SerializeField] GameObject canon;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] GameObject projectile;

    [Header("Mine")]
    [SerializeField] GameObject mine;
    [SerializeField] Transform mineSpawn;

    [Header("Feedbacks")]
    [SerializeField] GameObject trace;
    [SerializeField] ParticleSystem muzzleFlash;

    [Header("Sounds")]
    [SerializeField] AudioClip idleSound;

    Rigidbody rb;
    AudioSource audioSource;
    float pitchTankEngine = 1f;

    Camera main;

    [SyncVar]
    int score = 0;

    public int Score { get => score; set => score = value; }

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        Score = 0;
    }

    float traceTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isOn)
        {
            // Move
            float vertMove = Input.GetAxis("Vertical");
            float horMove = Input.GetAxis("Horizontal");

            rb.velocity = transform.forward * vertMove * 8.0f;
            transform.Rotate(Vector3.up, horMove * Time.deltaTime * 180.0f);

            if (vertMove != 0)
            {
                traceTimer += Time.deltaTime;

                MovingSound();
            }
            else
            {
                NonMovingSound();
            }

            if (traceTimer > 0.05f)
            {
                CmdDisplayTrace();
                traceTimer = 0f;
            }

            // Fire
            if (Input.GetButtonDown("Fire1"))
            {
                CmdOnShoot();
                CmdProcessFire();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                CmdDropMine();
            }
        }       

        // Rotate Canon
        Vector3 mousePos = main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookAt = new Vector3(mousePos.x, canon.transform.position.y, mousePos.z);

        canon.transform.LookAt(lookAt);        
    }

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    [ClientRpc]
    void RpcDoShootEffect()
    {
        muzzleFlash.Play();
    }

    [Command]
    void CmdDisplayTrace()
    {
        Vector3 angle = trace.transform.eulerAngles + transform.eulerAngles;
        GameObject go = Instantiate(trace, transform.position, Quaternion.Euler(angle));

        LevelManager.Instance.tankTraces.Add(go);

        NetworkServer.Spawn(go);
    }

    [Command]
    void CmdProcessFire()
    {
        GameObject proj = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
        proj.GetComponent<Projectile>().Owner = this;

        NetworkServer.Spawn(proj);
    }

    public void IncreaseScore()
    {
        score++;
    }

    [Command]
    void CmdDropMine()
    {
        GameObject go = Instantiate(mine, mineSpawn.position, mineSpawn.rotation);
        go.GetComponent<Mine>().Owner = this;

        NetworkServer.Spawn(go);
    }

    private void OnDestroy()
    {   
        if (NetworkManager.singleton)
        {
            NetworkManager.singleton.StopHost();
        }

        LevelManager.Instance.ResetDefaults();
        GameManager.Instance.ResetDefaults();
    }

    void MovingSound()
    {        
        if (pitchTankEngine < 1.2f)
        {
            pitchTankEngine += 0.01f;
            audioSource.pitch = pitchTankEngine;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void NonMovingSound()
    {
        if (pitchTankEngine > 0.9f)
        {
            pitchTankEngine -= 0.01f;
            audioSource.pitch = pitchTankEngine;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
