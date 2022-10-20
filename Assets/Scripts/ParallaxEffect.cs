using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxEffect;

    private Transform cameraPos;
    private Vector3 cameraLastPosition;

    // Start is called before the first frame update
    void Start()
    {
        cameraPos = Camera.main.transform;
        cameraLastPosition = cameraPos.position;
    }
    private void LateUpdate()
    {

        Vector3 backGroundMovment = cameraPos.position - cameraLastPosition;
        transform.position += new Vector3(backGroundMovment.x * parallaxEffect, backGroundMovment.y, 0);
        cameraLastPosition = cameraPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
