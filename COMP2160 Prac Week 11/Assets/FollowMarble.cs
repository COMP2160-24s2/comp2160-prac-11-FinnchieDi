using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMarble : MonoBehaviour
{
    [SerializeField] private GameObject marble;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(marble.transform.position.x, gameObject.transform.position.y, marble.transform.position.z);
    }
}