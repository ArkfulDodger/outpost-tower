using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDown : MonoBehaviour
{
    float minspeed = 0.5f;
    float maxspeed = 1.2f;
    float speed;

    float baseY = -6;
    float topY = 6;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(minspeed, maxspeed);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;

        if (transform.position.y <= baseY)
            transform.position = new Vector3(transform.position.x, topY, transform.position.z);
    }
}
