using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 1f;
    public List<Transform> axisForward;
    public List<Transform> axisReverse;
    public List<Transform> activeFalse30;
    public List<Transform> activeFalse330;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (NormalAngle(transform.eulerAngles.y) <= 45f)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(new Vector3(0f, speed * Time.deltaTime, 0f));
                foreach (Transform axisi in axisForward)
                {
                    axisi.Rotate(new Vector3(0f, -speed * Time.deltaTime, 0f), Space.Self);
                }
                foreach (Transform axisi in axisReverse)
                {
                    axisi.Rotate(new Vector3(0f, speed * Time.deltaTime, 0f), Space.Self);
                }
            }
        }
        if (NormalAngle(transform.eulerAngles.y) >= -45f)
        {
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(new Vector3(0f, -speed * Time.deltaTime, 0f));
                foreach (Transform axisi in axisForward)
                {
                    axisi.Rotate(new Vector3(0f, speed * Time.deltaTime, 0f), Space.Self);
                }
                foreach (Transform axisi in axisReverse)
                {
                    axisi.Rotate(new Vector3(0f, -speed * Time.deltaTime, 0f), Space.Self);
                }
            }
        }
        foreach (Transform activei in activeFalse30)
        {
            activei.gameObject.SetActive(NormalAngle(transform.eulerAngles.y) <= 30f);
        }
        foreach (Transform activei in activeFalse330)
        {
            activei.gameObject.SetActive(NormalAngle(transform.eulerAngles.y) >= -30f);
        }
    }

    private float NormalAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }
}
