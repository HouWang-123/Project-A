using System;
using UnityEngine;
using DG.Tweening;

public class BulletControl : MonoBehaviour
{
    public float Speed = 10;


    private void FixedUpdate()
    {
        transform.position += transform.right * Speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
