using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machinegun : Tower
{
    public int pipeIndex;
    protected float projectileTravelTime;
    public Transform machinegunCylinder;
    public Transform[] machinegunPipes = new Transform[6];
    public float rotateMachinegunByAmount;

    public float fireRate;
    public float fireRateAcceleration;
    public float fireRateMaximum;

    protected override void Awake()
    {
        base.Awake();
        projectileTravelTime = 0.19f;
        fireCooldown = 0.3f;
        minRange = 0f;
        maxRange = 5f;
        goldWorth = 40;
        damage = 0.5f;
        radius = 0f;
        bulletPrefab = (GameObject)Resources.Load("Prefabs/Towers/PhysicsBullet");
        pipeIndex = 0;
        rotateMachinegunByAmount = 3.333f;

        fireRate = 0;
        fireRateAcceleration = 1f;
        fireRateMaximum = 6;
    }

    protected override void Start()
    {
        machinegunCylinder = movableTurretPart.GetChild(0);
        for(int i = 0; i < 6; i++)
        {
            machinegunPipes[i] = machinegunCylinder.GetChild(0).GetChild(i);
        }
        base.Start();
    }

    private void IncreaseFireRateBy(float _amount)
    {
        fireRate += _amount * Time.deltaTime;

        fireRate = Mathf.Clamp(fireRate,0,fireRateMaximum);
    }

    protected override void Update()
    {
        if(fireRate > 0)
        {
            machinegunCylinder.RotateAround(machinegunCylinder.forward, rotateMachinegunByAmount * fireRate * Time.deltaTime);
        }
        
        if (target == null)
        {
            IncreaseFireRateBy(-fireRateAcceleration);
            return;
        }
        IncreaseFireRateBy(fireRateAcceleration);

        if (pipeIndex > 5)
        {
            pipeIndex = 0;
        }

        Vector3 velocity = BallisticVelocity(movableTurretPart.transform.position, target.transform.position, projectileTravelTime);

        Quaternion lookAtRotation = Quaternion.LookRotation(velocity);

        movableTurretPart.rotation = Quaternion.Lerp(movableTurretPart.rotation, lookAtRotation, turningSpeed * Time.deltaTime);

        if(fireRate > 1)
        {
            fireCooldownLeft -= Time.deltaTime;

            if (fireCooldownLeft <= 0)
            {
                fireCooldownLeft = fireCooldown / fireRate;
                Shoot(velocity);
                pipeIndex++;
            }
        }
    }

    private void Shoot(Vector3 _velocity)
    {
        GameObject physicsBulletGO = (GameObject)Instantiate(bulletPrefab, machinegunPipes[pipeIndex].position, movableTurretPart.rotation, bulletsParent.transform);

        physicsBulletGO.GetComponent<Rigidbody>().velocity = _velocity;

        PhysicsBullet c = physicsBulletGO.GetComponent<PhysicsBullet>();
        c.damage = damage;
        c.radius = radius;
    }
}
