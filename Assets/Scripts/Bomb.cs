using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Rigidbody2D rb;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }

    IEnumerator CancelBomb()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        StartCoroutine(CancelBomb());
    }
}
