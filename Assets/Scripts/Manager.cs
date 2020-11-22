using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Mission
{
    _1,
    _2,
    _3,
    _4,
    _5,
    _6,
    _7,
    _8,
    _9,
    _10,
    _11,
    _12,
    _13,
    _14,
    _15,
    _16,
    _17,
    _18,
    _19,
    _20,
}

public class Manager : MonoBehaviour
{
    public static Manager manager;

    public float timeToCreateLeaf;
    public GameObject leafPrefab;
    public GameObject leaf2Prefab;

    public Queue<string> sentences;
    public bool talking;
    public GameObject talkBox;
    public Text textTalk;
    public GameObject textV;
    public bool isTyping;

    public GameObject hitBloodPrefab;


    public GameObject textDamagePrefab;

    public GameObject textTakeHealthPrefab;

    public GameObject textTakeManaPrefab;

    public GameObject textTakeExpPrefab;

    public GameObject textTakeMoneyPrefab;

    public GameObject textLevelUpPrefab;


    public GameObject healthBarEnemyPrefab;

    public bool isPauseGame;

    public bool playerTalking;
    public NPC currentTalkNPC;

    [Header("Player")]
    public bool isAlive;

    public int level;
    public Text textLevel;
    public float exp;
    public int[] expToLevelUp = new int[] { 0, 0, 100, 200, 300, 400, 500 };

    public Vector2 oldPos;

    public int damage;

    public Text textMoney;
    public int money;

    public float maxHealth;
    public float currentHealth;

    public float maxMana;
    public float currentMana;

    public Player player;

    public Mission mission;

    public GameObject deathPanel;

    [Header("Đối tượng nhiệm vụ")]
    public GameObject sakuraThacNuoc;
    public GameObject tonikuThonSuzuya;
    //

    public Player.NameWeapon nameWeapon;

    public int levelJump;

    public bool invisibleMode;

    public bool rageMode;

    public Image imageHealth;
    public Image imageMana;
    public Image imageExp;

    [Header("Inventory")]

    public int nhoHp;
    public int vuaHp;
    public int toHp;

    public int nhoMp;
    public int vuaMp;
    public int toMp;

    //Ten - Index 
    public Dictionary<int, string> inventory = new Dictionary<int, string>();

    public bool kiemTre;
    public bool phiTieu;
    public bool loiKiem;
    public bool hoaLongKiem;
    public bool hanBangKiem;

    [Header("Chọn Vũ khí")]
    public GameObject panelSelectWeapon;
    public Image imageWeaponSelect;
    public Text textWeaponSelect;
    public int indexSelect;

    public GameObject notificationPrefab;

    [Header("Rơi vật phẩm")]

    public GameObject effectCollectPrefab;

    public float amountSmallHealthPotion;
    public float amountNormalHealthPotion;
    public float amountBigHealthPotion;

    public float amountSmallManaPotion;
    public float amountNormalManaPotion;
    public float amountBigManaPotion;

    public GameObject healthPotionSmallPrefab;
    public GameObject healthPotionNormalPrefab;
    public GameObject healthPotionBigPrefab;

    public GameObject manaPotionSmallPrefab;
    public GameObject manaPotionNormalPrefab;
    public GameObject manaPotionBigPrefab;

    public GameObject coinPrefab;
    public GameObject blueCrystalPrefab;
    public GameObject redCrystalPrefab;

    private void Awake()
    {
        if (manager == null)
        {
            manager = this;
        }
    }

    private void Start()
    {
        sentences = new Queue<string>();
        AddToInventory("Kiếm Tre");
        //AddToInventory("Lôi Kiếm");
        AddToInventory("Phi Tiêu");
        //AddToInventory("Hỏa Long Kiếm");
        //AddToInventory("Hàn Băng Kiếm");
        //AddToInventory("Xích Mao Kiếm");

        SetImageStatsPlayer();
    }

    private void Update()
    {
        CreateLeaf();

        SelectWeapon();

        if (panelSelectWeapon.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                BtnChangeWeapon(-1);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                BtnChangeWeapon(1);
            }
        }
    }

    void CreateLeaf()
    {
        timeToCreateLeaf -= Time.deltaTime;
        if (timeToCreateLeaf <= 0)
        {
            int r = Random.Range(0, 2);
            Vector2 v = new Vector2(Random.Range(-30f, 15f), 15f);
            Instantiate((r == 0 ? leafPrefab : leaf2Prefab), v, Quaternion.identity, transform);
            timeToCreateLeaf = Random.Range(0f, 2f);
        }
    }

    public void MakeEffectHitBlood(Vector2 v, bool flip)
    {
        GameObject e = Instantiate(hitBloodPrefab, v, Quaternion.identity);
        e.GetComponent<SpriteRenderer>().flipX = flip;
        Destroy(e, .25f);
    }

    public void MakeTextDamage(Vector2 v, float amount)
    {
        Vector2 p = Camera.main.WorldToScreenPoint(v);
        p = new Vector2(Random.Range(p.x - 20, p.x + 20), Random.Range(p.y - 20, p.y + 20));

        GameObject t = Instantiate(textDamagePrefab, p, Quaternion.identity, GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().position = p;
        t.GetComponent<Text>().text = "-" + amount;
        Destroy(t, .5f);
    }

    public void MakeTextTakeHealth(Vector2 v, float amount)
    {
        Vector2 p = Camera.main.WorldToScreenPoint(v);
        p = new Vector2(Random.Range(p.x - 20, p.x + 20), Random.Range(p.y - 20, p.y + 20));

        GameObject t = Instantiate(textTakeHealthPrefab, p, Quaternion.identity, GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().position = p;
        t.GetComponent<Text>().text = "+" + amount;
        Destroy(t, .5f);
    }

    public void MakeTextTakeMana(Vector2 v, float amount)
    {
        Vector2 p = Camera.main.WorldToScreenPoint(v);
        p = new Vector2(Random.Range(p.x - 20, p.x + 20), Random.Range(p.y - 20, p.y + 20));

        GameObject t = Instantiate(textTakeManaPrefab, p, Quaternion.identity, GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().position = p;
        t.GetComponent<Text>().text = "+" + amount;
        Destroy(t, .5f);
    }

    public void MakeTextTakeExp(Vector2 v, float amount)
    {
        Vector2 p = Camera.main.WorldToScreenPoint(v);
        p = new Vector2(Random.Range(p.x - 20, p.x + 20), Random.Range(p.y - 20, p.y + 20));

        GameObject t = Instantiate(textTakeExpPrefab, p, Quaternion.identity, GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().position = p;
        t.GetComponent<Text>().text = "+" + amount;
        Destroy(t, .5f);
    }

    public void MakeTextTakeMoney(Vector2 v, float amount)
    {
        Vector2 p = Camera.main.WorldToScreenPoint(v);
        p = new Vector2(Random.Range(p.x - 20, p.x + 20), Random.Range(p.y - 20, p.y + 20));

        GameObject t = Instantiate(textTakeMoneyPrefab, p, Quaternion.identity, GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().position = p;
        t.GetComponent<Text>().text = "+" + amount;
        Destroy(t, .5f);
    }

    public void MakeTextLevelUp(Vector2 v)
    {
        Vector2 p = Camera.main.WorldToScreenPoint(v);
        p = new Vector2(Random.Range(p.x - 20, p.x + 20), Random.Range(p.y - 20, p.y + 20));

        GameObject t = Instantiate(textLevelUpPrefab, p, Quaternion.identity, GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().position = p;
        Destroy(t, .5f);
    }

    void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.F) &&
            player.animWeapon.GetCurrentAnimatorStateInfo(0).IsName("None") &&
            player.animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            Manager.manager.ShowPanelSelectWeapon();
        }
    }

    public void ShowPanelSelectWeapon()
    {
        panelSelectWeapon.SetActive(!panelSelectWeapon.activeInHierarchy);
        if (panelSelectWeapon.activeInHierarchy)
        {
            isPauseGame = true;
            Time.timeScale = 0;
        }
        else
        {
            isPauseGame = false;
            Time.timeScale = 1;

            string s = textWeaponSelect.text;
            if (s == "Kiếm Tre")
            {
                SelectWeapon("KiemTre");
            }
            else if (s == "Phi Tiêu")
            {
                SelectWeapon("PhiTieu");
            }
            else if (s == "Lôi Kiếm")
            {
                SelectWeapon("LoiKiem");
            }
            else if (s == "Hỏa Long Kiếm")
            {
                SelectWeapon("HoaLongKiem");
            }
            else if (s == "Hàn Băng Kiếm")
            {
                SelectWeapon("HanBangKiem");
            }
            else if (s == "Xích Mao Kiếm")
            {
                SelectWeapon("XichMaoKiem");
            }
        }
    }

    void SelectWeapon(string s)
    {
        nameWeapon = (Player.NameWeapon)System.Enum.Parse(typeof(Player.NameWeapon), s);
    }

    public void BtnChangeWeapon(int i)
    {
        indexSelect += i;

        if (indexSelect < 0)
        {
            indexSelect = inventory.Count - 1;
        }

        if (indexSelect == inventory.Count)
        {
            indexSelect = 0;
        }

        string nameW = "";
        inventory.TryGetValue(indexSelect, out nameW);
        Sprite s = Resources.Load<Sprite>("UI/" + nameW);
        imageWeaponSelect.sprite = s;
        textWeaponSelect.text = nameW;
    }

    public void AddToInventory(string nameToAdd)
    {
        if (nameToAdd == "Kiếm Tre")
        {
            inventory.Add(inventory.Count, "Kiếm Tre");
        }
        if (nameToAdd == "Lôi Kiếm")
        {
            inventory.Add(inventory.Count, "Lôi Kiếm");
        }
        if (nameToAdd == "Phi Tiêu")
        {
            inventory.Add(inventory.Count, "Phi Tiêu");
        }
        if (nameToAdd == "Hỏa Long Kiếm")
        {
            inventory.Add(inventory.Count, "Hỏa Long Kiếm");
        }
        if (nameToAdd == "Hàn Băng Kiếm")
        {
            inventory.Add(inventory.Count, "Hàn Băng Kiếm");
        }
        if (nameToAdd == "Xích Mao Kiếm")
        {
            inventory.Add(inventory.Count, "Xích Mao Kiếm");
        }
    }

    public void StartDialogue(Dialogue dialogue, NPC npc)
    {
        talkBox.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(
            new Vector3(npc.transform.position.x, npc.transform.position.y + 1));

        npc.isTalking = true;
        currentTalkNPC = npc;

        isPauseGame = true;
        talkBox.SetActive(true);
        talking = true;
        foreach (var item in dialogue.sentences)
        {
            sentences.Enqueue(item);
        }
        DisplayNextSentences();
    }

    public void DisplayNextSentences()
    {
        if (!isTyping)
        {
            if (sentences.Count == 0)
            {
                CheckMission(textTalk.text);
                EndDialogue();

                if (currentTalkNPC)
                {
                    currentTalkNPC.isTalking = false;
                }
                return;
            }

            string sentence = sentences.Dequeue();

            StartCoroutine(TypeSentence(sentence));
        }
    }

    public void CheckMission(string s)
    {
        if (s == "Con hiểu ý ta chứ, hãy đến gặp cô Ayumi đi!")
        {
            TakeMission("Gặp cô Ayumi");
            mission = Mission._2;
        }
        else if (s == "Rồi ta sẽ giúp con hồi phục khả năng khinh công!")
        {
            TakeMission("Giết nhím nâu");
            mission = Mission._3;
        }
        else if (s == "Nhím nâu")
        {
            if (mission == Mission._3)
            {
                TakeMission("Hoàn thành, báo cáo cô Ayumi");
                mission = Mission._4;
            }
        }
        else if (s == "Thầy hiệu trưởng gọi con đó, mau lại đó nhanh lên.")
        {
            levelJump = 1;
            TakeMission("Thầy hiệu trưởng");
            mission = Mission._5;
        }
        else if (s == "Hãy đi gọi nó quay về đây gặp ta! Con bé ấy bên khu thác nước.")
        {
            if (mission == Mission._5)
            {
                TakeMission("Tìm Sakura");
                mission = Mission._6;
            }
        }
        else if (s == "Cậu giúp tớ tìm được không, chắc nó trôi xuống dưới chân thác rồi.")
        {
            TakeMission("Tìm thanh Kunai dưới thác nước");
            mission = Mission._7;
        }
        else if (s == "Kunai của Sakura")
        {
            if (mission == Mission._7)
            {
                TakeMission("Đã lấy được kiếm của Sakura");
                mission = Mission._8;
            }
        }
        else if (s == "Về trường thôi!")
        {
            sakuraThacNuoc.SetActive(false);
            TakeMission("Quay về gặp thầy hiệu trưởng!");
            mission = Mission._9;
        }
        else if (s == "Toniku sẽ đi cùng các con!")
        {
            TakeMission("Đến thôn Suzuya tìm Toniku!");
            mission = Mission._10;
        }
        else if (s == "Đi thôi!")
        {
            tonikuThonSuzuya.SetActive(false);
            TakeMission("Quay về gặp thầy hiệu trưởng!");
            mission = Mission._11;
        }
        else if (s == "Chúc các con may mắn!")
        {
            TakeMission("Lên đường!");
            mission = Mission._12;
        }
        else if (s == "Ta cần gân của 20 con ốc ma đen dưới chân núi vách Ichidai")
        {
            TakeMission("Giết 20 con ốc ma đen!");
            mission = Mission._13;
        }
        else if (s == "Ốc ma đen")
        {
            if (mission == Mission._13)
            {
                TakeMission("Báo cáo già làng!");
                mission = Mission._14;
            }
        }
        else if (s == "Sau khi ngươi giết chết được nó ta sẽ hướng dẫn người cách sử dụng diều lượn")
        {
            TakeMission("Đánh mực khổng lồ!");
            mission = Mission._15;
        }
        else if (s == "Mực khổng lồ")
        {
            if (mission == Mission._15)
            {
                TakeMission("Đã tiêu diệt mực khổng lồ");
                mission = Mission._16;
            }
        }
        else if (s == "Để diều bay được lâu và cao người phải nhấn liên tục phím nhảy")
        {
            TakeMission("Dùng diều lượn tiếp tục hành trình");
            mission = Mission._17;
        }
    }

    public void CheckObjectMission(string nMap)
    {
        sakuraThacNuoc.SetActive(false);
        tonikuThonSuzuya.SetActive(false);

        if (nMap == "Thác nước")
        {
            if (mission == Mission._6 || mission == Mission._7 || mission == Mission._8)
            {
                sakuraThacNuoc.SetActive(true);
            }
        }
        if (nMap == "Thôn Suzuya")
        {
            if (mission == Mission._10)
            {
                tonikuThonSuzuya.SetActive(true);
            }
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        textTalk.text = "";
        foreach (char item in sentence.ToCharArray())
        {
            textTalk.text += item;
            yield return new WaitForSeconds(.01f);

            if (Input.GetKeyDown(KeyCode.V))
            {
                textTalk.text = sentence;
                break;
            }
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        isPauseGame = false;
        talkBox.SetActive(false);
        talking = false;
        textTalk.text = "";
    }

    public void TakeMission(string s)
    {
        StartCoroutine(NotificationMission(s));
    }

    IEnumerator NotificationMission(string s)
    {
        GameObject n = GameObject.Find("Notification Mission");
        if (n)
        {
            Destroy(n);
        }

        GameObject g = Instantiate(notificationPrefab, GameObject.Find("Canvas").transform);
        g.name = "Notification Mission";
        GameObject t = g.transform.GetChild(1).gameObject;
        t.GetComponent<Text>().text = s;

        for (int i = 0; i < 5; i++)
        {
            if (t)
            {
                t.SetActive(false);
            }
            yield return new WaitForSeconds(.1f);
            if (t)
            {
                t.SetActive(true);
            }
            yield return new WaitForSeconds(.5f);
        }

        Destroy(g);
    }

    public void EnemyDrop(Enemy e)
    {
        Vector2 dir = (e.transform.position - player.transform.position).normalized;

        //Tạo Tiền
        GameObject m = Instantiate(coinPrefab, e.transform.position, Quaternion.identity);
        m.name = "Coin";
        m.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(50f, 100f));

        //Tạo bình hồi phục
        int rPotion = Random.Range(1, 21);
        if (rPotion <= e.dropPotion.health)
        {
            //Tạo Bình máu
            int rSize = Random.Range(1, 31);
            if (rSize < e.dropPotion.small)
            {
                GameObject g = Instantiate(healthPotionSmallPrefab, e.transform.position, Quaternion.identity);
                g.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(50f, 100f));
                g.name = "Small Heal Potion";
            }
            else if (rSize >= e.dropPotion.small && rSize < e.dropPotion.normal)
            {
                GameObject g = Instantiate(healthPotionNormalPrefab, e.transform.position, Quaternion.identity);
                g.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(50f, 100f));
                g.name = "Normal Heal Potion";
            }
            else
            {
                GameObject g = Instantiate(healthPotionBigPrefab, e.transform.position, Quaternion.identity);
                g.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(50f, 100f));
                g.name = "Big Heal Potion";
            }
            return;
        }

        if (rPotion >= e.dropPotion.mana)
        {
            //Tạo Bình mana
            int rSize = Random.Range(1, 31);
            if (rSize < e.dropPotion.small)
            {
                GameObject g = Instantiate(manaPotionSmallPrefab, e.transform.position, Quaternion.identity);
                g.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(50f, 100f));
                g.name = "Small Mana Potion";
            }
            else if (rSize >= e.dropPotion.small && rSize < e.dropPotion.normal)
            {
                GameObject g = Instantiate(manaPotionNormalPrefab, e.transform.position, Quaternion.identity);
                g.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(50f, 100f));
                g.name = "Normal Mana Potion";
            }
            else
            {
                GameObject g = Instantiate(manaPotionBigPrefab, e.transform.position, Quaternion.identity);
                g.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(50f, 100f));
                g.name = "Big Mana Potion";
            }
            return;
        }
    }

    public void TakeItem(string s)
    {
        GameObject e = Instantiate(effectCollectPrefab, player.transform.position, Quaternion.identity);
        Destroy(e, 1f);

        if (s == "Small Heal Potion")
        {
            currentHealth = Mathf.Clamp(currentHealth + amountSmallHealthPotion, 0, maxHealth);
            MakeTextTakeHealth(player.transform.position, amountSmallHealthPotion);
        }
        else if (s == "Normal Heal Potion")
        {
            currentHealth = Mathf.Clamp(currentHealth + amountNormalHealthPotion, 0, maxHealth);
            MakeTextTakeHealth(player.transform.position, amountNormalHealthPotion);
        }
        else if (s == "Big Heal Potion")
        {
            currentHealth = Mathf.Clamp(currentHealth + amountBigHealthPotion, 0, maxHealth);
            MakeTextTakeHealth(player.transform.position, amountBigHealthPotion);
        }
        else if (s == "Small Mana Potion")
        {
            currentMana = Mathf.Clamp(currentHealth + amountSmallManaPotion, 0, maxMana);
            MakeTextTakeMana(player.transform.position, amountSmallManaPotion);
        }
        else if (s == "Normal Mana Potion")
        {
            currentMana = Mathf.Clamp(currentHealth + amountNormalManaPotion, 0, maxMana);
            MakeTextTakeMana(player.transform.position, amountNormalManaPotion);
        }
        else if (s == "Big Mana Potion")
        {
            currentMana = Mathf.Clamp(currentHealth + amountBigManaPotion, 0, maxMana);
            MakeTextTakeMana(player.transform.position, amountBigManaPotion);
        }
        else if (s == "Coin")
        {
            MakeTextTakeMoney(player.transform.position, 10);
            money += 10;
        }

        SetImageStatsPlayer();
    }

    public void TakeExp(float amount)
    {
        MakeTextTakeExp(player.transform.position, amount);
        exp += amount;

        if (exp >= expToLevelUp[level + 1])
        {
            MakeTextLevelUp(player.transform.position);

            level++;
            exp = 0;

            maxHealth += 10;
            currentHealth = maxHealth;
            maxMana += 10;
            currentMana = maxMana;
            damage += 3;
        }

        SetImageStatsPlayer();
    }

    public void SetImageStatsPlayer()
    {
        imageHealth.fillAmount = currentHealth / maxHealth;
        imageMana.fillAmount = currentMana / maxMana;
        imageExp.fillAmount = exp / expToLevelUp[level + 1];
        textLevel.text = level.ToString();

        textMoney.text = money.ToString();
    }

    public void ShakeCam(float t, float d)
    {
        StartCoroutine(ShakeCamCo(t, d));
    }
    IEnumerator ShakeCamCo(float t, float d)
    {
        Transform cam = Camera.main.transform;

        Vector3 oriV = cam.position;
        Vector3 newV = Vector3.zero;

        float tLeft = t;
        while (tLeft > 0)
        {
            newV = new Vector3(Random.Range(oriV.x - d, oriV.x + d), Random.Range(oriV.y - d, oriV.y + d), oriV.z);

            cam.position = newV;

            tLeft -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        cam.position = oriV;
    }

    public void Death()
    {
        isPauseGame = true;
        deathPanel.SetActive(true);
        player.gameObject.SetActive(false);
        isAlive = false;
    }

    public void RespawnBtn()
    {
        isPauseGame = false;
        deathPanel.SetActive(false);

        isAlive = true;

        currentHealth = 50;
        currentMana = 50;
        SetImageStatsPlayer();

        player.PlayerReset();
         
        player.transform.position = oldPos;
        player.gameObject.SetActive(true);
    }
}
