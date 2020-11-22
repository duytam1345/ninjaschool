using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTho : Enemy
{
    private void Update()
    {
        if (!isAlive)
        {
            return;
        }
        if (!NearPlayer())
        {
            Move();
        }
        AttackPlayer();
        CheckGround();
        SetPosHealthBar();
    }

    public override void TakeDamage(float amount, bool knockUp)
    {
        if (isAlive)
        {
            base.TakeDamage(amount, knockUp);
            KnockUp(knockUp);
        }
    }
}
