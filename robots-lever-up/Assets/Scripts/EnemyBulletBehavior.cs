using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBehavior : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float force;
    [SerializeField] private GameObject explosion;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * force);
        GameObject.Destroy(gameObject, 2.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<JaegerMovement>().TakeDamage();
            Instantiate(explosion, collision.transform.position, Quaternion.identity);
            GameObject.Destroy(gameObject);
        }
    }
}
