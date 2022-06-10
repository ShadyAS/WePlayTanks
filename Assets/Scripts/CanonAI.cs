using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonAI : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] EnemyLevel level;

    [Header("Stats")]
    [SerializeField] float canonRotationSpeed = 10f;
    [SerializeField] float overlapRange = 10f;

    [Header("Projectile")]
    [SerializeField] ProjectileEnemy projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] float fireRate;
    [SerializeField] int bulletLimit;

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isOn)
        {
            CheckForEnemies();

            if (players.Count != 0)
            {
                Vector3 dir = (players[0].transform.position - transform.position).normalized;
                Quaternion lookRot = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * canonRotationSpeed);

                Shoot();
            }
            else
            {
                LookForPlayer();
            }
        }
    }

    public List<Collider> players = new List<Collider>();
    void CheckForEnemies()
    {
        players.Clear();

        Collider[] cols = Physics.OverlapSphere(transform.position, overlapRange);

        if (level != EnemyLevel.ONE)
        {
            foreach (Collider c in cols)
            {
                if (c.CompareTag("Player"))
                {
                    players.Add(c);
                }
            }
        }
        else
        {
            foreach (Collider c in cols)
            {
                if (c.CompareTag("Player"))
                {
                    Vector3 direction = (transform.position - c.transform.position).normalized;
                    direction.y *= 0;

                    float angle = Vector3.Angle(-transform.forward, direction);

                    if (angle <= 30f)
                    {
                        players.Add(c);
                    }                    
                }
            }
        }

        if (players.Count == 2)
        {
            players.Sort(DistanceSort);
        }
    }

    int DistanceSort(Collider a, Collider b)
    {
        return (transform.position - a.transform.position).sqrMagnitude.CompareTo((transform.position - b.transform.position).sqrMagnitude);
    }

    Vector2 point2;
    Vector3 randomPoint;
    float tmpAngle;
    bool isRotating = false;

    void LookForPlayer()
    {
        if (!isRotating)
        {
            point2 = Random.insideUnitCircle * overlapRange;
            randomPoint = new Vector3(point2.x, 0, point2.y);

            tmpAngle = Vector3.Angle(transform.forward, (randomPoint - transform.position).normalized);
        }

        if (tmpAngle <= 60f)
        {
            Vector3 dir = (randomPoint - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * canonRotationSpeed / 6f);

            isRotating = true;

            tmpAngle = Vector3.Angle(transform.forward, (randomPoint - transform.position).normalized);
            if (tmpAngle <= 2f)
            {
                isRotating = false;
            }
        }
    }

    public List<ProjectileEnemy> projs = new List<ProjectileEnemy>();
    float shootTimer = 0f;
    void Shoot()
    {
        Ray ray = new Ray(transform.position, players[0].transform.position - transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, overlapRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (projs.Count < bulletLimit)
                {
                    shootTimer += Time.deltaTime;
                    if (shootTimer >= fireRate)
                    {
                        ProjectileEnemy go = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
                        go.launcher = this;
                        projs.Add(go);

                        shootTimer = 0f;
                    }                    
                }
            }
        }
    }
}
