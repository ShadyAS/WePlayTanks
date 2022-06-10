﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mine : NetworkBehaviour
{
    TankController owner;

    [SerializeField] Material explodingMat;
    [SerializeField] float explodingRadius = 5f;

    MeshRenderer renderer;

    float lifeTime = 0f;
    float explosionTime = 0f;

    public bool hasExplode = false;

    public TankController Owner { get => owner; set => owner = value; }

    [SerializeField] GameObject cross;
    [SerializeField] GameObject selfExplo;
    [SerializeField] GameObject tankExplo;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();

        if (GameManager.Instance.inGameUI.activeSelf)
        {
             LevelManager.Instance.mine.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > 5f)
        {
            RetardedExplosion();
        }
    }

    GameObject toDestroy;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            toDestroy = collision.gameObject;

            CmdTankExplo();

            Owner.IncreaseScore();

            CmdDisplayCross();

            NetworkServer.Destroy(collision.gameObject);

            Explode();
        }
    }

    void RetardedExplosion()
    {
        renderer.material = explodingMat;

        explosionTime += Time.deltaTime;
        if (explosionTime > 3f)
        {
            Explode();
        }
    }

    public void Explode()
    {
        hasExplode = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explodingRadius);

        foreach (Collider c in colliders)
        {   
            if (c.gameObject != gameObject)
            {
                if (c.CompareTag("Mine"))
                {
                    if (!c.gameObject.GetComponent<Mine>().hasExplode)
                    {
                        c.gameObject.GetComponent<Mine>().Explode();
                    }                    
                }

                if (c.CompareTag("MineEnemy"))
                {
                    if (!c.gameObject.GetComponent<MineEnemy>().hasExplode)
                    {
                        c.gameObject.GetComponent<MineEnemy>().Explode();
                    } 
                }

                if (c.CompareTag("Breakable") || c.CompareTag("Projectile") || c.CompareTag("ProjectileEnemy"))
                {
                    NetworkServer.Destroy(c.gameObject);
                }

                if (c.CompareTag("Enemy"))
                {
                    toDestroy = c.gameObject;

                    CmdTankExplo();

                    Owner.IncreaseScore();

                    CmdDisplayCross();

                    NetworkServer.Destroy(c.gameObject);
                }
            }
        } 

        CmdSelfExplo();  
        NetworkServer.Destroy(gameObject);
    }

    private void OnDestroy()
    {  
        if (GameManager.Instance.inGameUI.activeSelf)
        {
            LevelManager.Instance.mine.Remove(this);
        }        
    }

    void CmdTankExplo()
    {
        GameObject go = Instantiate(tankExplo, toDestroy.transform.position, selfExplo.transform.rotation);
        Destroy(go, 3f);

        NetworkServer.Spawn(go);
    }

    void CmdDisplayCross()
    {
        GameObject go = Instantiate(cross, toDestroy.transform.position+ new Vector3(0f, 0.1f, 0f), cross.transform.rotation);

        LevelManager.Instance.tankCrosses.Add(go);

        NetworkServer.Spawn(go);
    }

    void CmdSelfExplo()
    {
        GameObject go = Instantiate(selfExplo, transform.position, selfExplo.transform.rotation);
        Destroy(go, 3f);

        NetworkServer.Spawn(go);
    }
}
