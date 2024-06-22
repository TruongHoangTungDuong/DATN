using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public float speed = 2f;
    public float range;
    private float initialPosition;
    private bool movingUp = true;
    void Start()
    {
        initialPosition = transform.position.y;
    }

    private void Update()
    {
        if (movingUp)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
        if (transform.position.y >= initialPosition + range)
        {
            movingUp = false;
        }
        else if (transform.position.y <= initialPosition - range)
        {
            movingUp = true;
        }
    }
}
