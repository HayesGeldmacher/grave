using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwGrave : MonoBehaviour
{
    public float vertspeed;
    public float horispeed;
    public PlayerController controller;
    bool isRight = false;
    public AudioSource hitsound1;
    public AudioSource hitsound2;
    public bool hasBounced = false;
  Rigidbody2D rb;
    void Awake()
    {
         
        rb = GetComponent<Rigidbody2D>();
       
    }
    
    
  public void Throw(bool left)
    {
        if (left)
        {
        rb.velocity = new Vector2(-horispeed, vertspeed);
            isRight = false;
        }
        else
        {
            rb.velocity = new Vector2(horispeed, vertspeed);
            isRight = true;
        }
    } 
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        hitsound1.pitch += 0.05f;
        hitsound2.pitch += 0.05f;
        if (hitsound1.isPlaying)
        {
            hitsound2.Play();
            
        }
        else
        {
            hitsound1.Play();
            
        }
        
        if (collision.gameObject.tag == "Ground")
        {
          
            Instantiate(controller.PickupGrave, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        
       else if(collision.gameObject.tag == "Enemy")
        {
            hasBounced = true;

            Vector2 HitPoint = collision.contacts[0].point;

            collision.gameObject.GetComponent<enemy>().TakeDamage(controller.damage, HitPoint);
            if (isRight)
            {
                rb.velocity = new Vector2(-horispeed/2, vertspeed);
                isRight = false;
                
            }
            else
            {
                rb.velocity = new Vector2(horispeed/2, vertspeed);
                isRight = true;
            }
        }
        else
        {
            hasBounced = true;
            if (isRight)
            {
                rb.velocity = new Vector2(-horispeed / 2, vertspeed);
                isRight = false;
            }
            else
            {
                rb.velocity = new Vector2(horispeed / 2, vertspeed);
                isRight = true;
            }
        }
    }
}
