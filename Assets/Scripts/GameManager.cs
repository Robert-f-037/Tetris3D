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
    public List<GameObject> scoreTexts;
    public GameObject scorePanel;
    public GameObject panelText;
    public GameObject tipText;
    private float tipAlpha;
    public float speedAlpha = 0.01f;
    private int highScore;
    private bool startGame;
    private int levelId;
    private string levelName;
    private GameObject LevelManager;

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

        LevelManager = GameObject.Find("LevelManager");
        levelId = LevelManager.GetComponent<LevelManager>().levelId;
        levelName = LevelManager.GetComponent<LevelManager>().levelNames[levelId];
        highScore = PlayerPrefs.GetInt(levelName);
    }

    // Update is called once per frame
    void Update()
    {
        startGame = LevelManager.GetComponent<LevelManager>().startGame;
        if (startGame)
        {
            tipText.GetComponent<TextMeshPro>().color = new Color(0f, 0f, 0f, tipAlpha);
            if (tipAlpha > 0)
            {
                tipAlpha = Mathf.Lerp(tipAlpha, 0, speedAlpha * Time.deltaTime);
            }
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
                startPointClone = startPointCloneDown + objectCubeClone.transform.position - TransMinMaxy(objectCubeClone.GetComponentsInChildren<Transform>());
                objectCubeClone.transform.position = startPointClone;
                startPoint = startPointClone + cloneDistance;
                objectCube = Instantiate(objectCubes[cubeId], startPoint, Quaternion.Euler(startRotation));
                for (int i = 0; i < 4; i++)
                {
                    objectCubeClone.GetComponent<CollisionDetect>().cubeCorr.Add(objectCubeClone.transform.GetChild(i).gameObject, objectCube.transform.GetChild(i).gameObject);
                    objectCube.GetComponent<CollisionDetect>().cubeCorr.Add(objectCube.transform.GetChild(i).gameObject, objectCubeClone.transform.GetChild(i).gameObject);
                    objectCubeClone.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Clone");
                }
                foreach (Transform cubeChild in objectCube.transform)
                {
                    cubeChild.GetComponent<Renderer>().material.color = colorCube;
                }
                instantiated = true;
                stoped = false;
                endPointCloney = startPointClone.y;

                LevelManager.GetComponent<LevelManager>().reStart = false;
            }
            else
            {
                if (stoped)
                {
                    objectCubeClone.transform.position =
                        new Vector3(objectCubeClone.transform.position.x, Mathf.Ceil(objectCubeClone.transform.position.y), objectCubeClone.transform.position.z);
                    objectCube.transform.position = objectCubeClone.transform.position + cloneDistance;
                    objectCubeClone.GetComponent<CollisionDetect>().enabled = false;
                    objectCube.GetComponent<CollisionDetect>().enabled = false;
                    foreach (Transform cube in objectCube.transform)
                    {
                        cube.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    if (TransMinMaxy(objectCubeClone.GetComponentsInChildren<Transform>()).y < startPointCloneDown.y)
                    {
                        instantiated = false;
                        timer = 0f;
                        playerKey = "0";
                        CalScore();
                    }
                    else
                    {
                        if (!LevelManager.GetComponent<LevelManager>().reStart)
                        {
                            LevelManager.GetComponent<LevelManager>().startGame = false;
                        }
                    }
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
                        if (Vector3.Angle(objectCubeClone.GetComponent<CollisionDetect>().normalCollision, new Vector3(0f, 1f, 0f)) <= 0.01f
                            && !DetectOverlap(objectCubeClone))
                        {
                            //Transform transformStandard = objectCubeClone.GetComponent<CollisionDetect>().collisionObject.transform;
                            //if (transformStandard.parent)
                            //{
                            //    if (Mathf.Abs(transformStandard.position.y - 
                            //        (TransMinMaxy(transformStandard.parent.GetComponentsInChildren<Transform>(), "Max", transformStandard).y - 0.5f)) <= 0.01f)
                            //    {
                            //        stoped = true;
                            //    }
                            //}
                            //else
                            //{
                            //    if (Mathf.Abs(transformStandard.position.y - 
                            //        (TransMinMaxy(transformStandard.GetComponentsInChildren<Transform>(), "Max", transformStandard).y - 0.5f)) <= 0.01f)
                            //    {
                            //        stoped = true;
                            //    }
                            //}
                            //逻辑上有bug
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
                                tipAlpha = 255f;
                                objectCubeClone.transform.Rotate(-90f, 0f, 0f, Space.World);
                            }
                            else if (playerKey == "K")//绕Z轴进行逆时针旋转
                            {
                                tipAlpha = 255f;
                                objectCubeClone.transform.Rotate(0f, 0f, -90f, Space.World);
                            }
                            else if (playerKey == "L")//绕Y轴进行逆时针旋转
                            {
                                tipAlpha = 255f;
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
            scorePanel.SetActive(false);
        }
        else
        {
            scorePanel.SetActive(true);
            if (score > highScore)
            {
                panelText.GetComponent<TextMeshPro>().text = "新纪录!";
                PlayerPrefs.SetInt(levelName, score);
            }
            else
            {
                panelText.GetComponent<TextMeshPro>().text = "失败";
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
        foreach(GameObject scoreText in scoreTexts)
        {
            scoreText.GetComponent<TextMeshPro>().text = score.ToString();
        }
    }

    private Vector3 TransMinMaxy(Transform[] transforms, string index = "Min", Transform transformStandard = null)
    {
        int num = transforms.Length;
        float[] vectorys = new float[num];
        for(int i = 0; i < num; i++)
        {
            if (transformStandard == null)
            {
                vectorys[i] = transforms[i].position.y;
            }
            else
            {
                if (Mathf.Abs(transforms[i].position.x - transformStandard.position.x) <= 0.01f && 
                    Mathf.Abs(transforms[i].position.z - transformStandard.position.z) <= 0.01f)
                {
                    vectorys[i] = transforms[i].position.y;
                }
                else
                {
                    vectorys[i] = transformStandard.position.y;
                }
            }
        }
        if (index == "Min")
        {
            return new Vector3(0f, Mathf.Min(vectorys) - 0.5f, 0f);
        }
        else if (index == "Max")
        {
            return new Vector3(0f, Mathf.Max(vectorys) + 0.5f, 0f);
        }
        else
        {
            return new Vector3(0f, Mathf.Min(vectorys) - 0.5f, 0f);
        }
    }

    private bool DetectOverlap(GameObject cubeObject)
    {
        bool overlap = false;
        for(int i = 0; i < 4; i++)
        {
            RaycastHit[] hits = Physics.RaycastAll(cubeObject.transform.GetChild(i).position, new Vector3(0f, 1f, 0f), 0.1f);
            if (hits.Length > 0)
            {
                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].collider.gameObject != cubeObject.transform.GetChild(i).gameObject)
                    {
                        overlap = true;
                        break;
                    }
                }
                if (overlap)
                {
                    break;
                }
            }
        }
        return overlap;
    }
}
