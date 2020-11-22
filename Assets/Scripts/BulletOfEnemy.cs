using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOfEnemy : MonoBehaviour
{
    public bool changed;

    public Vector2 dir;

    public float damage;

    public float speed;

    public Rigidbody2D rb;

    public SpriteRenderer sprite;

    private void Start()
    {
        CheckFlip();
        Destroy(gameObject, 3f);
    }

    void CheckFlip()
    {
        if (dir.x < 0)
        {
            sprite.flipX = true;
        }
        else if (dir.x > 0)
        {
            sprite.flipX = false;
        }
    }

    private void Update()
    {
        rb.velocity = dir * speed;
    }

    void Change()
    {
        if (!changed)
        {
            dir = new Vector2(dir.x * -1, dir.y);
            changed = true;
            CheckFlip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            Change();
        }
        if (collision.gameObject.layer == 8)
        {
            FindObjectOfType<Player>().TakeHit(transform,damage);
            Destroy(gameObject);
        }
    }
}
