using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    TankController owner;
    Rigidbody rb;

    int nbBounds = 1;

    [SyncVar]
    Vector3 currentVelocity;

    public TankController Owner { get => owner; set => owner = value; }

    [SerializeField] GameObject cross;
    [SerializeField] GameObject selfExplo;
    [SerializeField] GameObject tankExplo;
    [SerializeField] AudioClip wallHit;
    [SerializeField] AudioClip fireSound;

    AudioSource audioSource;

    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(fireSound);

        currentVelocity = transform.forward * speed;

        Destroy(gameObject, 15f);

        if (GameManager.Instance.inGameUI.activeSelf)
        {
             LevelManager.Instance.proj.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = currentVelocity;
    }

    GameObject toDestroy;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            toDestroy = collision.gameObject;

            CmdTankExplo();

            owner.IncreaseScore();

            CmdDisplayCross();

            NetworkServer.Destroy(collision.gameObject);
            NetworkServer.Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("ProjectileEnemy"))
        {
            CmdSelfExplo();

            NetworkServer.Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (nbBounds != 0)
            {
                audioSource.PlayOneShot(wallHit);

                float speed = currentVelocity.magnitude;
                Vector3 direction = Vector3.Reflect(currentVelocity.normalized, collision.contacts[0].normal);

                currentVelocity = direction * Mathf.Max(speed, 0f);
                transform.rotation = Quaternion.LookRotation(currentVelocity);

                nbBounds--;
            }
            else
            {
                CmdSelfExplo();

                NetworkServer.Destroy(gameObject);
            }            
        }
    }

    void CmdDisplayCross()
    {
        GameObject go1 = Instantiate(cross, toDestroy.transform.position + new Vector3(0f, 0.1f, 0f), cross.transform.rotation);

        LevelManager.Instance.tankCrosses.Add(go1);

        NetworkServer.Spawn(go1);
    }

    private void OnDestroy()
    {    
        if (GameManager.Instance.inGameUI.activeSelf)
        {
            LevelManager.Instance.proj.Remove(this);
        }        
    }

    void CmdTankExplo()
    {
        GameObject go = Instantiate(tankExplo, toDestroy.transform.position, selfExplo.transform.rotation);
        Destroy(go, 3f);

        NetworkServer.Spawn(go);
    }

    void CmdSelfExplo()
    {
        GameObject go = Instantiate(selfExplo, transform.position, selfExplo.transform.rotation);
        Destroy(go, 3f);

        NetworkServer.Spawn(go);
    }
}
