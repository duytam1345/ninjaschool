using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMucKhongLo : Enemy
{
    public BoxCollider2D boxCheckPlayer;

    public GameObject bullet;
    public float timeCooldownAttack;

    public Vector2 vToMove;

    private void Update()
    {
        if (!isAlive)
        {
            return;
        }

        Attack();

        SetPosHealthBar();
        CheckPlayer();
        NewMove();
    }

    void NewMove()
    {
        if (vToMove != Vector2.zero && Vector2.Distance(transform.position, vToMove) > .5f)
        {
            moving = true;
            rb.velocity = new Vector2((transform.position.x < vToMove.x ? 1 : -1) * moveSpeed, 0);
        }
        else
        {
            moving = false;
            rb.velocity = Vector2.zero;
            vToMove = Vector2.zero;
        }
    }

    void CheckPlayer()
    {
        if (moving)
        {
            return;
        }

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxCheckPlayer.transform.position, boxCheckPlayer.size, 0);
        foreach (var item in collider2Ds)
        {
            if (item.gameObject.layer == 8)
            {
                if (Vector2.Distance(transform.position, item.transform.position) <= 2)
                {
                    vToMove = new Vector2(Random.Range(minX, maxX), transform.position.y);
                }
            }
        }
    }

    void Attack()
    {
        if (moving)
        {
            return;
        }

        timeCooldownAttack -= Time.deltaTime;
        if (timeCooldownAttack <= 0)
        {
            GameObject b = Instantiate(bullet, transform.position, Quaternion.identity);
            b.GetComponent<BulletOfEnemy>().dir = new Vector2(player.transform.position.x > transform.position.x ? 1 : -1, 0);
            timeCooldownAttack = Random.Range(.25f, 1.5f);
        }
    }

    public override void TakeDamage(float amount, bool knockUp)
    {
        if (isAlive)
        {
            base.TakeDamage(amount, knockUp);
        }
    }
}
