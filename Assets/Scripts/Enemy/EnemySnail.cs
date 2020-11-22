using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySnail : Enemy
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
        base.TakeDamage(amount, knockUp);
        KnockUp(knockUp);
    }
}
