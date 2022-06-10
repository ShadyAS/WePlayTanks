using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float t = Time.time / 6f;

        float x = Mathf.Sin(t) / (1 + Mathf.Pow(Mathf.Cos(t), 2));
        float y = Mathf.Sin(t) * Mathf.Cos(t) / (1 + Mathf.Pow(Mathf.Cos(t), 2));

        transform.position = new Vector3 ((x * 2f), (y * 2f) + 6.9f, transform.position.z);
        transform.LookAt(Vector3.zero + Vector3.up);
    }
}