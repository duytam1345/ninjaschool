using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public Vector2 dir;
    public float speed;
    public Rigidbody2D rb;
    public Animator anim;

    void Start()
    {
        rb.velocity = dir * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            anim.speed = 0;
            rb.velocity = Vector2.zero;
            Destroy(gameObject, 1);
        }
        if (collision.gameObject.layer == 10)
        {
            anim.speed = 0;
            collision.GetComponent<Enemy>().TakeDamage(1, false);
            rb.velocity = Vector2.zero;
            Destroy(gameObject, 1);
        }
    }
}
