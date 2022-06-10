using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDetector : MonoBehaviour
{
    [SerializeField] BaseAI baseAI;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            if (IsGoingToMe(other.gameObject))
            {
                baseAI.DodgeProjectile(GetDodgeDirection(other.gameObject), other.transform);

                Debug.Log("PROJECTILE");
            }
        }
    }

    bool IsGoingToMe(GameObject _proj)
    {
        Ray ray = new Ray(_proj.transform.position, _proj.transform.forward);
        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray, 20f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                return true;
            }
        }

        return false;
    }

    DodgeDirection GetDodgeDirection(GameObject _proj)
    {
        Vector3 toProj = ( _proj.transform.position - transform.position).normalized;

        float dotFB = Vector3.Dot(transform.forward, toProj);
        float dotRL = Vector3.Dot(-transform.right, toProj);

        // Back
        if (dotFB < 0)
        {
            // Left
            if (dotRL > 0)
            {
                Debug.Log("bl");
                return DodgeDirection.BACK_LEFT;
            }
            // Right
            else if (dotRL < 0)
            {
                Debug.Log("br");
                return DodgeDirection.BACK_RIGHT;
            }
        }
        // Front
        else if (dotFB > 0)
        {
            // Left
            if (dotRL > 0)
            {
                Debug.Log("fl");
                return DodgeDirection.FRONT_LEFT;
            }
            // Right
            else if (dotRL < 0)
            {
                Debug.Log("fr");
                return DodgeDirection.FRONT_RIGHT;
            }
        }        

        return DodgeDirection.FRONT_RIGHT;
    }
}
