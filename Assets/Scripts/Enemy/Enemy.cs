using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DropMoneyRate
{
    [Range(0, 10)]
    public int gold;
    [Range(0, 10)]
    public int blueCrystal;
    [Range(0, 10)]
    public int redCrystal;
}

[System.Serializable]
public class DropPotion
{
    [Range(1, 10)]
    public int health;

    [Range(11, 20)]
    public int mana;

    [Range(1, 30)]
    public int small;

    [Range(11, 30)]
    public int normal;
}

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public float damage;

    public float exp;

    public DropMoneyRate dropItemRate;
    public DropPotion dropPotion;

    public GameObject healthBar;

    public Rigidbody2D rb;

    public bool grounded;

    public Player player;

    public Animator anim;
    public SpriteRenderer sprite;

    public string nameEnemy;

    public bool isAlive;
    public GameObject deathPrefab;

    public float timeToReSpawn;
    public bool isBoss; //Boss không knock bay và không tái sinh
    public bool bossSpawned;
    public GameObject itemDrop;

    public bool nearPlayer;
    public float timeToAttack;

    [Header("Move")]
    public bool moving;
    public float minX;
    public float maxX;
    public float minY;
    public float moveSpeed;
    public bool isMoveLeft;

    public float tMinToMove;
    public float tMaxToMove;
    public float timeToMove;

    public float tMinStay;
    public float tMaxStay;
    public float timeStay;

    public virtual void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void CheckGround()
    {
        //Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxCheckGround.transform.position, boxCheckGround.size, 0);
        //foreach (var item in collider2Ds)
        //{
        //    if (item.gameObject.layer == 9)
        //    {
        //        grounded = true; return;
        //    }
        //}
        //grounded = false;

        if (transform.position.y <= minY)
        {
            if (transform.position.y < minY)
            {
                transform.position = new Vector3(transform.position.x, minY);
            }
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.gravityScale = 0;
            grounded = true;
        }
        else
        {
            rb.gravityScale = 3;
            grounded = false;
        }
    }

    public void Move()
    {
        if (timeStay > 0)
        {
            timeStay -= Time.deltaTime;

            anim.SetBool("Move", false);

            if (timeStay <= 0)
            {
                timeToMove = Random.Range(tMinToMove, tMaxToMove);
            }
        }

        if (timeStay <= 0)
        {
            timeToMove -= Time.deltaTime;

            anim.SetBool("Move", true);

            if (timeToMove > 0)
            {
                if (isMoveLeft)
                {
                    sprite.flipX = true;

                    rb.velocity = new Vector2(-1 * moveSpeed, rb.velocity.y);
                    if (transform.position.x <= minX)
                    {
                        isMoveLeft = false;
                    }
                }
                else
                {
                    sprite.flipX = false;

                    rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                    if (transform.position.x >= maxX)
                    {
                        isMoveLeft = true;
                    }
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                timeStay = Random.Range(tMinStay, tMaxStay);
            }
        }
    }

    public virtual void TakeDamage(float amount, bool knockUp)
    {
        timeStay = 2;
        timeToAttack = 2;
        rb.velocity = new Vector2(0, rb.velocity.y);

        anim.SetTrigger("Hit");

        Manager.manager.MakeTextDamage(transform.position, amount);
        currentHealth -= amount;
        SetFillAmountHealthBar();

        if (currentHealth <= 0)
        {
            Manager.manager.TakeExp(exp);

            Manager.manager.CheckMission(nameEnemy);

            EnemyDie();

            Manager.manager.EnemyDrop(this);

            if (itemDrop)
            {
                GameObject g = Instantiate(itemDrop, transform.position, Quaternion.identity);
                g.name = itemDrop.name;
            }

            if (isBoss)
            {
                bossSpawned = true;
            }
        }
    }

    public void KnockUp(bool b)
    {
        if (b)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector3.up * 500);
        }

        if (!grounded)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector3.up * 300);
        }


    }

    public bool NearPlayer()
    {
        nearPlayer = (Vector2.Distance(transform.position, player.transform.position) < 1f ? true : false);
        if (nearPlayer)
        {
            timeStay = 2;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        return nearPlayer;
    }

    public void AttackPlayer()
    {
        if (timeToAttack > 0)
        {
            timeToAttack -= Time.deltaTime;
        }

        if (NearPlayer())
        {
            anim.SetBool("Move", false);

            rb.velocity = new Vector2(0, rb.velocity.y);

            if (timeToAttack <= 0)
            {
                timeToAttack = 3;
                Attack();
            }
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        player.TakeHit(this);
    }

    public void SetPosHealthBar()
    {
        Vector2 p = Camera.main.WorldToScreenPoint(transform.position);
        p.y += 100;
        ReSpawn();
        healthBar.GetComponent<RectTransform>().position = p;
    }

    public void SetFillAmountHealthBar()
    {
        healthBar.transform.GetChild(1).GetComponent<Image>().fillAmount = currentHealth / maxHealth;
    }

    public void EnemyDie()
    {
        rb.velocity = Vector2.zero;

        isAlive = false;
        timeToReSpawn = 3;

        currentHealth = maxHealth;

        Destroy(healthBar.gameObject);
        sprite.enabled = false;
    }

    public void LoadToReSpawn()
    {
        timeToReSpawn -= Time.deltaTime;
        if (timeToReSpawn <= 0)
        {
            ReSpawn();
        }
    }

    public void ReSpawn()
    {
        if (isBoss)
        {
            if (bossSpawned)
            {
                return;
            }
        }

        isAlive = true;

        if(!healthBar)
        {
            healthBar = Instantiate(Manager.manager.healthBarEnemyPrefab, GameObject.Find("Canvas").transform);
        }

        SetFillAmountHealthBar();

        sprite.enabled = true;
    }
}
