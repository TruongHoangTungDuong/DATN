using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    Rigidbody2D rb;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }
    IEnumerator CancelBullet()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        StartCoroutine(CancelBullet());
    }
}
