using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(destruct(0.4f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator destruct(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
