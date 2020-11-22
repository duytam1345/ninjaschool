using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLeaf : MonoBehaviour
{
    public Animator anim;

    public float timeToChange;

    int i = 1;

    float speed;

    private void Start()
    {
        speed = Time.deltaTime * Random.Range(.1f, 1.5f);    
    }

    private void Update()
    {
        if (transform.position.y <= -10)
        {
            Destroy(gameObject); 
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position+(Vector3.down+Vector3.left), speed);

        timeToChange -= Time.deltaTime;

        if (timeToChange <= 0)
        {
            anim.SetTrigger("To " + (i == 1 ? 2 : 1));
            i = (i == 1 ? 2 : 1);
            timeToChange = Random.Range(0f, 3f);
        }
    }
}
