using UnityEngine;
using System.Collections;
using System;

public class Tower : MonoBehaviour {

    [Header("Attack")]
    public float damage;
    public float attackSpeed;
    private float nextAttack = 0f;
    public GameObject bullet;
    public float bulletSpeed = 5;
    public float range = 5;

    GameObject target;

    [HideInInspector]
    public GameObject hexagonPlatform;

    void Update()
    {
        GetClosestTarget();

        Shooting();
    }

    void Shooting()
    {
        if (target == null) return;

        transform.LookAt(target.transform.position);

        if (Time.time > nextAttack && Vector3.Distance(transform.position, target.transform.position) <= range)
        {
            nextAttack = Time.time + attackSpeed;

            GameObject bul = Instantiate(bullet, transform.position, Quaternion.identity);

            //Eher bullet following script
            bul.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        }
    }

    void GetClosestTarget()
    {
        float maxDist = float.MaxValue;

        foreach (GameObject unit in TowerManager.units)
        {
            float dist = Vector3.Distance(unit.transform.position, transform.position);
            if (dist < maxDist)
            {
                maxDist = dist;
                target = unit;
            }
        }
    }
}
