using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    public bool collided = false;
    public Vector3 normalCollision;
    public GameObject collisionObject;
    public Dictionary<GameObject,GameObject> cubeCorr = new Dictionary<GameObject, GameObject>();
    // Start is called before the first frame update

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
        collisionObject = collision.collider.gameObject;
    }
}
