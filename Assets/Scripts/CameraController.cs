using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0f, speed * Time.deltaTime, 0f));
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0f, -speed * Time.deltaTime, 0f));
        }
    }
}
