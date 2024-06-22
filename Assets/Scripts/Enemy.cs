using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D enemyCollider;
    public Animator anim;
    public GameObject player;
    public GameObject[] players;

    public void Die()
    {
        rb.AddForce(new Vector2(0, 100f));
        enemyCollider.enabled = false;
        anim.SetBool("isDeath", true);
        //for (int i=0; i < player.Length; i++)
        //{
            if (player/*[i]*/.activeInHierarchy)
            {
                player/*[i]*/.GetComponent<Player>().Point += 10;
                UIManager.Instance.SetScoreText(Convert.ToString(player/*[i]*/.GetComponent<Player>().Point));
            }
        //}
    }
    private void OnEnable()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerIn in players)
        {
            if (playerIn.activeSelf)
            {
                player = playerIn;
            }
        }
    }
    public IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    public virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (transform.position.y - col.gameObject.transform.position.y <= 0.2f)
            {
                Die();
                StartCoroutine(DestroyEnemy());
            }
        }
        if(col.gameObject.tag == "Bomb")
        {
            Die();
            StartCoroutine(DestroyEnemy());
        }
    }
}
