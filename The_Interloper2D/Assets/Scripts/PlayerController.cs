using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 15f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Walk();
    }
    private void Walk()
    {
        float translationX = Input.GetAxis("Horizontal");
        float translationY = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(translationX, translationY ) * speed;
        
    }

}
