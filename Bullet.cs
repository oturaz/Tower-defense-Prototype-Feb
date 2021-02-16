using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1f;
    public Transform target;
    public float damage = 1f;
    public float radius = 0f;

    void Awake()
    {
        speed = 7;
    }

    
    void Update()
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - this.transform.localPosition;

        float distThisFrame = speed * Time.deltaTime;

        if (target != null)
        {
            if (dir.magnitude <= distThisFrame)
            {
                //reached target
                DoBulletHit();
            }
            else
            {
                //move towards target
                transform.Translate(transform.forward * distThisFrame, Space.World);
                this.transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }

    protected virtual void DoBulletHit()
    {
        if (radius == 0)
        {
            target.GetComponent<Enemy>().TakeDamage(damage);
        }
        else
        {
            //AoE dmg
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
