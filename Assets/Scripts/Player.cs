using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float m_FallSpeed = 0f;
    //inspector variables
    [SerializeField] private float speed, jumpForce, checkRadius;
    [SerializeField] private int resetJumps, health, resetDashes;
    [SerializeField] Transform groundCheck, teleportCheck;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] ParticleSystem ps;
    [SerializeField] Transform frontCheck;
    [SerializeField] private float wallJumpTime, dashDistance = 15f;
    [SerializeField] private float dashTime;
    [SerializeField] private int playerHealth;
    [SerializeField] private Text playerHealthText;

    //hidden variables
    private float moveInput;
    private bool isGrounded;
    private int extraJumps, extraDashes;
    private bool facingRight = false;
    private Rigidbody2D rb = null;
    private bool isTouchingFront;
    private bool wallSliding;
    private float ySpeed, wallJumpCounter;
    private bool isDashing;
    private float doubleTapTime;
    private KeyCode lastKeyCode;
    private bool canTeleport;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        extraJumps = resetJumps;
        extraDashes = resetDashes;
        playerHealthText.text = "Player health: " + playerHealth;
    }
  
    void Update()
    {
        if(wallJumpCounter <= 0 && isDashing == false){
            if(Input.GetKey(KeyCode.LeftShift)){
                StartGliding();
            }

            if (IsGliding && rb.velocity.y < 0f && Mathf.Abs(rb.velocity.y) > m_FallSpeed){
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.y) * m_FallSpeed);
            }
            else{
                StopGliding();
            }

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
            canTeleport = !Physics2D.OverlapCircle(teleportCheck.position, checkRadius);
            isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround);

            if(Input.GetKeyDown(KeyCode.T) && canTeleport && !GrapplingHook.isGrappled){
                transform.position = teleportCheck.position;
            }

            if(isGrounded){
                extraJumps = resetJumps;
                extraDashes = resetDashes;
            }
            if(Input.GetKeyDown(KeyCode.Space) && extraJumps > 0){
                rb.velocity = Vector2.up * jumpForce;
                wallSliding = false;
                extraJumps--;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded){
                rb.velocity = Vector2.up * jumpForce;
            }

            if(facingRight && moveInput > 0){
                Flip();
            }
            else if(facingRight == false && moveInput < 0){
                Flip();
            }

            moveInput = Input.GetAxis("Horizontal");

            if(Input.GetKey(KeyCode.S) && wallSliding == false){
                rb.gravityScale = 7f;
            }
            else if(wallSliding == false){
                rb.gravityScale = 3f;
            }
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);


            wallSliding = false;
            if(isTouchingFront && !isGrounded){
                if((transform.localScale.x > 0 && Input.GetAxisRaw("Horizontal") > 0) || (transform.localScale.x < 0 && Input.GetAxisRaw("Horizontal") < 0)){
                    wallSliding = true;
                    extraJumps = resetJumps;
                    extraDashes = resetDashes;
                }
            }

            if(wallSliding){
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                if(Input.GetKeyDown(KeyCode.Space)){
                    wallJumpCounter = wallJumpTime;
                    rb.velocity = new Vector2(-Input.GetAxisRaw("Horizontal") * speed, jumpForce);
                    rb.gravityScale = 3f;
                    wallSliding = false;
                }
            }

            //dashing code
            if(Input.GetKeyDown(KeyCode.A)){
                if(doubleTapTime > Time.time && lastKeyCode == KeyCode.A){
                    if(!isGrounded){
                        extraDashes--;
                    }
                    if(extraDashes > 0){
                        StartCoroutine(Dash(-1f));
                    }
                    
                }
                else{
                    doubleTapTime = Time.time + dashTime;   //0.5f is the time you can dash in
                }
                lastKeyCode = KeyCode.A;
            }
            if(Input.GetKeyDown(KeyCode.D)){
                if(doubleTapTime > Time.time && lastKeyCode == KeyCode.D){
                    if(!isGrounded){
                        extraDashes--;
                    }
                    if(extraDashes > 0){
                        StartCoroutine(Dash(1f));
                    }
                    
                }
                else{
                    doubleTapTime = Time.time + 0.5f;   //0.5f is the time you can dash in
                }
                lastKeyCode = KeyCode.D;
            }
        }
        else{
            wallJumpCounter-=Time.deltaTime;
        }
        

        if(transform.position.y < -10){
            transform.position = new Vector2(0,-3.5f);
        }
    }
    
    IEnumerator Dash(float dir){
        isDashing = true;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(dashDistance * dir, 0f), ForceMode2D.Impulse);
        float gravity = rb.gravityScale;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(.3f);   //dash time
        isDashing = false;
        rb.gravityScale = gravity;
    }

    void Flip(){
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void StartGliding()
    {
        IsGliding = true;
    }
  
    public void StopGliding()
    {
        IsGliding = false;
    }

    public bool IsGliding { get; set; } = false;

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("ground")){
            Vector3 psVec = new Vector3(transform.position.x, transform.position.y-.5f, transform.position.z);
            ParticleSystem spawnedPs = Instantiate(ps, psVec, Quaternion.identity);
        }

        //head bounce
        if(other.gameObject.CompareTag("enemy") && Input.GetKey(KeyCode.S)){
            rb.velocity = Vector2.up * jumpForce;
        }
        //enemy deals damage
        else if(other.gameObject.CompareTag("enemy")){
            playerHealth--;
            playerHealthText.text = "Player health: " + playerHealth;
            if(playerHealth == 0){
                print("gameover");
            }
        }

    }
}
