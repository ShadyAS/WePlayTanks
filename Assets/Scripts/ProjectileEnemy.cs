using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ProjectileEnemy : NetworkBehaviour
{
    Rigidbody rb;

    [SyncVar]
    Vector3 currentVelocity;

    [SerializeField] float speed;
    [SerializeField] int nbBounds = 1;
    [SerializeField] bool isDestroyingObstacle = false;

    public CanonAI launcher;

    [SerializeField] GameObject cross;
    [SerializeField] GameObject selfExplo;
    [SerializeField] GameObject tankExplo;
    [SerializeField] AudioClip wallHit;

    AudioSource audioSource;

    GameObject toDestroy;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        currentVelocity = transform.forward * speed;

        Destroy(gameObject, 15f);

        if (GameManager.Instance.inGameUI.activeSelf)
        {
            LevelManager.Instance.projEn.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = currentVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroyingObstacle && collision.gameObject.CompareTag("Breakable"))
        {
            NetworkServer.Destroy(collision.gameObject);
            NetworkServer.Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            toDestroy = collision.gameObject;

            CmdTankExplo();
            CmdDisplayCross();

            collision.gameObject.SetActive(false);
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

    private void OnDestroy()
    {
        launcher.projs.Remove(this);

        if (GameManager.Instance.inGameUI.activeSelf)
        {
            LevelManager.Instance.projEn.Remove(this);
        }
    }

    void CmdDisplayCross()
    {
        GameObject go1 = Instantiate(cross, toDestroy.transform.position + new Vector3(0f, 0.1f, 0f), cross.transform.rotation);

        if (toDestroy.name == "Player1")
        {
            go1.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            go1.GetComponent<SpriteRenderer>().color = Color.red;
        }

        LevelManager.Instance.tankCrosses.Add(go1);

        NetworkServer.Spawn(go1);
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
