using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonHeadXichMao : MonoBehaviour
{
    public Rigidbody2D rb;
    private void Start()
    {
        rb.velocity = transform.right*3;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10)
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(1, false);
        }
    }
}
