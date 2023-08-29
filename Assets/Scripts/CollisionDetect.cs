using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    public bool collided = false;
    public Vector3 normalCollision;
    private Color32 colorCube;
    public Dictionary<GameObject,GameObject> cubeCorr = new Dictionary<GameObject, GameObject>();
    // Start is called before the first frame update

    private void Awake()
    {
        colorCube = GameObject.Find("GameManager").GetComponent<GameManager>().colorList[Random.Range(0, 5)];
        foreach (Transform cubeChild in transform)
        {
            cubeChild.GetComponent<Renderer>().material.color = colorCube;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        collided = true;
        normalCollision = collision.contacts[0].normal;
    }
}
