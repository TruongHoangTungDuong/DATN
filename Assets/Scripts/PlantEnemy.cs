using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlantEnemy : Enemy
{
    public GameObject bullet;
    private int speed;
    private List<GameObject> bulletPool;
    private int poolSize=10;
    SpriteRenderer spriteRenderer;
    public Vector3 bulletSpawnPoint;
    //public GameObject player;
    void InitializeObjectPool()
    {
        bulletPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bullet);
            obj.SetActive(false);
            bulletPool.Add(obj);
        }
    }
    GameObject GetBulletFromPool()
    {
        foreach (GameObject obj in bulletPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        GameObject newObj = Instantiate(bullet);
        newObj.SetActive(false);
        bulletPool.Add(newObj);
        return newObj;
    }
    void Shoot()
    {
        GameObject bulletObj = GetBulletFromPool();
        bulletObj.transform.position = bulletSpawnPoint;
        bulletObj.SetActive(true);
        //player = GameObject.FindGameObjectWithTag("Player");
        //for (int i = 0; i < player.Length;i++) 
        //{

        if (player/*[i]*/.activeInHierarchy)
        {
            if (player/*[i]*/.transform.position.x <= transform.position.x)
                {
                    bulletObj.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, bulletObj.GetComponent<Rigidbody2D>().velocity.y);
                    spriteRenderer.flipX = false;
                }
                else
                {
                    bulletObj.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, bulletObj.GetComponent<Rigidbody2D>().velocity.y);
                    spriteRenderer.flipX = true;
                }
            }
        //}
    }
    void Start()
    {
        InitializeObjectPool();
        anim = GetComponent<Animator>();
        bulletSpawnPoint = new Vector3(transform.position.x+0.05f, transform.position.y-0.2f, transform.position.z);
        spriteRenderer = GetComponent<SpriteRenderer>();
        speed = 1;
        enemyCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        }

    // Update is called once per frame
    void Update()
    {
        //for(int i=0; i < player.Length; i++)
        //{
            if (player/*[i]*/.activeInHierarchy)
            {
                if (Vector3.Distance(player/*[i]*/.transform.position, transform.position) < 2)
                {
                    Debug.Log("shoot");
                    anim.SetBool("isAttacking", true);
                }
                else
                {
                    anim.SetBool("isAttacking", false);
                }
            }
        //}
    }
}
