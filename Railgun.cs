using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : Tower
{
    protected override void Awake()
    {

        base.Awake();

        fireCooldown = 2f;
        damage = 10;
        maxRange = 6f;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void ShootAt(Transform e)
    {
        base.ShootAt(e);
    }
}
