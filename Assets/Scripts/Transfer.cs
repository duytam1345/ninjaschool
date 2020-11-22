using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transfer : MonoBehaviour
{
    public Vector2 newPositionPlayer;

    public Vector3 minV;
    public Vector3 maxV;

    public Enemy[] es;

    public bool isIn;

    public string nameMap;

    private void Update()
    {
        if (isIn)
        {
            foreach (Enemy item in es)
            {
                if (!item.isAlive)
                {
                    item.LoadToReSpawn();
                }
            }
        }
    }

    void Transfersition()
    {
        Manager.manager.CheckObjectMission(nameMap);

        FindObjectOfType<Player>().transform.position = newPositionPlayer;

        Camera.main.GetComponent<CameraFollowMain>().minV = minV;
        Camera.main.GetComponent<CameraFollowMain>().maxV = maxV;

        Manager.manager.oldPos = newPositionPlayer;

        foreach (Transfer item in FindObjectsOfType<Transfer>())
        {
            if (item)
            {
                item.isIn = false;
            }
        }

        isIn = true;

        foreach (Enemy item in FindObjectsOfType<Enemy>())
        {
            if (item)
            {
                item.EnemyDie();
            }
        }

        foreach (var item in es)
        {
            if (item)
            {
                item.ReSpawn();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            Transfersition();
        }
    }
}
