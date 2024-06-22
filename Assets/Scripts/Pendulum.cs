using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Pendulum : MonoBehaviour
{
    public float rotationDuration = 1f;
    public Vector3 targetRotation = new Vector3(0f, 0f, 360f);

    void Start()
    {
        transform.DORotate(targetRotation, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
}
