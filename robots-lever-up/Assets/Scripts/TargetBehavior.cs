using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    [SerializeField] Transform nozzle;
    [SerializeField] Transform bulletPrefab;

    void Start()
    {
        InvokeRepeating(nameof(Shoot), 1f, 1f);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 3 + 1.5f * Mathf.Sin(Time.time * 2), transform.position.z);   
        transform.Rotate(Vector3.up * 60 * Time.deltaTime);
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, nozzle);
    }
}
