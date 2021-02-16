
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Tower
{
    protected float projectileTravelTime;

    protected override void Awake()
    {
        base.Awake();
        projectileTravelTime = 1.1f;
        fireCooldown = 4f;
        minRange = 2f;
        maxRange = 9f;
        goldWorth = 30;
        damage = 10f;
        radius = 1f;
        bulletPrefab = (GameObject)Resources.Load("Prefabs/Towers/CannonBall");
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 velocity = BallisticVelocity(movableTurretPart.transform.position, target.transform.position, projectileTravelTime);

        Quaternion lookAtRotation = Quaternion.LookRotation(velocity);

        movableTurretPart.rotation = Quaternion.Lerp(movableTurretPart.rotation, lookAtRotation, turningSpeed * Time.deltaTime);

        fireCooldownLeft -= Time.deltaTime;

        if (fireCooldownLeft <= 0)
        {
            fireCooldownLeft = fireCooldown;
            Shoot(velocity);
        }
    }

    private void Shoot(Vector3 _velocity)
    {
        GameObject cannonBallGO = (GameObject)Instantiate(bulletPrefab, movableTurretPart.transform.position, Quaternion.Euler(0f, 0f, 0f), bulletsParent.transform);

        cannonBallGO.GetComponent<Rigidbody>().velocity = _velocity;

        Cannonball c = cannonBallGO.GetComponent<Cannonball>();
        c.damage = damage;
        c.radius = radius;
    }
}