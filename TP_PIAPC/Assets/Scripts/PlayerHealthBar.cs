using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : HealthBar
{
    protected override void OnDeath()
    {
        Debug.Log("Game Over");
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BulletController>().owner == "enemy")
        {
            UpdateHealth(-20f);
        }
    }
}
