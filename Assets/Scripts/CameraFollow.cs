using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject[] player;
    public float positionY;

    void SetCameraPosition()
    {
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].activeInHierarchy)
            {
                transform.position = new Vector3(player[i].GetComponent<Transform>().transform.position.x,
                                                 player[i].GetComponent<Transform>().transform.position.y, -10);
            }
        }
    }
    void Update()
    {
        SetCameraPosition();
    }
}

