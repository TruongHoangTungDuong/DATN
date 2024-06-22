using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherEnemy : Enemy
{
    public float speed;
    public float range;
    private float initialPosition;
    private bool movingRight = false;
    [SerializeField] SpriteRenderer spriteRenderer;

    void Start()
    {
        initialPosition = transform.position.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb=GetComponent<Rigidbody2D>();
        enemyCollider=GetComponent<Collider2D>();
        anim=GetComponent<Animator>();
    }

    void Update()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        if (transform.position.x > initialPosition + range)
        {
            movingRight = false;
            spriteRenderer.flipX = false;
        }
        else if (transform.position.x <= initialPosition - range)
        {
            movingRight = true;
            spriteRenderer.flipX = true;
        }
    }
}
