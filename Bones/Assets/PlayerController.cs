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

    [Header("Dash Variables")]
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private int direction;
    bool isLunging = false;

    public float Health;

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
        
        if( rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime; 
        }
       else if(rb.velocity.y > 0 && !Input.GetButton("Fire1"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
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
        
        if(Mathf.Abs(horizontal) > 0.1)
        {
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
            anim.SetBool("isRunning", false);
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

   public IEnumerator Smash(float time)
    {
        Debug.Log("SMash");
        anim.SetTrigger("smash");
        isSmashing = true;
        yield return new WaitForSeconds(time);
        isSmashing = false;
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

    public void TakeDamage(float damage)
    {
        Health -= damage;
        Debug.Log("Player has taken damage");
        if (isHolding)
        {
            ThrowGrave();
        }
        anim.SetTrigger("damaged");
        
        //raycast to enemy position, shoot the player in the opposite direction
        //drop the grave if holding it, just do throwgrave logic
        //dont allow character input and also make invincible for a brief moment
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
            if(!isSmashing)
            TakeDamage(10);
        }
    }
}
