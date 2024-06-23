using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float force = 15f;
    public string owner;
    private EnemyController _enemyController;
    void Start()
    {
        if(owner == "player")
        {
            rb = GetComponent<Rigidbody2D>();
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            Vector3 rotation = transform.position - mousePosition;

            //normalizar para que independientemente de la distancia del puntero del jugador la bala no cambie de velocidad
            rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        } 
        else
        {
            rb = GetComponent<Rigidbody2D>();
            Vector3 direction = (enemyController.target.position - enemyController.rotatePoint.position).normalized;
            Vector3 rotation = transform.position - enemyController.target.position;
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 90);
            rb.velocity = direction * force;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag != "bullet")
        {
            Destroy(gameObject);
        }
    }


    //Properties
    public EnemyController enemyController
    {
        get { return _enemyController; }
        set { _enemyController = value; }
    }
}
