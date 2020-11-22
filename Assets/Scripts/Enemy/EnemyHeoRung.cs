using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeoRung : Enemy
{

    public BoxCollider2D boxCheckPlayer;

    public float timeCooldownAttack;
    public bool attacking;

    public Vector2 vToAttack;

    private void Update()
    {
        if (!isAlive)
        {
            return;
        }

        CheckPlayer();

        if (attacking)
        {
            if (timeCooldownAttack <= 0)
            {
                anim.SetBool("Attacking", true);
            }
            else
            {
                anim.SetBool("Attacking", false);
            }
        }
        else
        {
            anim.SetBool("Attacking", false);
        }

        if (attacking)
        {
            Attack();
        }
        else
        {
            NewMove();
        }

        CheckGround();
        SetPosHealthBar();
    }

    void NewMove()
    {
        rb.velocity = Vector2.zero;
    }

    void CheckPlayer()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxCheckPlayer.transform.position, boxCheckPlayer.size, 0);
        foreach (var item in collider2Ds)
        {
            if (item.gameObject.layer == 8)
            {
                vToAttack = item.transform.position;
                attacking = true;
                return;
            }
        }
        vToAttack = Vector2.zero;
        attacking = false;
    }

    void Attack()
    {
        if (timeCooldownAttack > 0)
        {
            timeCooldownAttack -= Time.deltaTime;
            rb.velocity = Vector2.zero;
            return;
        }

        if (vToAttack != Vector2.zero && timeCooldownAttack <= 0)
        {
            rb.velocity = new Vector2((vToAttack.x > transform.position.x ? moveSpeed * 1.5f : -moveSpeed * 1.5f), 0);

            sprite.flipX = vToAttack.x > transform.position.x ? false : true;

            if (Vector2.Distance(transform.position, vToAttack) <= 1)
            {
                player.TakeHit(this);
                timeCooldownAttack = 2;
            }
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
