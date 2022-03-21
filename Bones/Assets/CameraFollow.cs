using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform Playertrans;


    // Start is called before the first frame update
    void Start()
    {
       //Super fucking basic follow script, prototype for testing, improve later  
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(Playertrans.position.x, transform.position.y, transform.position.z);
    }
}
