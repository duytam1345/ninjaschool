using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDamageEnemy : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            player.DamageEnemy(collision.gameObject);
        }
    }
}
