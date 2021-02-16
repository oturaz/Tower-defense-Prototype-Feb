using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float timeLeft;
    public float damage = 10f;
    public float radius = 0f;

    public void Awake()
    {
        timeLeft = 10f;
        explosionPrefab = (GameObject)Resources.Load("Prefabs/Particles/CannonballExplosion");
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            Destroy(this.gameObject); //safety mechanism in case the ball hits nothing
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;
        Instantiate(explosionPrefab, position, rotation);

        if (radius == 0)
        {
            Enemy e = collision.gameObject.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
            }
        }
        else
        {
            //Aoe dmg
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider c in colliders)
            {
                Enemy e = c.GetComponent<Enemy>();
                if (e != null)
                {
                    e.TakeDamage(damage);
                }
            }
        }
        Destroy(gameObject);
    }
}
