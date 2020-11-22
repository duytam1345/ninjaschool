using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float lenght;
    public float startPos;
    public GameObject original;
    public float parallaxEffect;

    private void Start()
    {
        transform.position = new Vector3(original.transform.position.x, transform.position.y);

        startPos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        float dist = (original.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
    }
}
