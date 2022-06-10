using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum EnemyLevel
{
    ONE,
    TWO,
    THREE,
    FOUR,
    FIVE
}

public enum MovementType
{
    DEFENSIVE = 12,
    OFFENSIVE = 6,
    ULTRA_OFFENSIVE = 3
}

public enum DodgeDirection
{
    FRONT_RIGHT,
    FRONT_LEFT,
    BACK_RIGHT,
    BACK_LEFT
}

public class BaseAI : NetworkBehaviour
{
    [Header("Level")]
    [SerializeField] EnemyLevel level;
    [SerializeField] MovementType movementType;

    [Header("Base")]
    [SerializeField] public GameObject tankBase;
    [SerializeField] float baseRotationSpeed;
    [SerializeField] float baseMovementSpeed;
    [SerializeField] CanonAI canon;
    [SerializeField] GameObject trace;

    Rigidbody rb;

    Vector3 target = new Vector3(0, 1, 0);

    int direction = 1;

    bool isDodging = false;
    float dodgeTimer = 0f;
    float traceTimer = 0f;
    float traceTimerLimit = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        traceTimerLimit = (baseMovementSpeed == 2) ? 0.15f : 0.075f;
    }

    void Update()
    {
        if (!PauseMenu.isOn)
        {
            if (!isDodging)
            {
                if (level != EnemyLevel.ONE)
                {
                    Movement();
                }
            }        

            if (target.y != 1)
            {
                Vector3 dir = (target - transform.position).normalized;
                Quaternion lookRot = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * baseRotationSpeed / 6f);

                rb.velocity = tankBase.transform.forward * baseMovementSpeed * direction;

                traceTimer += Time.deltaTime;
                if (traceTimer > traceTimerLimit)
                {
                    RpcDisplayTrace();
                    traceTimer = 0f;
                }

                if (isDodging)
                {
                    dodgeTimer += Time.deltaTime;
                    if (dodgeTimer > 1f)
                    {
                        StopMovement();
                        isDodging = false;
                        dodgeTimer = 0f;
                    }
                }
            }
        } 
    }

    [ClientRpc]
    void RpcDisplayTrace()
    {
        Vector3 angle = trace.transform.eulerAngles + transform.eulerAngles;
        GameObject go = Instantiate(trace, transform.position, Quaternion.Euler(angle));

        LevelManager.Instance.tankTraces.Add(go);

        NetworkServer.Spawn(go);
    }

    void Movement()
    {
        if (canon.players.Count != 0)
        {
            target = canon.players[0].transform.position;
        }

        float dist = Vector3.Distance(transform.position, target);

        if (dist < (float)movementType - 0.2f)
        {
            direction = -1;
        }
        else if (dist > (float)movementType + 0.2f)
        {
            direction = 1;
        }
        else
        {
            StopMovement();
        }

    }

    void StopMovement()
    {
        target.y = 1;
        rb.velocity = Vector3.zero;
    }

    public void DodgeProjectile(DodgeDirection _dir, Transform _proj)
    {
        isDodging = true;

        switch (_dir)
        {
            case DodgeDirection.FRONT_RIGHT:
                direction = -1;
                target = (transform.position + _proj.forward - _proj.right * 3f);
                break;
            case DodgeDirection.FRONT_LEFT:
                direction = -1;
                target = (transform.position + _proj.forward + _proj.right * 3f);
                break;

            case DodgeDirection.BACK_RIGHT:
                direction = 1;
                target = (transform.position + _proj.forward - _proj.right * 3f);
                break;
            case DodgeDirection.BACK_LEFT:
                direction = 1;
                target = (transform.position + _proj.forward + _proj.right * 3f);
                break;
        }
    }
}
