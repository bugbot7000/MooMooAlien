using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAndSpin : MonoBehaviour
{
    public float hoverHeight = 0.1f;
    public float hoverSpeed = 1f;
    public float spinSpeed = 100f;
    public bool canHover, canSpin;
    private Vector3 startPos;
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (canHover)
        {
            Hover();
        }

        if (canSpin)
        {
            Spin();
        }
    }
    private void Hover()
    {
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        Vector3 hoverPos = startPos + new Vector3(0, hoverOffset, 0);
        transform.localPosition = hoverPos;
    }

    void Spin()
    {
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
    }
}
