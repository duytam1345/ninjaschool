using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuNhin : Enemy
{
    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        CheckGround();
        SetPosHealthBar();
    }

    void SetPosHealthBar()
    {
        if (healthBar)
        {
            Vector2 p = Camera.main.WorldToScreenPoint(transform.position);
            p.y += 100;
            healthBar.GetComponent<RectTransform>().position = p;
        }
    }

    public override void TakeDamage(float amount, bool knockUp)
    {
        if (player.transform.position.x > transform.position.x)
        {
            anim.SetTrigger("Left");
            Manager.manager.MakeEffectHitBlood(transform.position, true);
        }
        else
        {
            anim.SetTrigger("Right");
            Manager.manager.MakeEffectHitBlood(transform.position, false);
        }

        Manager.manager.MakeTextDamage(transform.position, amount);
        currentHealth -= amount;
        healthBar.transform.GetChild(1).GetComponent<Image>().fillAmount = currentHealth / maxHealth;

        if (knockUp)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector3.up * 500);
        }

        if (!grounded)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector3.up * 300);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //Manager.manager.CheckMission("Bù nhìn");

        //EnemyDie();

        //GameObject g = Instantiate(deathPrefab, transform.position, Quaternion.identity);
        //g.GetComponent<Rigidbody2D>().AddForce(
        //    (player.transform.position.x > transform.position.x ? Vector2.left : Vector2.right) * 300);

        //Destroy(g, 3f);

        //Manager.manager.EnemyDrop(this);
        //Manager.manager.RespawnEnemy(this);

        //Destroy(healthBar.gameObject);
        //Destroy(gameObject);
    }
}
