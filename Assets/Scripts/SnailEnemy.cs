using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailEnemy : OtherEnemy
{
    public float duration = 4f;
    private bool isSpeedIncreased = false;
    public bool isMoving = true;
    private bool hasCollided = false;

    public override void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Bomb")
        {
            Die();
            StartCoroutine(DestroyEnemy());
        }
        if (col.gameObject.tag == "Player" && !hasCollided)
        {
            hasCollided = true;

            if (transform.position.y - col.gameObject.transform.position.y <= 0.2f)
            {
                if (!isSpeedIncreased)
                {
                    isMoving = false;
                    gameObject.tag = "Untagged";
                    StartCoroutine(ChangeSpeedForDuration(0));
                }
            }
        }
        else if(col.gameObject.tag == "Player" && hasCollided)
        {
            if (!isMoving)
            {
                StartCoroutine(ChangeTag());
                StartCoroutine(ChangeSpeedForDuration(2));
            }
        }
    }
    IEnumerator ChangeSpeedForDuration(int newSpeed)
    {
        isSpeedIncreased = true;
        speed = newSpeed;
        anim.SetBool("isIncrease", true);
        yield return new WaitForSeconds(duration);
        speed = 0.5f;
        anim.SetBool("isIncrease", false);
        isSpeedIncreased = false;
        isMoving = true;
        hasCollided = false;
        gameObject.tag = "Enemy";
    }
    IEnumerator ChangeTag()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.tag = "Enemy";
    }
}
