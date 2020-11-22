using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowShuriken : MonoBehaviour
{
    public Player player;

    public GameObject shurikenPrefab;


    public void Throw()
    {
        GameObject s = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
        s.GetComponent<Shuriken>().dir = (player.isFaceLeft ? Vector2.left : Vector2.right);
    }
}
