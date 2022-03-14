using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwPickup : MonoBehaviour
{

    public PlayerController controller;
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
       
        Debug.Log("Collided");
      
        if (collision.gameObject.tag == "Player")
        {
            if (controller.isHolding == false)
            {
                    if (transform.parent.gameObject.GetComponent<throwGrave>().hasBounced == true)
                    {
                      controller.GrabGrave();
                Destroy(transform.parent.gameObject);

                    }

            }
        }
    }
}
