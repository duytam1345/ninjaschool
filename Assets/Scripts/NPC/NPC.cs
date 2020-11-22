using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea]
    public string[] sentences;
}

public class NPC : MonoBehaviour
{
    [Header("Dialogue")]
    public Dialogue[] dialogue;

    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sprite;

    public bool isTalking;

    [Header("Move")]
    public bool moving;
    public float moveSpeed;

    public float minX;
    public float maxX;

    public float minTMove;
    public float maxTMove;
    public float tToMove;

    public float minTStay;
    public float maxTStay;
    public float tToStay;

    public bool isMoveLeft;

    public void Move()
    {
        if(isTalking)
        {
            return;
        }

        if (tToStay > 0)
        {
            tToStay -= Time.deltaTime;

            if (tToStay <= 0)
            {
                tToMove = Random.Range(minTMove,maxTMove);
                moving = true;
                anim.SetBool("Move", true);
            }
        }
        else
        {
            if (tToMove > 0)
            {
                tToMove -= Time.deltaTime;

                if (isMoveLeft)
                {
                    rb.velocity = Vector2.left * moveSpeed;

                    if (transform.position.x <= minX)
                    {
                        isMoveLeft = false;
                        sprite.flipX = false;
                    }
                }
                else
                {
                    rb.velocity = Vector2.right * moveSpeed;

                    if (transform.position.x >= maxX)
                    {
                        isMoveLeft = true;
                        sprite.flipX = true;
                    }
                }
            }
            else
            {
                tToStay = Random.Range(minTStay, maxTStay);
                rb.velocity = Vector2.zero;
                moving = false;
                anim.SetBool("Move", false);
            }
        }
    }

    public virtual void TriggerDialogue(){} 
}
