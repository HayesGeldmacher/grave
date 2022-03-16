using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackGrave : MonoBehaviour
{
    public PlayerController controller;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {

           
           Vector2 HitPoint = collision.contacts[0].point;
       
            GameObject currentEnemy = collision.gameObject;
            currentEnemy.GetComponent<enemy>().TakeDamage(controller.damage, HitPoint);
            
        }

    }
}
