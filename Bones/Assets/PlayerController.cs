using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    SpriteRenderer sprite;
    Rigidbody2D rb;
    public float speed;
    Animator anim;
    public float rayLength;
    public bool FacingRight = true;
    public Transform RayPoint;
    public GameObject GraveHeld;
    public bool isHolding = false;
    bool isSmashing = false;
    public BoxCollider2D box;
    public BoxCollider2D playerBox;
    public float damage;
    public GameObject ThrownGrave;
    public GameObject PickupGrave;
    public GameObject CurrentThrownGrave;
    Vector2 BoxOffset;
    Vector2 BoxSize;
    bool canJump = true;
    float lungeTimer;
    public float lungTime;

    [Header("Jump Variables")]
    public float FallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float jumpVelocity = 100;
    public bool grounded = true;
    public float JumpRayLength = 0.2f;
    public Transform throwpoint;
    public AudioSource jumpsound;

    [Header("Dash Variables")]
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private int direction;
    bool isLunging = false;

    [Header("Combat Variables")]
    public bool invincible = false;
    public float invinceTime = 0.8f;
    public AudioSource Smashsound1;
    public AudioSource Smashsound2;
    public AudioSource hurtsound;


    public float Health;
    public float MaxGraveDist;
    public ParticleSystem feetdust;
    public float dustSize;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        BoxOffset = new Vector2(playerBox.offset.x, playerBox.offset.y);
        BoxSize = playerBox.size;
        dashTime = -1;
        lungeTimer = 0;
        
    }


    

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        lungeTimer -= Time.deltaTime;



        RaycastHit2D hit = Physics2D.Raycast(RayPoint.position, -Vector2.up * JumpRayLength);
       if(hit.collider != null)
        {
        if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {          
                grounded = true;
                canJump = true;
                anim.SetBool("isJumping", false);
        }
            else
            {
                grounded = false;
                anim.SetBool("isJumping", true);
            }

        }
        else
        {
            grounded = false;
            anim.SetBool("isJumping", true);
        }


        if (invincible == false)
        {

            if (dashTime < 0)
            {
                anim.SetBool("isLunging", false);
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

            playerBox.size = BoxSize;
            playerBox.offset = BoxOffset;

            }
            else
                {
               
                
                dashTime -= Time.deltaTime;
              
                if (FacingRight)
                {
                rb.velocity = new Vector2(1, 0.15f) * dashSpeed;
              
               
                }
                else
                {
                    rb.velocity = new Vector2(-1, 0.15f) * dashSpeed;
                }

            
                }
        
        }
        if( rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime; 
        }
       else if(rb.velocity.y > 0 && !Input.GetButton("Fire1"))
        {
           
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //raycast distance from child object and player, if too far rematerialize grave in hand
        if (!isHolding)
        {
            //figure out where grave child is'
            GameObject rayGrave = GameObject.FindGameObjectWithTag("Grave");
            
            Vector2 gravedist =  rayGrave.transform.position - transform.position;
            
            float distance = Mathf.Abs(gravedist.x) + Mathf.Abs(gravedist.y);
            if(distance > MaxGraveDist)
            {
               
                Debug.Log("grave rematerialized");
                GrabGrave();
                Destroy(rayGrave);
                if (CurrentThrownGrave)
                {
                Destroy(CurrentThrownGrave);

                }
            }


        }
        
        if (isSmashing)
        {
            box.enabled = true;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            box.enabled = false;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        AudioSource walk = GetComponent<AudioSource>();
        if(Mathf.Abs(horizontal) > 0.1)
        {
          if(grounded == true)
            {
                //plays particle system and sound when walking
                walk.UnPause();
                feetdust.Play();
                
            }
            else
            {
                walk.Pause();
                feetdust.Pause();
                feetdust.Clear();
                
            }

            anim.SetBool("isRunning", true);
            if(horizontal > 0.1f)
            {
                //sprite.flipX = false;
                if (!FacingRight)
                {
                Flip();
                }

            }
            if(horizontal < -0.1f)
            {
                //sprite.flipX = true;
                if (FacingRight)
                {
                Flip();

                }
            }
            
        }
        else
        {
           // disables particle system, run animation, and sound effect when not walking
            anim.SetBool("isRunning", false);
            walk.Pause();
            feetdust.Pause();
            feetdust.Clear();

        }

        if (Input.GetButtonDown("Fire1") )
        {

            if (isHolding)
            {
            StartCoroutine(Smash(0.5f));
            }
            else
            {
               if(grounded)
                rb.velocity = Vector2.up * jumpVelocity;
            }
        }

        if (Input.GetButtonDown("Fire3"))
        {
            if (isHolding)
            {
            ThrowGrave();
            }
            else
            {
                if (canJump && lungeTimer < 0)
                {
                anim.SetBool("isLunging", true);
                playerBox.size = new Vector2(0.2404453f, 0.24274f);
                playerBox.offset = new Vector2(playerBox.offset.x, -0.19863f);
                DodgeRoll();
                    canJump = false;
                    lungeTimer = lungTime;
                }
            }
        }
        

        if(Health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("player has fucking died");
        }

    }

    public void Heal(float HealAmount)
    {
        //Heal the player
       if((Health + HealAmount) < 100)
        {

        Health += HealAmount;
        }
        else
        {
            Health = 100;
        }
        // add feedback
    }


   public IEnumerator Smash(float time)
    {
        Debug.Log("SMash");
        anim.SetTrigger("smash");
        if(Smashsound1.isPlaying == false)
        {
            
            Smashsound1.Play();
        }
        else
        {
            Smashsound2.Play();
        }
        isSmashing = true;
        yield return new WaitForSeconds(time);
        isSmashing = false;
    }

   IEnumerator HurtState(float time)
    {
        StartCoroutine(ColorFlash(0.1f));
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
        //make the sprite flash during this duration

    }
    IEnumerator ColorFlash(float time)
    {
        Color color = sprite.color;
      for(int i = 0; i < 3; i++)
        {
        color.a = 0f;
        sprite.color = color;
            yield return new WaitForSeconds(time);
        color.a = 100f;
        sprite.color = color;
            yield return new WaitForSeconds(time);

        }


    }
    void DodgeRoll()
    {
        dashTime = startDashTime;
        Debug.Log("Dodged");
        anim.SetBool("isLunging", true);
        isLunging = true;

      
     
    }
    
    void ThrowGrave()
    {
        isHolding = false;
        GraveHeld.SetActive(false);
        anim.SetTrigger("Throw");
        anim.SetBool("isHolding", false);
       CurrentThrownGrave = Instantiate(ThrownGrave, new Vector2(throwpoint.transform.position.x, throwpoint.transform.position.y), Quaternion.identity);
        CurrentThrownGrave.GetComponent<throwGrave>().controller = this;
        CurrentThrownGrave.transform.GetChild(1).GetComponent<throwPickup>().controller = this;
        CurrentThrownGrave.GetComponent<throwGrave>().Throw(!FacingRight);
    }
    
    void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void GrabGrave()
    {
        isHolding = true;
        GraveHeld.SetActive(true);
        anim.SetBool("isHolding", true);
    }

    public void TakeDamage(float damage, Vector2 direction)
    {
        Health -= damage;
        Debug.Log("Player has taken damage");
        StartCoroutine(HurtState(invinceTime));
        rb.velocity = (Vector2.up * 5) + direction * -3;
        hurtsound.Play();
        if (isHolding)
        {
            ThrowGrave();
        }
        anim.SetTrigger("damaged");
        
        //fix bug where dash rolling into enemy and getting hurt briefly makes physics weird
        //make sprite flash with damage
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Grave")
        {
            if (!isHolding)
            {
            GrabGrave();
            Destroy(collision.transform.parent.gameObject);

            }
        }

        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (!isSmashing && !invincible)
            {
            Vector2 enemydirection = collision.gameObject.transform.position - transform.position; 
            TakeDamage(10, enemydirection);

            }
                
        }
    }
}
