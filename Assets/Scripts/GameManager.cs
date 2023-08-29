using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> objectCubes;
    public float speed = 1f;
    private float speedNow;
    private bool instantiated = false;
    private GameObject objectCube;
    private GameObject objectCubeClone;
    private int high = 16;//层数
    private Vector3 startPoint;
    private Vector3 startPointClone;
    private Vector3 startPointCloneUP = new Vector3(0f,-4.5f,0f);
    private Vector3 endPoint = new Vector3(0f, 0f, 0f);
    private Vector3 endPointClone;
    private Vector3 cloneDistance = new Vector3(0f, 21f, 0f);
    private List<Vector3> startRotations;
    private string playerKey = "0";
    public float timeGap = 0.02f;
    private float timer = 0f;
    private float endPointCloney;
    private bool stoped = false;
    public List<Color32> colorList;
    private List<GameObject>[] cubeCloneList;
    private bool[] eliminated;
    private int cubeNum = 36;//每层格子数
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        startRotations = new List<Vector3>() { 
            new Vector3(0f,0f,0f),
            new Vector3(90f,0f,0f),
            new Vector3(0f,90f,0f),
            new Vector3(0f,0f,90f),
            new Vector3(90f,90f,0f),
            new Vector3(90f,0f,90f),
            new Vector3(0f,90f,90f),
            new Vector3(90f,90f,90f)
            };
        speedNow = speed;
        colorList = new List<Color32>
        {
            new Color32(255,150,150,255),
            new Color32(150,255,150,255),
            new Color32(150,150,255,255),
            new Color32(255,255,150,255),
            new Color32(255,150,255,255),
            new Color32(150,255,255,255),
        };
        endPointClone = endPoint - cloneDistance;
        cubeCloneList = new List<GameObject>[high];
        eliminated = new bool[high];
    }

    // Update is called once per frame
    void Update()
    {
        if (!instantiated)
        {
            int cubeId = Random.Range(0, objectCubes.Count - 1);
            Vector3 startRotation = startRotations[Random.Range(0, 7)];
            objectCubeClone = Instantiate(objectCubes[cubeId], startPointCloneUP, Quaternion.Euler(startRotation));
            startPointClone = startPointCloneUP + objectCubeClone.transform.position - new Vector3(0f, Mathf.Max(objectCubeClone.GetComponentInChildren<Transform>().position.y), 0f);
            objectCubeClone.transform.position = startPointClone;
            startPoint = startPointClone + cloneDistance;
            objectCube = Instantiate(objectCubes[cubeId], startPoint, Quaternion.Euler(startRotation));
            for (int i = 0; i < 4; i++)
            {
                objectCubeClone.GetComponent<CollisionDetect>().cubeCorr.Add(objectCubeClone.transform.GetChild(i).gameObject, objectCube.transform.GetChild(i).gameObject);
                objectCube.GetComponent<CollisionDetect>().cubeCorr.Add(objectCube.transform.GetChild(i).gameObject, objectCubeClone.transform.GetChild(i).gameObject);
            }
            instantiated = true;
            stoped = false;
            endPointCloney = startPointClone.y;
        }
        else
        {
            if (stoped)
            {
                instantiated = false;
                timer = 0f;
                playerKey = "0";
                CalScore();
            }
            else
            {
                objectCubeClone.transform.Translate(0f, -speedNow * Time.deltaTime, 0f, Space.World);
            }
            stoped = (endPointCloney - objectCubeClone.transform.position.y) / Time.deltaTime < 0.01f;
            endPointCloney = objectCubeClone.transform.position.y;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speedNow = 2 * speed;
            }
            else
            {
                speedNow = speed;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                playerKey = "W";
                objectCubeClone.transform.Translate(-1f, 0f, 0f, Space.World);
                timer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                playerKey = "S";
                objectCubeClone.transform.Translate(1f, 0f, 0f, Space.World);
                timer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                playerKey = "A";
                objectCubeClone.transform.Translate(0f, 0f, -1f, Space.World);
                timer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                playerKey = "D";
                objectCubeClone.transform.Translate(0f, 0f, 1f, Space.World);
                timer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.J))//绕X轴进行逆时针旋转
            {
                playerKey = "J";
                objectCubeClone.transform.Rotate(90f, 0f, 0f, Space.World);
                timer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.K))//绕Z轴进行逆时针旋转
            {
                playerKey = "K";
                objectCubeClone.transform.Rotate(0f, 0f, 90f, Space.World);
                timer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.L))//绕Y轴进行逆时针旋转
            {
                playerKey = "L";
                objectCubeClone.transform.Rotate(0f, 90f, 0f, Space.World);
                timer = 0f;
            }

            if (objectCubeClone.GetComponent<CollisionDetect>().collided)
            {
                if (Vector3.Angle(objectCubeClone.GetComponent<CollisionDetect>().normalCollision, new Vector3(0f,1f,0f)) <= 0.01f)
                {
                    stoped = true;
                }
                else
                {
                    if (playerKey == "W")
                    {
                        objectCubeClone.transform.Translate(1f, 0f, 0f, Space.World);
                    }
                    else if (playerKey == "S")
                    {
                        objectCubeClone.transform.Translate(-1f, 0f, 0f, Space.World);
                    }
                    else if (playerKey == "A")
                    {
                        objectCubeClone.transform.Translate(0f, 0f, 1f, Space.World);
                    }
                    else if (playerKey == "D")
                    {
                        objectCubeClone.transform.Translate(0f, 0f, -1f, Space.World);
                    }
                    else if (playerKey == "J")//绕X轴进行逆时针旋转
                    {
                        objectCubeClone.transform.Rotate(-90f, 0f, 0f, Space.World);
                    }
                    else if (playerKey == "K")//绕Z轴进行逆时针旋转
                    {
                        objectCubeClone.transform.Rotate(0f, 0f, -90f, Space.World);
                    }
                    else if (playerKey == "L")//绕Y轴进行逆时针旋转
                    {
                        objectCubeClone.transform.Rotate(0f, -90f, 0f, Space.World);
                    }
                }
                objectCubeClone.GetComponent<CollisionDetect>().collided = false;
            }
            timer += Time.deltaTime;
            if (timer >= timeGap)
            {
                objectCube.transform.position = objectCubeClone.transform.position + cloneDistance;
                objectCube.transform.rotation = objectCubeClone.transform.rotation;
                timer = 0;
                playerKey = "0";
            }
        }
    }

    private void CalScore()
    {
        for (int i = 0; i < high; i++)
        {
            foreach (Transform cubeClone in objectCubeClone.transform)
            {
                if (cubeClone.position.y - ((float)i - cloneDistance.y) <= 0.01f)
                {
                    cubeCloneList[i].Add(cubeClone.gameObject);
                }
            }
        }
        for (int i = 0; i < high; i++)
        {
            eliminated[i] = (cubeCloneList[i].Count == cubeNum);
        }
        for (int i = 0; i < high; i++)
        {
            if (eliminated[i])
            {
                foreach (GameObject cubeClone in cubeCloneList[i])
                {
                    Destroy(cubeClone);
                    Destroy(cubeClone.transform.parent.GetComponent<CollisionDetect>().cubeCorr[cubeClone]);
                }
                score = score + 36;
            }
        }
        for (int i = 0; i < high; i++)
        {
            if (eliminated[i])
            {

            }
        }
    }
}
