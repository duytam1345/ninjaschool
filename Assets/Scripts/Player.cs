using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum NameWeapon
    {
        KiemTre,
        LoiKiem,
        HoaLongKiem,
        HanBangKiem,
        PhiTieu,
        XichMaoKiem,
    }

    public enum State
    {
        Idle,
        Sit,
        Move,
        Jump,
        Fall,
        Slash1,
        Slash2,
        Slash3,
        Slash1Fall,
        Slash2Fall,
    }

    public State state;

    public NPC npcToTalk;
    public BoxCollider2D boxCheck;

    [Header("Hiệu ứng chém")]
    public GameObject effectLoiKiemPrefab;
    public GameObject effectHanBangKiemPrefab;
    public GameObject effectXichMaoKiemPrefab;

    [Header("Shuriken")]
    public GameObject shurikenPrefab;
    public bool shurikenThrowed;

    //
    public Animator animPlayer;
    public Animator animWeapon;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public SpriteRenderer spriteEyes;
    public Transform spriteTransf;
    public bool isFaceLeft;

    public float x;
    public float y;

    public float moveSpeed;

    public bool inWater;

    public float timeEffectDelay = .15f;
    public GameObject effectMovePrefab;
    public GameObject effectMoveWaterPrefab;

    public bool chayNuoc;

    public bool dieuLuon;
    public SpriteRenderer spriteDieuLuon;
    public GameObject dieuLuonPrefab;

    public float jumpPow;
    public float jumpPowLevel2;
    public float jumpPowLevel3;
    public bool jumping;

    public BoxCollider2D boxCheckGround;
    public bool grounded;

    public BoxDamageEnemy BoxDamageEnemy;

    public bool attacking;
    public bool attackJumpUp;
    public int numSlash;
    public float timeSlash;
    public float timeDelaySlash = .3f;
    public float timeCooldown;

    public GameObject effectRagePrefab;
    public float timeDelayRage;

    [Header("Hit")]
    public bool hitting;

    private void Update()
    {

        SetState();

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (Manager.manager.talking)
            {
                rb.velocity = Vector2.zero;
                Manager.manager.DisplayNextSentences();
            }
            else
            {
                if (!Manager.manager.isPauseGame && !Manager.manager.playerTalking && npcToTalk != null &&
                    x == 0 && y == 0)
                {
                    rb.velocity = Vector2.zero;
                    npcToTalk.TriggerDialogue();
                }
            }
        }

        if (!Manager.manager.isPauseGame && !Manager.manager.playerTalking)
        {
            if (timeDelayRage <= 0)
            {
                animPlayer.SetFloat("VelocityX", x);
                y = rb.velocity.y;
                animPlayer.SetFloat("VelocityY", y);
            }

            if (x != 0)
            {
                if (grounded)
                {
                    timeEffectDelay -= Time.deltaTime;
                    if (timeEffectDelay <= 0)
                    {
                        GameObject e = Instantiate((chayNuoc ? effectMoveWaterPrefab : effectMovePrefab), new Vector3(transform.position.x, transform.position.y - 1f), Quaternion.identity);
                        e.GetComponent<SpriteRenderer>().flipX = isFaceLeft;
                        Destroy(e, .25f);
                        timeEffectDelay = .15f;
                    }
                }
            }

            Move();
            CheckGround();
            CheckWater();
            Jump();
            Sit();
            Attack();

            FindNPCToTalk();

            CheckCollect();

            Invisible();
            Rage();
        }
    }

    void SetState()
    {
        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1_Rage"))
        {
            state = State.Slash1;
            return;
        }
        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2_Rage"))
        {
            state = State.Slash2;
            return;
        }
        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3_Rage"))
        {
            state = State.Slash3;
            return;
        }
        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1UnGrounded") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1UnGrounded_Rage"))
        {
            state = State.Slash1Fall;
            return;
        }
        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2UnGrounded") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2UnGrounded_Rage"))
        {
            state = State.Slash2Fall;
            return;
        }

        if (x != 0)
        {
            if (y > 0)
            {
                state = State.Jump;
                return;
            }
            else if (y < 0)
            {
                state = State.Fall;
                return;
            }

            state = State.Move;
            return;
        }

        if (x == 0)
        {
            if (y > 0)
            {
                state = State.Jump;
                return;
            }
            else if (y < 0)
            {
                state = State.Fall;
                return;
            }

            state = State.Idle;
            return;
        }


    }

    void Move()
    {
        x = Input.GetAxisRaw("Horizontal");

        if (!Manager.manager.isPauseGame && timeDelayRage <= 0 && !hitting)
        {
            if (x > 0)
            {
                SetFace(false);
            }
            else if (x < 0)
            {
                SetFace(true);
            }

            float calWater = 1;

            if (inWater)
            {
                if (chayNuoc)
                {
                    calWater = .8f;
                }
                else
                {
                    calWater = 1.5f;
                }
            }

            if (!attacking)
            {
                rb.velocity = new Vector2(x * (moveSpeed / (dieuLuon ? 2f : 1) / calWater), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(x * (moveSpeed / (dieuLuon ? 2f : 1) / calWater) * .5f, rb.velocity.y);
            }
        }
    }

    void SetFace(bool b)
    {
        isFaceLeft = b;
        spriteTransf.eulerAngles = new Vector3(0, 180 * (isFaceLeft ? 1 : 0), 0);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && dieuLuon && !grounded)
        {
            rb.velocity = Vector2.zero;
        }

        if (Input.GetKey(KeyCode.W) && grounded && !jumping && !attacking && !hitting)
        {
            if (!dieuLuon)
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxCheck.transform.position, boxCheck.size, 0);
                foreach (var item in collider2Ds)
                {
                    if (item.gameObject.layer == 15)
                    {
                        Destroy(item.gameObject);
                        SetDieuLuon(true);
                        return;
                    }
                }
            }

            if (Manager.manager.levelJump == 0)
            {
                StartCoroutine(JumpCo());
            }
            else if (Manager.manager.levelJump == 1)
            {
                StartCoroutine(JumpCo2());
            }
            else if (Manager.manager.levelJump == 2)
            {

            }
        }
    }

    IEnumerator JumpCo()
    {
        rb.velocity = Vector2.zero;
        jumping = true;
        rb.AddForce(Vector2.up * jumpPow);
        yield return new WaitForSeconds(.1f);
        jumping = false;
    }

    IEnumerator JumpCo2()
    {
        rb.velocity = Vector2.zero;
        jumping = true;
        rb.AddForce(Vector2.up * (jumpPowLevel2 / (dieuLuon ? 3 : 1)));
        yield return new WaitForSeconds(.1f);
        jumping = false;
        yield return new WaitForSeconds(.15f);

        if (!dieuLuon)
        {
            animPlayer.SetTrigger("Roll");
        }
    }

    void SetDieuLuon(bool b)
    {
        dieuLuon = b;
        spriteDieuLuon.enabled = b;
        rb.gravityScale = (b ? .5f : 3);
    }

    void Sit()
    {
        if (Input.GetKeyDown(KeyCode.S) && grounded)
        {
            if (dieuLuon)
            {
                SetDieuLuon(false);
                GameObject d = Instantiate(dieuLuonPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    void CheckGround()
    {
        if (!chayNuoc)
        {
            rb.gravityScale = 3;
        }
        else
        {
            if (x != 0)
            {
                rb.gravityScale = 0;
                rb.velocity = new Vector2(rb.velocity.x, 0);

            }
            else
            {
                rb.gravityScale = 1;
            }
        }

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxCheckGround.transform.position, boxCheckGround.size, 0);
        foreach (var item in collider2Ds)
        {
            if (item.gameObject.layer == 16)
            {
                if (x != 0)
                {
                    Vector3 p = new Vector3(transform.position.x, item.transform.position.y + 1);
                    transform.position = p;
                }

                chayNuoc = true;
                SetGrounded(true);
                return;
            }

            if (item.gameObject.layer == 9)
            {
                chayNuoc = false;
                if (!grounded)
                {
                    GameObject e = Instantiate(effectMoveWaterPrefab, new Vector3(transform.position.x, transform.position.y - 1), Quaternion.identity);
                    Destroy(e, .25f);

                    timeDelaySlash = .15f;
                }
                SetGrounded(true);
                return;
            }
        }
        chayNuoc = false;
        SetGrounded(false);
    }

    void SetGrounded(bool b)
    {
        grounded = b;
        animPlayer.SetBool("Grounded", grounded);
    }

    void CheckWater()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxCheck.transform.position, boxCheck.size, 0);
        foreach (var item in collider2Ds)
        {
            if (item.gameObject.layer == 16)
            {
                inWater = true;
                return;
            }
        }
        inWater = false;
    }

    #region A1
    //void Attack()
    //{
    //    animPlayer.SetBool("Attacking", attacking);
    //    animWeapon.SetBool("Attacking", attacking);

    //    //if(animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
    //    //{
    //    //    if (attacking)
    //    //    {
    //    //        numSlash = 0;
    //    //        attacking = false;
    //    //        animPlayer.SetBool("Slash1", false);
    //    //        animPlayer.SetBool("Slash2", false);
    //    //        animPlayer.SetBool("Slash3", false);

    //    //        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //    //        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
    //    //        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
    //    //    }
    //    //}

    //    if (Time.time - timeSlash >= timeDelaySlash)
    //    {
    //        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1"))
    //        {
    //            //CheckToThrowShuriken();

    //            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
    //            {
    //                if (!animPlayer.GetBool("Slash2"))
    //                {
    //                    numSlash = 0;
    //                    attacking = false;
    //                    animPlayer.SetBool("Slash1", false);

    //                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //                }
    //            }
    //        }
    //        else if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2"))
    //        {
    //            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
    //            {
    //                if (Manager.manager.nameWeapon == NameWeapon.PhiTieu)
    //                {
    //                    ThrowShuriken();
    //                }

    //                if (Manager.manager.nameWeapon == NameWeapon.XichMaoKiem)
    //                {
    //                    GameObject e = Instantiate(effectXichMaoKiemPrefab, transform.position, Quaternion.identity);
    //                    e.transform.eulerAngles = new Vector3(0, 180 * (isFaceLeft ? 1 : 0), 0);
    //                    Destroy(e, .5f);
    //                }

    //                if (!animPlayer.GetBool("Slash3"))
    //                {
    //                    numSlash = 0;
    //                    attacking = false;
    //                    animPlayer.SetBool("Slash1", false);
    //                    animPlayer.SetBool("Slash2", false);

    //                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3"))
    //            {
    //                if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .3f)
    //                {
    //                    if (!attackJumpUp)
    //                    {
    //                        attackJumpUp = true;
    //                        rb.velocity = Vector2.zero;
    //                        rb.AddForce(Vector2.up * 500);
    //                    }
    //                }

    //                numSlash = 0;
    //                attacking = false;
    //                animPlayer.SetBool("Slash1", false);
    //                animPlayer.SetBool("Slash2", false);
    //                animPlayer.SetBool("Slash3", false);

    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
    //            }
    //        }
    //    }

    //    if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1UnGrounded"))
    //    {
    //        if (!grounded)
    //        {
    //            rb.velocity = Vector2.zero;
    //            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
    //            {
    //                if (Manager.manager.nameWeapon == NameWeapon.PhiTieu)
    //                {
    //                    ThrowShuriken();
    //                }

    //                if (!animPlayer.GetBool("Slash2"))
    //                {
    //                    numSlash = 0;
    //                    attacking = false;
    //                    animPlayer.SetBool("Slash1", false);
    //                    animPlayer.SetBool("Slash2", false);
    //                    animPlayer.SetBool("Slash3", false);

    //                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
    //                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
    //                }
    //            }
    //        }
    //    }
    //    else if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2UnGrounded"))
    //    {
    //        if (!grounded)
    //        {
    //            rb.velocity = Vector2.zero;

    //            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
    //            {
    //                if (Manager.manager.nameWeapon == NameWeapon.PhiTieu)
    //                {
    //                    ThrowShuriken();
    //                }

    //                if (Manager.manager.nameWeapon == NameWeapon.XichMaoKiem)
    //                {
    //                    GameObject e = Instantiate(effectXichMaoKiemPrefab, transform.position, Quaternion.identity);
    //                    e.transform.eulerAngles = new Vector3(0, 180 * (isFaceLeft ? 1 : 0), 0);
    //                    Destroy(e, .5f);
    //                }

    //                numSlash = 0;
    //                attacking = false;
    //                animPlayer.SetBool("Slash1", false);
    //                animPlayer.SetBool("Slash2", false);
    //                animPlayer.SetBool("Slash3", false);

    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);

    //                timeCooldown = .1f;
    //            }
    //        }
    //        else
    //        {
    //            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
    //            {
    //                numSlash = 0;
    //                attacking = false;
    //                animPlayer.SetBool("Slash1", false);
    //                animPlayer.SetBool("Slash2", false);
    //                animPlayer.SetBool("Slash3", false);

    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);

    //                timeCooldown = .1f;
    //            }
    //        }
    //    }
    //    else if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3"))
    //    {
    //        if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .3f)
    //        {
    //            if (!attackJumpUp)
    //            {
    //                attackJumpUp = true;
    //                rb.velocity = Vector2.zero;
    //                rb.AddForce(Vector2.up * 500);
    //                timeCooldown = .25f;
    //            }

    //            numSlash = 0;
    //            attacking = false;
    //            animPlayer.SetBool("Slash1", false);
    //            animPlayer.SetBool("Slash2", false);
    //            animPlayer.SetBool("Slash3", false);

    //            animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //            animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
    //            animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
    //        }
    //    }

    //    if (timeCooldown > 0)
    //    {
    //        timeCooldown -= Time.deltaTime;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space) && timeCooldown <= 0)
    //    {
    //        attackJumpUp = false;
    //        if (grounded)
    //        {
    //            numSlash++;
    //            attacking = true;
    //            timeSlash = Time.time;

    //            if (!Manager.manager.invisibleMode)
    //            {
    //                animPlayer.SetBool("Slash" + numSlash, true);
    //            }
    //            if (Manager.manager.nameWeapon != NameWeapon.PhiTieu)
    //            {
    //                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash" + numSlash, true);

    //                if (numSlash >= 3)
    //                {
    //                    numSlash = 0;
    //                }
    //            }
    //            else
    //            {
    //                animWeapon.SetTrigger("ThrowShuriken");
    //                shurikenThrowed = false;
    //                if (numSlash >= 3)
    //                {
    //                    numSlash = 0;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            numSlash++;
    //            attacking = true;
    //            timeSlash = Time.time;

    //            animPlayer.SetBool("Slash" + numSlash, true);
    //            animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash" + numSlash, true);

    //            attackJumpUp = false;
    //        }
    //    }
    //}
    #endregion

    #region A2
    //void Attack()
    //{
    //    animPlayer.SetBool("Attacking", attacking);
    //    animWeapon.SetBool("Attacking", attacking);

    //    #region Chém trên không
    //    if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1UnGrounded"))
    //    {
    //        rb.velocity = Vector2.zero;
    //    }
    //    if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2UnGrounded"))
    //    {
    //        rb.velocity = Vector2.zero;
    //    }
    //    #endregion

    //    if (animWeapon.GetCurrentAnimatorStateInfo(0).IsName("Slash3"))
    //    {

    //        if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .5f)
    //        {
    //            timeSlash = 0;
    //        }
    //    }

    //    if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3") ||
    //        animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3_Rage"))
    //    {
    //        if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
    //        {
    //            if (!attackJumpUp)
    //            {
    //                attackJumpUp = true;
    //                rb.velocity = Vector2.zero;
    //                numSlash = 0;
    //                rb.AddForce(Vector2.up * 500);
    //                attacking = false;
    //                timeCooldown = .1f;
    //                animPlayer.SetBool("Slash1", false);
    //            }

    //            timeSlash = 0;
    //            timeCooldown = .25f;
    //        }
    //    }

    //    if (timeSlash > 0)
    //    {
    //        timeSlash -= Time.deltaTime;

    //    }

    //    if (timeSlash <= 0)
    //    {
    //        numSlash = 0;
    //        attacking = false;
    //        animPlayer.SetBool("Slash1", false);
    //        animPlayer.SetBool("Slash2", false);
    //        animPlayer.SetBool("Slash3", false);

    //        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
    //        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
    //        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
    //    }

    //    if (timeCooldown > 0)
    //    {
    //        timeCooldown -= Time.deltaTime;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space) && timeCooldown <= 0 && timeDelayRage <= 0)
    //    {
    //        if (grounded)
    //        {
    //            numSlash++;
    //            attacking = true;
    //            timeSlash += .3f;

    //            animPlayer.SetBool("Slash" + numSlash, true);
    //            animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash" + numSlash, true);

    //            if (numSlash >= 3)
    //            {
    //                attackJumpUp = false;

    //                numSlash = 0;

    //                timeCooldown = .0f;
    //            }
    //        }
    //        else
    //        {
    //            numSlash++;
    //            attacking = true;
    //            timeSlash += .3f;

    //            animPlayer.SetBool("Slash" + numSlash, true);
    //            animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash" + numSlash, true);

    //            attackJumpUp = false;

    //            if (numSlash >= 2)
    //            {
    //                attackJumpUp = false;

    //                numSlash = 0;

    //                timeCooldown = .05f;
    //            }
    //        }
    //    }
    //}
    #endregion


    void Attack()
    {
        animPlayer.SetBool("Attacking", attacking);
        animWeapon.SetBool("Attacking", attacking);


        if (timeSlash > 0)
        {
            timeSlash -= Time.deltaTime;
        }

        if (timeDelaySlash > 0)
        {
            timeDelaySlash -= Time.deltaTime;
        }

        if (animWeapon.GetCurrentAnimatorStateInfo(0).IsName("KiemTreSlash1") ||
            animWeapon.GetCurrentAnimatorStateInfo(0).IsName("KiemTreSlash2"))
        {
            if (animWeapon.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
            {
                if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    attacking = false;
                    animPlayer.SetBool("Slash1", false);
                    animPlayer.SetBool("Slash2", false);
                    animPlayer.SetBool("Slash3", false);

                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
                }
            }
        }

        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1_Rage"))
        {
            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
            {
                if (!animPlayer.GetBool("Slash2"))
                {
                    attacking = false;
                    animPlayer.SetBool("Slash1", false);
                    animPlayer.SetBool("Slash2", false);
                    animPlayer.SetBool("Slash3", false);

                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
                }
            }
        }

        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2_Rage"))
        {
            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
            {
                if (!animPlayer.GetBool("Slash3"))
                {
                    attacking = false;
                    animPlayer.SetBool("Slash1", false);
                    animPlayer.SetBool("Slash2", false);
                    animPlayer.SetBool("Slash3", false);

                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
                }
            }
        }

        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3_Rage"))
        {
            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
            {
                attacking = false;
                animPlayer.SetBool("Slash1", false);
                animPlayer.SetBool("Slash2", false);
                animPlayer.SetBool("Slash3", false);

                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
            }
            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
            {
                if (!attackJumpUp)
                {
                    attackJumpUp = true;

                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(Vector2.up * (500 / (dieuLuon ? 3 : 1)));
                }
            }
        }

        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1UnGrounded") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash1UnGrounded_Rage"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
            {
                if (!animPlayer.GetBool("Slash2"))
                {
                    attacking = false;
                    animPlayer.SetBool("Slash1", false);
                    animPlayer.SetBool("Slash2", false);

                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
                    animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
                }
            }
        }

        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2UnGrounded") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash2UnGrounded_Rage"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            if (animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
            {
                timeDelaySlash = .15f;

                attacking = false;
                animPlayer.SetBool("Slash1", false);
                animPlayer.SetBool("Slash2", false);

                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
                animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && timeDelaySlash <= 0 && !hitting)
        {
            switch (state)
            {
                case State.Idle:
                    attacking = true;
                    animPlayer.SetBool("Slash1", true);
                    animWeapon.SetBool("KiemTreSlash1", true);
                    break;
                case State.Sit:
                    break;
                case State.Move:
                    break;
                case State.Jump:
                    if (y <= 6.5f)
                    {
                        attackJumpUp = false;
                        attacking = true;
                        animPlayer.SetBool("Slash1", true);
                        animWeapon.SetBool("KiemTreSlash1", true);
                    }
                    break;
                case State.Fall:
                    attackJumpUp = false;
                    attacking = true;
                    animPlayer.SetBool("Slash1", true);
                    animWeapon.SetBool("KiemTreSlash1", true);
                    break;
                case State.Slash1:
                    attackJumpUp = false;
                    attacking = true;
                    animPlayer.SetBool("Slash2", true);
                    animWeapon.SetBool("KiemTreSlash2", true);
                    break;
                case State.Slash2:
                    attackJumpUp = false;
                    attacking = true;
                    animPlayer.SetBool("Slash3", true);
                    animWeapon.SetBool("KiemTreSlash3", true);
                    break;
                case State.Slash3:
                    break;
                case State.Slash1Fall:
                    attackJumpUp = false;
                    attacking = true;
                    animPlayer.SetBool("Slash2", true);
                    animWeapon.SetBool("KiemTreSlash2", true);
                    break;
                case State.Slash2Fall:
                    break;
            }
        }
    }

    void CheckToThrowShuriken()
    {
        if (Manager.manager.nameWeapon == NameWeapon.PhiTieu && animPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= .5f && !shurikenThrowed)
        {
            shurikenThrowed = true;
            ThrowShuriken();
        }
    }

    void ThrowShuriken()
    {
        GameObject s = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
        s.GetComponent<Shuriken>().dir = (isFaceLeft ? Vector2.left : Vector2.right);
    }

    public void DamageEnemy(GameObject enemy)
    {
        if (Manager.manager.rageMode)
        {
            Manager.manager.ShakeCam(.1f, .1f);
        }

        bool addForceEnemyUp = false;

        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3") ||
            animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Slash3_Rage"))
        {
            addForceEnemyUp = true;
        }
        else
        {
            addForceEnemyUp = false;
        }

        enemy.GetComponent<Enemy>().TakeDamage(Manager.manager.damage, addForceEnemyUp);
        //
        GameObject e = null;
        switch (Manager.manager.nameWeapon)
        {
            case NameWeapon.KiemTre:
                break;
            case NameWeapon.LoiKiem:
                int rSprite = Random.Range(0, 2);
                int rFlipx = Random.Range(0, 2);

                Vector2 p = enemy.transform.position;
                p = new Vector2(Random.Range(p.x - .2f, p.x + .2f), Random.Range(p.y - .35f, p.y + .35f));
                e = Instantiate(effectLoiKiemPrefab, p, Quaternion.identity);

                e.transform.GetChild(rSprite).gameObject.SetActive(true);
                e.transform.GetChild(rSprite).GetComponent<SpriteRenderer>().flipX = (rFlipx == 0 ? true : false);

                Destroy(e, .1f);
                break;
            case NameWeapon.HoaLongKiem:
                break;
            case NameWeapon.HanBangKiem:
                e = Instantiate(effectHanBangKiemPrefab, enemy.transform.position, Quaternion.identity);
                Destroy(e, 3);
                break;
            case NameWeapon.PhiTieu:
                break;
            case NameWeapon.XichMaoKiem:
                break;
        }
    }

    public void FindNPCToTalk()
    {
        Collider2D[] c = Physics2D.OverlapBoxAll(boxCheck.transform.position, boxCheck.size, 0);
        foreach (var item in c)
        {
            if (item.gameObject.layer == 13)
            {
                if (!item.GetComponent<NPC>().moving)
                {
                    npcToTalk = item.gameObject.GetComponent<NPC>();
                    Manager.manager.textV.SetActive(true);
                    return;
                }
            }
        }
        Manager.manager.textV.SetActive(false);
        npcToTalk = null;
        return;
    }

    public void CheckCollect()
    {
        Collider2D[] c = Physics2D.OverlapBoxAll(boxCheck.transform.position, boxCheck.size, 0);
        foreach (var item in c)
        {
            if (item.gameObject.layer == 14)
            {
                if (item.gameObject.name == "Small Heal Potion")
                {
                    Manager.manager.TakeItem("Small Heal Potion");
                    Destroy(item.gameObject);
                    return;
                }
                if (item.gameObject.name == "Normal Heal Potion")
                {
                    Manager.manager.TakeItem("Normal Heal Potion");
                    Destroy(item.gameObject);
                    return;
                }
                if (item.gameObject.name == "Big Heal Potion")
                {
                    Manager.manager.TakeItem("Big Heal Potion");
                    Destroy(item.gameObject);
                    return;
                }

                if (item.gameObject.name == "Small Mana Potion")
                {
                    Manager.manager.TakeItem("Small Mana Potion");
                    Destroy(item.gameObject);
                    return;
                }
                if (item.gameObject.name == "Normal Mana Potion")
                {
                    Manager.manager.TakeItem("Normal Mana Potion");
                    Destroy(item.gameObject);
                    return;
                }
                if (item.gameObject.name == "Big Mana Potion")
                {
                    Manager.manager.TakeItem("Big Mana Potion");
                    Destroy(item.gameObject);
                    return;
                }
                if (item.gameObject.name == "Coin")
                {
                    Manager.manager.TakeItem("Coin");
                    Destroy(item.gameObject);
                    return;
                }
                if (item.gameObject.name == "Kunai Cua Sakura")
                {
                    Manager.manager.CheckMission("Kunai của Sakura");
                    Destroy(item.gameObject);
                    return;
                }
            }
        }
    }

    void Invisible()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !hitting)
        {
            animPlayer.SetBool("Invisible", !animPlayer.GetBool("Invisible"));
            Manager.manager.invisibleMode = animPlayer.GetBool("Invisible");
            sprite.enabled = !Manager.manager.invisibleMode;
            spriteEyes.enabled = Manager.manager.invisibleMode;
        }
    }

    void Rage()
    {
        if (timeDelayRage > 0)
        {
            timeDelayRage -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E) && timeDelayRage <= 0 && grounded && !attacking && !hitting)
        {
            timeDelayRage = 1f;
            GameObject e = Instantiate(effectRagePrefab, transform.position, Quaternion.identity, transform);
            Destroy(e, timeDelayRage);

            StartCoroutine(RageCo());
        }
    }

    IEnumerator RageCo()
    {
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        Manager.manager.rageMode = !Manager.manager.rageMode;
        animPlayer.SetBool("Rage", Manager.manager.rageMode);
    }

    public void TakeHit(Enemy enemy)
    {
        if (Manager.manager.isAlive)
        {
            StartCoroutine(TakeHitCo(enemy));
        }
    }

    public void TakeHit(Transform trans, float damage)
    {
        StartCoroutine(TakeHitCo(trans, damage));
    }

    IEnumerator TakeHitCo(Enemy enemy)
    {
        attacking = false;
        animPlayer.SetBool("Slash1", false);
        animPlayer.SetBool("Slash2", false);
        animPlayer.SetBool("Slash3", false);

        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
        timeDelaySlash = .1f;


        rb.velocity = Vector2.zero;

        hitting = true;
        animPlayer.SetTrigger("Hit");
        Vector2 dir = (transform.position - enemy.transform.position).normalized;
        rb.AddForce(new Vector2(dir.x * 100, dir.y * (200 / (dieuLuon ? 8 : 1))));

        Manager.manager.MakeTextDamage(transform.position, enemy.damage);
        Manager.manager.MakeEffectHitBlood(transform.position, (transform.position.x < enemy.transform.position.x ? true : false));

        TakeDamage(enemy.damage);

        yield return new WaitForSeconds(10f / 60f);

        hitting = false;
    }

    IEnumerator TakeHitCo(Transform trans, float damage)
    {
        attacking = false;
        animPlayer.SetBool("Slash1", false);
        animPlayer.SetBool("Slash2", false);
        animPlayer.SetBool("Slash3", false);

        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash1", false);
        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash2", false);
        animWeapon.SetBool(Manager.manager.nameWeapon.ToString() + "Slash3", false);
        timeDelaySlash = .1f;


        rb.velocity = Vector2.zero;

        hitting = true;
        animPlayer.SetTrigger("Hit");
        Vector2 dir = (transform.position - trans.position).normalized;
        rb.AddForce(new Vector2(dir.x * 100, dir.y * 200));

        Manager.manager.MakeTextDamage(transform.position, damage);
        Manager.manager.MakeEffectHitBlood(transform.position, (transform.position.x < trans.position.x ? true : false));

        TakeDamage(damage);

        yield return new WaitForSeconds(10f / 60f);

        hitting = false;
    }

    public void PlayerReset()
    {
        hitting = false;
    }

    void TakeDamage(float amount)
    {
        Manager.manager.currentHealth = Mathf.Clamp(Manager.manager.currentHealth - amount, 0, Manager.manager.maxHealth);
        Manager.manager.SetImageStatsPlayer();

        if (Manager.manager.currentHealth <= 0)
        {
            Manager.manager.Death();
        }
    }
}
