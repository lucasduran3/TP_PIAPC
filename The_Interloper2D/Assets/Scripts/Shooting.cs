using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletTransform;
    private bool canFire;
    private float timer;
    private float timeBeforeFiring = 0.2f;

    void Update()
    {
        RotateTowards();
        Shoot();
    }

    private void RotateTowards()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePosition - transform.position;
        float angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }
    private void Shoot()
    {
        if (!canFire)
        {
            timer += Time.deltaTime;
            if(timer > timeBeforeFiring)
            {
                canFire = true;
                timer = 0;
            }
        }
        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            GameObject bullet = Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);
            bullet.GetComponent<BulletController>().owner = "player";
        }
    }
}
