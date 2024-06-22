using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{ 
    [SerializeField]private int speed;
    [SerializeField]private int force;
    private Rigidbody2D rb;
    public bool isGrounded;
    [SerializeField] private bool isMovingLeft = false;
    [SerializeField] private bool isMovingRight = false;
    [SerializeField] private float horizontalMove;
    public LayerMask groundLayer;
    public float distance;
    public GameObject groundRayObject;
    public GameObject bulletObject;
    SpriteRenderer spriteRenderer;
    Animator anim;
    Collider2D playerCollider;
    private int point;
    private int coin;
    public int id;
    public GameObject bomb;
    public bool isClimbingWall;
    public bool isTouchingWall;

    public int Point
    {
        get { return point; }
        set { point = value; }
    }
    public void Jump()
    {
        if (isGrounded == true)
        {
            rb.AddForce(Vector3.up * force,ForceMode2D.Impulse);
            AudioManager.Instance.audioList[2].Play();
        }
    }
    public void JumpHigh()
    {
        if (isGrounded == true)
        {
            rb.AddForce(Vector3.up * force * 1.25f, ForceMode2D.Impulse);
        }
    }
    public void PointerDownLeft()
    {
        isMovingLeft = true;
    }
    public void PointerDownRight()
    {
        isMovingRight = true;
    }
    public void PointerUpRight()
    {
        isMovingRight = false;
    }
    public void PointerUpLeft()
    {
        isMovingLeft = false;
       
    }
    private void Move()
    {
        if (isClimbingWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, 3);
        }
        if (isMovingLeft)
        {
            horizontalMove = -speed;
            anim.SetBool("isRunning",true);
            spriteRenderer.flipX = true;
        }
        else if (isMovingRight)
        {
            horizontalMove = speed;
            anim.SetBool("isRunning", true);
            spriteRenderer.flipX = false;
        }
        else
        {
            horizontalMove = 0;
            anim.SetBool("isRunning", false);
        }
    }
    public void Shoot()
    {
        if (coin >= 50)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            coin -= 50;
            PlayerPrefs.SetInt("CoinCount", coin);
            PlayerPrefs.Save();
            UIManager.Instance.SetCoinText(Convert.ToString(PlayerPrefs.GetInt("CoinCount")));
            if (spriteRenderer.flipX == true)
            {
                GameObject bombObj = Instantiate(bomb, new Vector3(transform.position.x - 0.3f, transform.position.y + 0.3f, 0), Quaternion.identity);
                bombObj.GetComponent<Rigidbody2D>().velocity = new Vector2(-2, bombObj.GetComponent<Rigidbody2D>().velocity.y);
            }
            else
            {
                GameObject bombObj = Instantiate(bomb, new Vector3(transform.position.x + 0.3f, transform.position.y + 0.3f, 0), Quaternion.identity);
                bombObj.GetComponent<Rigidbody2D>().velocity = new Vector2(2, bombObj.GetComponent<Rigidbody2D>().velocity.y);
            }
        }
    }
    void Die()
    {
        UIManager.Instance.ShowGameOverPanel();
        AudioManager.Instance.audioList[0].Play();
        Time.timeScale = 0f;
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            if (transform.position.y - col.gameObject.transform.position.y < -0.2f)
            {
                Die();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Coin")
        {
            coin += 10;
            Point += 10;
            PlayerPrefs.SetInt("CoinCount", coin);
            PlayerPrefs.Save();
            UIManager.Instance.SetCoinText(Convert.ToString(PlayerPrefs.GetInt("CoinCount")));
            UIManager.Instance.SetScoreText(Convert.ToString(point));
            AudioManager.Instance.audioList[1].Play();
        }
        if (col.gameObject.tag == "BulletEnemy" || col.gameObject.tag == "Trap")
        {
            Die();
        }
    }

    void Start()
    {
        speed = 3;
        force = 10;
        Point = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim= GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        distance = 0.01f;
        coin = PlayerPrefs.GetInt("CoinCount");
        UIManager.Instance.SetCoinText(Convert.ToString(PlayerPrefs.GetInt("CoinCount")));
        if (id == PlayerPrefs.GetInt("CharacterID"))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    void Update()
    {
        Move();
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            Jump();
        }
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalMove,rb.velocity.y);
        RaycastHit2D hitGround= (Physics2D.Raycast(groundRayObject.transform.position, Vector2.down));
        Debug.DrawRay(groundRayObject.transform.position, Vector2.down*hitGround.distance, Color.yellow);
        if (hitGround.collider != null)
        {
            if (hitGround.distance < distance)
            {
                isGrounded=true;
                anim.SetBool("isJumping", false);
            }
            else
            {
                isGrounded=false;
                anim.SetBool("isJumping", true);
            }
        }
        RaycastHit2D hitWallLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.01f);
        RaycastHit2D hitWallRight = Physics2D.Raycast(transform.position, Vector2.right, 0.01f);
        isTouchingWall = hitWallLeft.collider != null || hitWallRight.collider != null;

        // Ki?m tra tr?ng thái leo t??ng
        if (isTouchingWall && /*!isGrounded &&*/ Input.GetKey(KeyCode.W))
        {
            isClimbingWall = true;
        }
        else
        {
            isClimbingWall = false;
        }
    }
}
