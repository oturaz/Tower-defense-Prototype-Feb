using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBullet : MonoBehaviour
{
    public float timeLeft;
    public float damage = 0f;
    public float radius = 0f;
    public bool collided;

    public void Awake()
    {
        timeLeft = 5f;
        collided = false;
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
        if(!collided)
        {
            collided = true;
            ContactPoint contact = collision.GetContact(0);

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
        }
        
    }
}
