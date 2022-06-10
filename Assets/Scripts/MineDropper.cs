using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineDropper : MonoBehaviour
{
    [Header("Mine")]
    [SerializeField] MineEnemy mine;
    [SerializeField] Transform mineSpawn;
    [SerializeField] int mineLimit;

    public List<MineEnemy> mines = new List<MineEnemy>();

    GameObject target = null;

    // Update is called once per frame
    void Update()
    {
        if (CheckForPlayersAround())
        {
            DropMine();
        }
    }

    private void OnDestroy()
    {
        DropMine();
    }

    bool CheckForPlayersAround()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);

        foreach (Collider c in colliders)
        {
            if (c.CompareTag("Player"))
            {
                target = c.gameObject;
                return true;
            }
        }

        target = null;
        return false;
    }

    float dropTimer = 0f;
    void DropMine()
    {
        if (mines.Count < mineLimit)
        {
            dropTimer += Time.deltaTime;
            if (dropTimer > 2f)
            {
                MineEnemy go = Instantiate(mine, mineSpawn.position, mineSpawn.rotation);
                go.launcher = this;
                mines.Add(go);

                dropTimer = 0f;
            }           
        }
    }
}
