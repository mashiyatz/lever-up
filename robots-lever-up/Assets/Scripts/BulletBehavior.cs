using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float force;
    [SerializeField] private GameObject explosion;
    public static event Action OnEnemyDeath;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force);
        GameObject.Destroy(gameObject, 2.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Target")) {
            GameObject.Destroy(collision.gameObject);
            Instantiate(explosion, collision.transform.position, Quaternion.identity);
            OnEnemyDeath.Invoke();
            GameObject.Destroy(gameObject);
        }
    }
}
