using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    GameObject cam;
    [SerializeField] float parallaxEffect;
    float startPosX;
    float lengthX;
    float startPosY;
    float lengthY;
    

    void Start()
    {
        cam = GameObject.Find("CM vcam1");
        startPosX = transform.position.x;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;

        startPosY = transform.position.y;
        lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
    }


    void Update()
    {
        float tempX = (cam.transform.position.x * (1 - parallaxEffect));
        float tempY = (cam.transform.position.y * (1 - parallaxEffect));
        float distanceX = (cam.transform.position.x * parallaxEffect);
        float distanceY = (cam.transform.position.y * parallaxEffect);

        transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);

        if (tempX > startPosX + lengthX)
        {
            startPosX += lengthX;
        } else if (tempX < startPosX - lengthX)
        {
            startPosX -= lengthX;
        }

        if (tempY > startPosY + lengthY)
        {
            startPosY += lengthY;
        } else if (tempY < startPosY - lengthY)
        {
            startPosY -= lengthY;
        }
    }
}
