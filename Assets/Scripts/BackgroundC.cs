using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundC : MonoBehaviour
{
    [SerializeField] Camera your_camera;
    [SerializeField] float parallax_value;
    Vector2 length;
    Vector3 startposition;
    

    void Start()
    {
        startposition = transform.position;      
        length = GetComponentInChildren<SpriteRenderer>().bounds.size;
    }


    void Update()
    {
        Vector3 relative_pos = your_camera.transform.position * parallax_value;   
        Vector3 dist = your_camera.transform.position - relative_pos;

        if(dist.x > startposition.x + length.x)
        {
            startposition.x +=length.x;
        }
        
        if(dist.x < startposition.x - length.x)
        {
            startposition.x -=length.x;
        }  
        relative_pos.z = startposition.z;
        transform.position = startposition + relative_pos;
    }
}
