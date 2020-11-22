using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowMain : MonoBehaviour
{
    public float smooth;

    public Vector3 offset;

    public bool lockMode;
    public Vector3 minV;
    public Vector3 maxV;

    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        Vector3 p = Vector3.Lerp(transform.position, player.transform.position + offset, smooth);
        if (lockMode)
        {
            if (p.x < minV.x)
            {
                p.x = minV.x;
            }
            if (p.y < minV.y)
            {
                p.y = minV.y;
            }
            if (p.x > maxV.x)
            {
                p.x = maxV.x;
            }
            if (p.y > maxV.y)
            {
                p.y = maxV.y;
            }
        }
        transform.position = p;
    }
}
