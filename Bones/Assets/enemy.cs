using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{

    public float health;
    public GameObject hitEffect;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Attack();
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        GameObject Particles = transform.GetChild(0).gameObject;
        ParticleSystem Bloodwind = Particles.GetComponent<ParticleSystem>();
        Bloodwind.Play();
        Particles.transform.parent = null;
        Destroy(gameObject);
    }

    public void TakeDamage(float damage, Vector3 pos)
    {
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("Hurt");
        health -= damage;
        
        Instantiate(hitEffect, pos, Quaternion.identity);
    }

    void Attack()
    {
        //This is where motherfucker attacks
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("Attack");
        
    }
}
