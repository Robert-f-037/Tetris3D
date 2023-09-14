using System.Collections;
using System.Collections.Generic;
using System;
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
    public float timeInput = 0.05f;
    private float timer2 = 0f;
    private float endPointCloney;
    private bool stoped = false;
    private List<Color32> colorList;
    private List<GameObject>[] cubeCloneList;
    private bool[] eliminated;
    private int cubeNum;//每层格子数
    public int cubeNumSL = 4;
    private int cubeNumSLRange;
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
    public GameObject scoreMusic;
    public GameObject endMusic;
    public GameObject origin;

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
        cubeNum = cubeNumSL * cubeNumSL;
        cubeNumSLRange = cubeNumSL / 2;

        nextCubeId = UnityEngine.Random.Range(0, objectCubes.Count);
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

        scoreMusic.SetActive(false);
        endMusic.SetActive(false);
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
                nextCubeId = UnityEngine.Random.Range(0, objectCubes.Count);
                Vector3 startRotation = startRotations[UnityEngine.Random.Range(0, 8)];
                Color32 colorCube = colorList[UnityEngine.Random.Range(0, 6)];

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
                startPointClone = startPointCloneDown + objectCubeClone.transform.position - TransMinMaxXYZ(objectCubeClone.GetComponentsInChildren<Transform>());
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
                    objectCube.transform.position = objectCubeClone.transform.position + cloneDistance;
                    objectCubeClone.GetComponent<CollisionDetect>().enabled = false;
                    objectCube.GetComponent<CollisionDetect>().enabled = false;
                    foreach (Transform cube in objectCube.transform)
                    {
                        cube.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    if (TransMinMaxXYZ(objectCubeClone.GetComponentsInChildren<Transform>()).y < startPointCloneDown.y &&
                        TransMinMaxXYZ(objectCubeClone.GetComponentsInChildren<Transform>(), "Max").y + cloneDistance.y < (float)high)
                    {
                        instantiated = false;
                        timer = 0f;
                        playerKey = "0";
                        scoreMusic.SetActive(false);
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

                    float angleCamera = origin.GetComponent<CameraController>().angleCamera;
                    bool detectRange =
                        TransMinMaxXYZ(objectCubeClone.GetComponentsInChildren<Transform>(), "Min", "x").x < -(float)cubeNumSLRange - 0.5f ||
                        TransMinMaxXYZ(objectCubeClone.GetComponentsInChildren<Transform>(), "Max", "x").x > (float)cubeNumSLRange + 0.5f ||
                        TransMinMaxXYZ(objectCubeClone.GetComponentsInChildren<Transform>(), "Min", "z").z < -(float)cubeNumSLRange - 0.5f ||
                        TransMinMaxXYZ(objectCubeClone.GetComponentsInChildren<Transform>(), "Max", "z").z > (float)cubeNumSLRange + 0.5f;
                    if (objectCubeClone.GetComponent<CollisionDetect>().collided || detectRange)
                    {
                        if (Vector3.Angle(objectCubeClone.GetComponent<CollisionDetect>().normalCollision, new Vector3(0f, 1f, 0f)) <= 0.01f &&
                            !detectRange)
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

                            Vector3 tempposition = objectCubeClone.transform.position;
                            objectCubeClone.transform.position = 
                                new Vector3(objectCubeClone.transform.position.x, Mathf.Round(objectCubeClone.transform.position.y), objectCubeClone.transform.position.z);
                            if (DetectOverlap(objectCubeClone))
                            {
                                objectCubeClone.transform.position = tempposition;
                                if (playerKey == "W" || playerKey == "S" || playerKey == "A" || playerKey == "D")
                                {
                                    objectCubeClone.transform.Translate(MoveControl(angleCamera, playerKey, -1), Space.World);
                                }
                                else if (playerKey == "J" || playerKey == "K" || playerKey == "L")
                                {
                                    tipAlpha = 255f;
                                    objectCubeClone.transform.Rotate(MoveControl(angleCamera, playerKey, -1), Space.World);
                                }
                                playerKey = "0";
                            }
                            else
                            {
                                //objectCubeClone.transform.position = 
                                //    new Vector3(objectCubeClone.transform.position.x, objectCubeClone.transform.position.y - 1, objectCubeClone.transform.position.z);
                                //if (DetectOverlap(objectCubeClone))
                                //{
                                //    objectCubeClone.transform.position =
                                //        new Vector3(objectCubeClone.transform.position.x, objectCubeClone.transform.position.y + 1, objectCubeClone.transform.position.z);
                                //    stoped = true;
                                //}
                                stoped = true;
                            }
                        }
                        else
                        {
                            if (playerKey == "W" || playerKey == "S" || playerKey == "A" || playerKey == "D")
                            {
                                objectCubeClone.transform.Translate(MoveControl(angleCamera, playerKey, -1), Space.World);
                            }
                            else if (playerKey == "J" || playerKey == "K" || playerKey == "L")
                            {
                                tipAlpha = 255f;
                                objectCubeClone.transform.Rotate(MoveControl(angleCamera, playerKey, -1), Space.World);
                            }
                            playerKey = "0";
                        }
                        objectCubeClone.GetComponent<CollisionDetect>().collided = false;
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.W) && timer2 >= timeInput && !stoped)
                        {
                            playerKey = "W";
                            objectCubeClone.transform.Translate(MoveControl(angleCamera, playerKey), Space.World);
                            timer = 0f;
                            timer2 = 0f;
                        }
                        if (Input.GetKeyDown(KeyCode.S) && timer2 >= timeInput && !stoped)
                        {
                            playerKey = "S";
                            objectCubeClone.transform.Translate(MoveControl(angleCamera, playerKey), Space.World);
                            timer = 0f;
                            timer2 = 0f;
                        }
                        if (Input.GetKeyDown(KeyCode.A) && timer2 >= timeInput && !stoped)
                        {
                            playerKey = "A";
                            objectCubeClone.transform.Translate(MoveControl(angleCamera, playerKey), Space.World);
                            timer = 0f;
                            timer2 = 0f;
                        }
                        if (Input.GetKeyDown(KeyCode.D) && timer2 >= timeInput && !stoped)
                        {
                            playerKey = "D";
                            objectCubeClone.transform.Translate(MoveControl(angleCamera, playerKey), Space.World);
                            timer = 0f;
                            timer2 = 0f;
                        }
                        if (Input.GetKeyDown(KeyCode.J) && timer2 >= timeInput && !stoped)//绕X轴进行逆时针旋转
                        {
                            playerKey = "J";
                            objectCubeClone.transform.Rotate(MoveControl(angleCamera, playerKey), Space.World);
                            timer = 0f;
                            timer2 = 0f;
                        }
                        if (Input.GetKeyDown(KeyCode.K) && timer2 >= timeInput && !stoped)//绕Z轴进行逆时针旋转
                        {
                            playerKey = "K";
                            objectCubeClone.transform.Rotate(MoveControl(angleCamera, playerKey), Space.World);
                            timer = 0f;
                            timer2 = 0f;
                        }
                        if (Input.GetKeyDown(KeyCode.L) && timer2 >= timeInput && !stoped)//绕Y轴进行逆时针旋转
                        {
                            playerKey = "L";
                            objectCubeClone.transform.Rotate(MoveControl(angleCamera, playerKey), Space.World);
                            timer = 0f;
                            timer2 = 0f;
                        }
                    }
                    timer += Time.deltaTime;
                    timer2 += Time.deltaTime;
                    if (timer >= timeGap)
                    {
                        objectCube.transform.position = objectCubeClone.transform.position + cloneDistance;
                        objectCube.transform.rotation = objectCubeClone.transform.rotation;
                        timer = 0;
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
            endMusic.SetActive(true);
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
                scoreMusic.SetActive(true);
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

    private Vector3 TransMinMaxXYZ(Transform[] transforms, string index = "Min", string axis = "y", Transform transformStandard = null)
    {
        int num = transforms.Length;
        float[] vectors = new float[num];
        for(int i = 1; i < num; i++)
        {
            if (transformStandard == null)
            {
                if (axis == "y")
                {
                    vectors[i] = transforms[i].position.y;
                }
                else if (axis == "x")
                {
                    vectors[i] = transforms[i].position.x;
                }
                else if (axis == "z")
                {
                    vectors[i] = transforms[i].position.z;
                }
                else
                {
                    vectors[i] = transforms[i].position.y;
                }
            }
            else
            {
                if (axis == "y")
                {
                    if (Mathf.Abs(transforms[i].position.x - transformStandard.position.x) <= 0.01f &&
                        Mathf.Abs(transforms[i].position.z - transformStandard.position.z) <= 0.01f)
                    {
                        vectors[i] = transforms[i].position.y;
                    }
                    else
                    {
                        vectors[i] = transformStandard.position.y;
                    }
                }
                else if (axis == "x")
                {
                    if (Mathf.Abs(transforms[i].position.y - transformStandard.position.y) <= 0.01f && 
                        Mathf.Abs(transforms[i].position.z - transformStandard.position.z) <= 0.01f)
                    {
                        vectors[i] = transforms[i].position.x;
                    }
                    else
                    {
                        vectors[i] = transformStandard.position.x;
                    }
                }
                else if (axis == "z")
                {
                    if (Mathf.Abs(transforms[i].position.y - transformStandard.position.y) <= 0.01f &&
                        Mathf.Abs(transforms[i].position.x - transformStandard.position.x) <= 0.01f)
                    {
                        vectors[i] = transforms[i].position.z;
                    }
                    else
                    {
                        vectors[i] = transformStandard.position.z;
                    }
                }
                else
                {
                    if (Mathf.Abs(transforms[i].position.x - transformStandard.position.x) <= 0.01f && 
                        Mathf.Abs(transforms[i].position.z - transformStandard.position.z) <= 0.01f)
                    {
                        vectors[i] = transforms[i].position.y;
                    }
                    else
                    {
                        vectors[i] = transformStandard.position.y;
                    }
                }
            }
        }
        float[] trueVectors = new float[num - 1];
        Array.Copy(vectors, 1, trueVectors, 0, num - 1);
        if (index == "Min")
        {
            return new Vector3(Convert.ToSingle(axis == "x"), Convert.ToSingle(axis == "y"), Convert.ToSingle(axis == "z")) * (Mathf.Min(trueVectors) - 0.5f);
        }
        else if (index == "Max")
        {
            return new Vector3(Convert.ToSingle(axis == "x"), Convert.ToSingle(axis == "y"), Convert.ToSingle(axis == "z")) * (Mathf.Max(trueVectors) + 0.5f);
        }
        else
        {
            return new Vector3(Convert.ToSingle(axis == "x"), Convert.ToSingle(axis == "y"), Convert.ToSingle(axis == "z")) * (Mathf.Min(trueVectors) - 0.5f);
        }
    }

    private bool DetectOverlap(GameObject cubeObject)
    {
        bool overlap = false;
        for(int i = 0; i < 4; i++)
        {
            RaycastHit[] hits = Physics.RaycastAll(cubeObject.transform.GetChild(i).position + new Vector3(0f, 0.5f, 0f), new Vector3(0f, -1f, 0f), 0.5f);
            if (hits.Length != 1)
            {
                overlap = true;
                break;
            }
        }
        return overlap;
    }

    private Vector3 MoveControl(float angleCamera, string playerKey, int reverse = 1)
    {
        if (angleCamera >= 0 && angleCamera <= 90)
        {
            if (playerKey == "W")
            {
                return new Vector3(reverse * -1f, 0f, 0f);
            }
            else if (playerKey == "S")
            {
                return new Vector3(reverse * 1f, 0f, 0f);
            }
            else if (playerKey == "A")
            {
                return new Vector3(0f, 0f, reverse * -1f);
            }
            else if (playerKey == "D")
            {
                return new Vector3(0f, 0f, reverse * 1f);
            }
            else if (playerKey == "J")
            {
                return new Vector3(reverse * 90f, 0f, 0f);
            }
            else if (playerKey == "K")
            {
                return new Vector3(0f, 0f, reverse * 90f);
            }
            else if (playerKey == "L")
            {
                return new Vector3(0f, reverse * 90f, 0f);
            }
            else
            {
                return new Vector3(0f, 0f, 0f);
            }
        }
        else if (angleCamera > 90 && angleCamera <= 180)
        {
            if (playerKey == "A")
            {
                return new Vector3(reverse * -1f, 0f, 0f);
            }
            else if (playerKey == "D")
            {
                return new Vector3(reverse * 1f, 0f, 0f);
            }
            else if (playerKey == "S")
            {
                return new Vector3(0f, 0f, reverse * -1f);
            }
            else if (playerKey == "W")
            {
                return new Vector3(0f, 0f, reverse * 1f);
            }
            else if (playerKey == "J")
            {
                return new Vector3(0f, 0f, reverse * -90f);
            }
            else if (playerKey == "K")
            {
                return new Vector3(reverse * 90f, 0f, 0f);
            }
            else if (playerKey == "L")
            {
                return new Vector3(0f, reverse * 90f, 0f);
            }
            else
            {
                return new Vector3(0f, 0f, 0f);
            }
        }
        else if (angleCamera < 0 && angleCamera >= -90)
        {
            if (playerKey == "D")
            {
                return new Vector3(reverse * -1f, 0f, 0f);
            }
            else if (playerKey == "A")
            {
                return new Vector3(reverse * 1f, 0f, 0f);
            }
            else if (playerKey == "W")
            {
                return new Vector3(0f, 0f, reverse * -1f);
            }
            else if (playerKey == "S")
            {
                return new Vector3(0f, 0f, reverse * 1f);
            }
            else if (playerKey == "J")
            {
                return new Vector3(0f, 0f, reverse * 90f);
            }
            else if (playerKey == "K")
            {
                return new Vector3(reverse * -90f, 0f, 0f);
            }
            else if (playerKey == "L")
            {
                return new Vector3(0f, reverse * 90f, 0f);
            }
            else
            {
                return new Vector3(0f, 0f, 0f);
            }
        }
        else if (angleCamera < -90 && angleCamera > -180)
        {
            if (playerKey == "S")
            {
                return new Vector3(reverse * -1f, 0f, 0f);
            }
            else if (playerKey == "W")
            {
                return new Vector3(reverse * 1f, 0f, 0f);
            }
            else if (playerKey == "D")
            {
                return new Vector3(0f, 0f, reverse * -1f);
            }
            else if (playerKey == "A")
            {
                return new Vector3(0f, 0f, reverse * 1f);
            }
            else if (playerKey == "J")
            {
                return new Vector3(reverse * -90f, 0f, 0f);
            }
            else if (playerKey == "K")
            {
                return new Vector3(0f, 0f, reverse * -90f);
            }
            else if (playerKey == "L")
            {
                return new Vector3(0f, reverse * 90f, 0f);
            }
            else
            {
                return new Vector3(0f, 0f, 0f);
            }
        }
        else
        {
            return new Vector3(0f, 0f, 0f);
        }
    }
}
