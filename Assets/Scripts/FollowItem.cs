using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowItem : MonoBehaviour
{
    public GameObject followThis;
    public GameObject rotateWithThis;

    private Vector3 offset;
    
    private void Start()
    {
        if(followThis)
        {
            offset = transform.position - followThis.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(followThis)
        {
            transform.position = followThis.transform.position + offset;
        }
        
        if(rotateWithThis)
        {
            transform.rotation = rotateWithThis.transform.rotation;
        }
    }
}
