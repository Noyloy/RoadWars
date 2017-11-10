using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float Speed = 2f;

    private float turnSpeed = 3f;
    private Dir nextTurn = Dir.Up;
    private Animator anim;
    private TouchSwipeManager swipeManager = new TouchSwipeManager();
    private Rigidbody rb;
    private float turnRadious = 1.35f;

    Quaternion startRot, endRot;

    float temp;
    bool isRotating, turnPlayer;
    int horizontalDirection, verticalDirection;

    void Start ()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        startRot = Quaternion.LookRotation(transform.forward);
        endRot = Quaternion.LookRotation(transform.forward);
    }

    void Update()
    {
        nextTurn = swipeManager.DetectSwipe() ?? nextTurn;
        nextTurn = getTurnFromAxis();
        rb.velocity = transform.forward * Speed;
    }



    IEnumerator RotateMe(Vector3 byAngles, float inTime)
    {
        isRotating = true;
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (float t = 0; t<=1.15f ; t += Time.fixedDeltaTime / inTime)
        {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
        isRotating = false;
        nextTurn = Dir.Up;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (nextTurn == Dir.Left && !isRotating)
        {
            StartCoroutine(RotateMe(Vector3.up * -90, (Mathf.PI * turnRadious) / (2 * rb.velocity.magnitude)));
            anim.Play("TurnLeft");
        }
        if (nextTurn == Dir.Right && !isRotating)
        {
            StartCoroutine(RotateMe(Vector3.up * 90, (Mathf.PI * turnRadious) / (2 * rb.velocity.magnitude)));
            anim.Play("TurnRight");
        }
    }

    /// <summary>
    /// helper method for input from keyboard
    /// </summary>
    /// <returns>the direction to turn to</returns>
    private Dir getTurnFromAxis()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (x > 0) return Dir.Right;
        if (x < 0) return Dir.Left;
        if (y > 0) return Dir.Up;
        if (y < 0) return Dir.Down;
        return nextTurn;
    }
}

public enum Dir { Up, Right, Left, Down }

public class TouchSwipeManager
{
    public float MaxTime = 0.5f;
    public float MinSwipeDist = 0.2f;

    private float startTime;
    private Vector3 startPos;

    public Dir? DetectSwipe()
    {
        if (Input.touchCount == 0) return null;

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            startTime = Time.time;
            startPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            float endTime = Time.time;
            Vector3 endPos = touch.position;

            float swipeDist = (endPos - startPos).magnitude;
            float swipeTime = endTime - startTime;
            if (swipeTime < MaxTime && swipeDist > MinSwipeDist)
            {
                Vector2 swipeDir = endPos - startPos;
                return getDirection(swipeDir);
            }
        }
        return null;
    }

    private Dir getDirection(Vector2 swipeDir)
    {
        //Horizontal
        if (Mathf.Abs(swipeDir.x) > Mathf.Abs(swipeDir.y))
        {
            if (swipeDir.x > 0)
            {
                return Dir.Right;
            }
            else if (swipeDir.x < 0)
            {
                return Dir.Left;
            }
        }
        //Vertical
        else if (Mathf.Abs(swipeDir.x) < Mathf.Abs(swipeDir.y))
        {
            if (swipeDir.y > 0)
            {
                return Dir.Up;
            }
            else if (swipeDir.y < 0)
            {
                return Dir.Down;
            }
        }
        return Dir.Up;
    }
}