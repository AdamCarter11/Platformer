using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float m_FallSpeed = 0f;
    //inspector variables
    [SerializeField] private float speed, jumpForce, checkRadius;
    [SerializeField] private int resetJumps, health;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] ParticleSystem ps;
    [SerializeField] Transform frontCheck;
    [SerializeField] private float wallSlidingSpeed;

    //hidden variables
    private float moveInput;
    private bool isGrounded;
    private int extraJumps;
    private bool facingRight = false;
    private Rigidbody2D rb = null;
    private bool isTouchingFront;
    private bool wallSliding;
    private float ySpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        extraJumps = resetJumps;
    }
  
    void Update()
    {
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
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround);

        if(isGrounded){
            extraJumps = resetJumps;
        }
        if(Input.GetKeyDown(KeyCode.Space) && extraJumps > 0){
            rb.velocity = Vector2.up * jumpForce;
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

        if(Input.GetKey(KeyCode.S)){
            rb.gravityScale = 7f;
        }
        else{
            rb.gravityScale = 3f;
        }
        
        if(isTouchingFront && isGrounded == false && moveInput != 0){
            wallSliding = true;
        }else{
            wallSliding = false;
        }

        if(wallSliding){
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        else{
            
        }

        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
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
    }
}
