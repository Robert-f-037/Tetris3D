using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int levelNum;
    public List<string> levelNames;
    public List<int> highScore;
    public int levelId = 0;
    public bool startGame = false;
    private GameObject levelNameObject;
    private GameObject highScoreObject;
    public Scene scene;
    public bool reStart = false;
    public static LevelManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        levelNames = new List<string>(){"新手", "熟练", "大师"};
        levelNum = levelNames.Count;
        for (int i = 0; i < levelNum; i++)
        {
            if (!PlayerPrefs.HasKey(levelNames[i]))
            {
                PlayerPrefs.SetInt(levelNames[i], 0);
            }
            highScore.Add(PlayerPrefs.GetInt(levelNames[i]));
        }
        levelNameObject = GameObject.Find("levelName");
        highScoreObject = GameObject.Find("highScore");
    }

    // Update is called once per frame
    void Update()
    {
        scene = SceneManager.GetActiveScene();
        if (scene.name == "Start")
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                levelId--;
                if (levelId == -1)
                {
                    levelId = levelNum - 1;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                levelId++;
                if (levelId == levelNum)
                {
                    levelId = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                SceneManager.LoadScene("Level" + levelId.ToString());
                startGame = true;
            }
            levelNameObject.GetComponent<TextMeshPro>().text = levelNames[levelId];
            highScoreObject.GetComponent<TextMeshPro>().text = "最高分:" + highScore[levelId].ToString();
        }
        else
        {
            if (!startGame)
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    SceneManager.LoadScene(scene.name);
                    startGame = true;
                    reStart = true;
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    SceneManager.LoadScene("Start");
                }
            }
        }
    }
}
