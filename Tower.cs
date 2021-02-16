using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform target;
    protected Transform movableTurretPart;

    protected float maxRange;
    protected float minRange;
    protected float turningSpeed;
    protected float fireCooldown;
    protected float fireCooldownLeft;
    protected float damage;
    protected float radius;

    protected GameObject bulletPrefab;

    protected int goldWorth;
    protected GameObject bulletsParent;

    public int GoldWorth
    {
        get
        {
            return goldWorth;
        }
    }

    public float MaxRange
    {
        get
        {
            return maxRange;
        }
    }

    public float MinRange
    {
        get
        {
            return minRange;
        }
    }

    protected virtual void Awake()
    {
        maxRange = 3f;
        minRange = 0f;
        turningSpeed = 10f;
        fireCooldown = 0.5f;
        fireCooldownLeft = 0f;
        damage = 1f;
        radius = 0f;
        goldWorth = 10;
        bulletPrefab = (GameObject)Resources.Load("Prefabs/Towers/Bullet");

        movableTurretPart = transform.Find("MovableTurretPart");

        if(GameObject.Find("bulletsParent") != null)
        {
            bulletsParent = GameObject.Find("bulletsParent");
        }
        else
        {
            bulletsParent = new GameObject("bulletsParent");
        }
    }
    protected virtual void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy e in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, e.transform.position);

            //tries to find shorter distance, but only if it is equal or higher than the minimum range of the tower
            if (distanceToEnemy < shortestDistance && distanceToEnemy >= minRange) 
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = e;
            }
        }

        //checks if closest detected enemy is within the max range
        if (nearestEnemy != null && shortestDistance <= maxRange)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    protected virtual void Update()
    {
        if(target == null)
        {
            return;
        }

        Vector3 dir = target.position - movableTurretPart.transform.position;
        Quaternion lookAtRotation = Quaternion.LookRotation(dir);
        movableTurretPart.rotation = Quaternion.Lerp(movableTurretPart.rotation, lookAtRotation, turningSpeed * Time.deltaTime);

        fireCooldownLeft -= Time.deltaTime;
        if(fireCooldownLeft <= 0)
        {
            fireCooldownLeft = fireCooldown;
            ShootAt(target);
        }
    }

    protected virtual void ShootAt(Transform e)
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, movableTurretPart.GetChild(0).GetChild(0).transform.position, movableTurretPart.rotation, bulletsParent.transform);

        Bullet b = bulletGO.GetComponent<Bullet>();
        b.target = e.transform;
        b.radius = radius;
        b.damage = damage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }

    /// <summary>
    /// Calculate a ballistic trajecory given an origin, target, and time to target which basically just is the arc
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="timeToTarget"></param>
    /// <returns>Vector3</returns>
    protected Vector3 BallisticVelocity(Vector3 origin, Vector3 target, float timeToTarget)
    {
        Vector3 toTarget = target - origin;
        float y = toTarget.y;
        toTarget.y = 0;

        // calculate starting speeds for xz and y. Physics forumula is deltaX = v0 * t + 1/2 * a * t * t
        // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
        // so xz = v0xz * t => v0xz = xz / t
        // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t

        // create result vector for calculated starting speeds
        Vector3 result = toTarget.normalized * toTarget.magnitude / timeToTarget; //v0xz                  
        result.y = y / timeToTarget + 0.5f * Physics.gravity.magnitude * timeToTarget; //v0y                           

        return result;
    }
}