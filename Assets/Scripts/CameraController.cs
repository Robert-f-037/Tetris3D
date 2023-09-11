using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraController : MonoBehaviour
{
    public bool angleLimit = false;
    public float speed = 1f;
    public List<Transform> axisForward;
    public List<Transform> axisReverse;
    public List<Transform> activeFalse45;
    public List<Transform> activeFalse_45;
    public List<Transform> activeFalse135;
    public List<Transform> activeFalse_135;
    public float rangeActive;
    public float angleCamera;
    public Transform[] axisName = new Transform[8];
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        angleCamera = NormalAngle(transform.eulerAngles.y);
        if (angleLimit)
        {
            if (angleCamera <= 45f)
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
            if (angleCamera >= -45f)
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
        }
        else
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

            if (angleCamera >= 0f && angleCamera <= 90f)
            {
                axisName[0].GetComponent<TextMeshPro>().text = "W��";
                axisName[1].GetComponent<TextMeshPro>().text = "A��";
                axisName[2].GetComponent<TextMeshPro>().text = "S��";
                axisName[3].GetComponent<TextMeshPro>().text = "D��";
                axisName[4].GetComponent<TextMeshPro>().text = "J��";
                axisName[5].GetComponent<TextMeshPro>().text = "K��";
                axisName[6].GetComponent<TextMeshPro>().text = "J��";
                axisName[7].GetComponent<TextMeshPro>().text = "K��";
            }
            else if (angleCamera > 90f && angleCamera <= 180f)
            {
                axisName[0].GetComponent<TextMeshPro>().text = "A��";
                axisName[1].GetComponent<TextMeshPro>().text = "S��";
                axisName[2].GetComponent<TextMeshPro>().text = "D��";
                axisName[3].GetComponent<TextMeshPro>().text = "W��";
                axisName[4].GetComponent<TextMeshPro>().text = "K��";
                axisName[5].GetComponent<TextMeshPro>().text = "J��";
                axisName[6].GetComponent<TextMeshPro>().text = "K��";
                axisName[7].GetComponent<TextMeshPro>().text = "J��";
            }
            else if (angleCamera < 0f && angleCamera >= -90f)
            {
                axisName[0].GetComponent<TextMeshPro>().text = "D��";
                axisName[1].GetComponent<TextMeshPro>().text = "W��";
                axisName[2].GetComponent<TextMeshPro>().text = "A��";
                axisName[3].GetComponent<TextMeshPro>().text = "S��";
                axisName[4].GetComponent<TextMeshPro>().text = "K��";
                axisName[5].GetComponent<TextMeshPro>().text = "J��";
                axisName[6].GetComponent<TextMeshPro>().text = "K��";
                axisName[7].GetComponent<TextMeshPro>().text = "J��";
            }
            else if (angleCamera < -90f && angleCamera > -180f)
            {
                axisName[0].GetComponent<TextMeshPro>().text = "S��";
                axisName[1].GetComponent<TextMeshPro>().text = "D��";
                axisName[2].GetComponent<TextMeshPro>().text = "W��";
                axisName[3].GetComponent<TextMeshPro>().text = "A��";
                axisName[4].GetComponent<TextMeshPro>().text = "J��";
                axisName[5].GetComponent<TextMeshPro>().text = "K��";
                axisName[6].GetComponent<TextMeshPro>().text = "J��";
                axisName[7].GetComponent<TextMeshPro>().text = "K��";
            }

            foreach (Transform activei in activeFalse45)
            {
                activei.gameObject.SetActive(!(angleCamera <= 45f + rangeActive && angleCamera >= 45f - rangeActive));
            }
            foreach (Transform activei in activeFalse_45)
            {
                activei.gameObject.SetActive(!(angleCamera <= -45f + rangeActive && angleCamera >= -45f - rangeActive));
            }
            foreach (Transform activei in activeFalse135)
            {
                activei.gameObject.SetActive(!(angleCamera <= 135f + rangeActive && angleCamera >= 135f - rangeActive));
            }
            foreach (Transform activei in activeFalse_135)
            {
                activei.gameObject.SetActive(!(angleCamera <= -135f + rangeActive && angleCamera >= -135f - rangeActive));
            }
            axisName[4].transform.parent.gameObject.SetActive((angleCamera <= -45f && angleCamera > -180f) || (angleCamera <= 180f && angleCamera > 135f));
            axisName[5].transform.parent.gameObject.SetActive((angleCamera <= 180f && angleCamera > 45f) || (angleCamera <= -135f && angleCamera > -180f));
            axisName[6].transform.parent.gameObject.SetActive(angleCamera <= 135f && angleCamera > -45f);
            axisName[7].transform.parent.gameObject.SetActive(angleCamera <= 45f && angleCamera > -135f);
        }
    }

    private float NormalAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        if (angle <= -180)
        {
            angle += 360;
        }
        return angle;
    }
}
