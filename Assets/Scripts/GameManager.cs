using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<GameObject> objectCubes;
    public float speed = 1f;
    private float speedNow;
    public float shiftAcc = 3f;
    private bool instantiated = false;
    private GameObject objectCube;
    private GameObject objectCubeClone;
    private GameObject nextObjectCube;
    private int nextCubeId;
    public int high = 16;//层数
    private Vector3 startPoint;
    private Vector3 startPointClone;
    private Vector3 startPointCloneDown = new Vector3(0f,-5f,0f);
    private Vector3 tempPoint = new Vector3(0f, 13.5f, 12f);
    private Vector3 cloneDistance = new Vector3(0f, 21f, 0f);
    private List<Vector3> startRotations;
    private string playerKey = "0";
    public float timeGap = 0.02f;
    private float timer = 0f;
    private float endPointCloney;
    private bool stoped = false;
    private List<Color32> colorList;
    private List<GameObject>[] cubeCloneList;
    private bool[] eliminated;
    public int cubeNum = 36;//每层格子数
    public int score;
    public GameObject scoreText;

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
        cubeCloneList = new List<GameObject>[high];
        for(int i = 0; i < high; i++)
        {
            cubeCloneList[i] = new List<GameObject>();
        }
        eliminated = new bool[high];

        nextCubeId = Random.Range(0, objectCubes.Count);
        nextObjectCube = Instantiate(objectCubes[nextCubeId]);
        nextObjectCube.transform.parent = GameObject.Find("Origin").transform;
        nextObjectCube.transform.position = tempPoint;
        foreach (Transform nextCube in nextObjectCube.transform)
        {
            nextCube.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!instantiated)
        {
            int cubeId = nextCubeId;
            nextCubeId = Random.Range(0, objectCubes.Count);
            Vector3 startRotation = startRotations[Random.Range(0, 8)];
            Color32 colorCube = colorList[Random.Range(0, 6)];

            Vector3 postposition = nextObjectCube.transform.position;
            Quaternion postrotation = nextObjectCube.transform.rotation;
            Destroy(nextObjectCube);
            nextObjectCube = Instantiate(objectCubes[nextCubeId], postposition, postrotation);
            nextObjectCube.transform.parent = GameObject.Find("Origin").transform;
            foreach (Transform nextCube in nextObjectCube.transform)
            {
                nextCube.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            objectCubeClone = Instantiate(objectCubes[cubeId], startPointCloneDown, Quaternion.Euler(startRotation));
            startPointClone = startPointCloneDown + objectCubeClone.transform.position - TransMiny(objectCubeClone.GetComponentsInChildren<Transform>());
            objectCubeClone.transform.position = startPointClone;
            startPoint = startPointClone + cloneDistance;
            objectCube = Instantiate(objectCubes[cubeId], startPoint, Quaternion.Euler(startRotation));
            for (int i = 0; i < 4; i++)
            {
                objectCubeClone.GetComponent<CollisionDetect>().cubeCorr.Add(objectCubeClone.transform.GetChild(i).gameObject, objectCube.transform.GetChild(i).gameObject);
                objectCube.GetComponent<CollisionDetect>().cubeCorr.Add(objectCube.transform.GetChild(i).gameObject, objectCubeClone.transform.GetChild(i).gameObject);
                objectCubeClone.transform.GetChild(i).GetComponent<Renderer>().enabled = false;
            }
            foreach (Transform cubeChild in objectCube.transform)
            {
                cubeChild.GetComponent<Renderer>().material.color = colorCube;
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
                objectCubeClone.transform.position =
                    new Vector3(objectCubeClone.transform.position.x, Mathf.Ceil(objectCubeClone.transform.position.y), objectCubeClone.transform.position.z);
                objectCube.transform.position = objectCubeClone.transform.position + cloneDistance;
                objectCubeClone.GetComponent<CollisionDetect>().enabled = false;
                objectCube.GetComponent<CollisionDetect>().enabled = false;
                CalScore();
            }
            else
            {
                objectCubeClone.transform.Translate(0f, -speedNow * Time.deltaTime, 0f, Space.World);

                stoped = (endPointCloney - objectCubeClone.transform.position.y) / Time.deltaTime < 0.01f;
                endPointCloney = objectCubeClone.transform.position.y;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speedNow = shiftAcc * speed;
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
                    if (Vector3.Angle(objectCubeClone.GetComponent<CollisionDetect>().normalCollision, new Vector3(0f, 1f, 0f)) <= 0.01f)
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
    }

    private void CalScore()
    {
        for (int i = 0; i < high; i++)
        {
            foreach (Transform cubeClone in objectCubeClone.transform)
            {
                if (Mathf.Abs(cubeClone.position.y - ((float)i - cloneDistance.y + 0.5f)) <= 0.01f)
                {
                    cubeCloneList[i].Add(cubeClone.gameObject);
                }
            }
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
                score = score + cubeNum;
                cubeCloneList[i].Clear();
            }
        }
        for (int i = high - 1; i >= 0; i--)
        {
            if (eliminated[i])
            {
                for (int j = i; j < high; j++)
                {
                    cubeCloneList[j].Clear();
                    if (j < high - 1)
                    {
                        foreach (GameObject cubeClone in cubeCloneList[j + 1])
                        {
                            cubeCloneList[j].Add(cubeClone);
                        }
                    }
                }
                for (int j = i; j < high; j++)
                {
                    foreach (GameObject cubeClone in cubeCloneList[j])
                    {
                        cubeClone.transform.Translate(0f, -1f, 0f, Space.World);
                        cubeClone.transform.parent.GetComponent<CollisionDetect>().cubeCorr[cubeClone].transform.Translate(0f, -1f, 0f, Space.World);
                    }
                }
                eliminated[i] = false;
            }
        }
        scoreText.GetComponent<TextMeshPro>().text = score.ToString();
    }

    private Vector3 TransMiny(Transform[] transforms)
    {
        int num = transforms.Length;
        float[] vectorys = new float[num];
        for(int i = 0; i < num; i++)
        {
            vectorys[i] = transforms[i].position.y;
        }
        return new Vector3(0f, Mathf.Min(vectorys) - 0.5f, 0f);
    }
}
