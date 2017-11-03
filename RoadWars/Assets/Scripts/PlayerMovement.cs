using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float Speed = 2f;

    private float speed { get; set; }
    private Dir nextTurn = Dir.Up;
    private Animator anim;
    private TouchSwipeManager swipeManager = new TouchSwipeManager();
    private Rigidbody rb;

    float temp;
    bool isRotating, turnPlayer;
    int horizontalDirection, verticalDirection;

    void Start ()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        speed = Speed;
    }

    void Update()
    {
        nextTurn = swipeManager.DetectSwipe() ?? nextTurn;
        nextTurn = getTurnFromAxis();

        // constant speed at turns - in order to solve the turn over/unde shoot.
        if (isRotating) speed = 2.4f; 
        else speed = Speed;

        performTurn();

        rb.velocity = transform.forward * speed;
    }

    private void performTurn()
    {
        if (nextTurn == Dir.Right && !isRotating && turnPlayer)
        {
            anim.Play("TurnRight");
            isRotating = true;
            horizontalDirection = 1;
            verticalDirection = 0;
            temp = 0;
            nextTurn = Dir.Up;
        }
        if (nextTurn == Dir.Left && !isRotating && turnPlayer)
        {
            anim.Play("TurnLeft");
            isRotating = true;
            horizontalDirection = -1;
            verticalDirection = 0;
            temp = 0;
            nextTurn = Dir.Up;
        }
        transform.Rotate(Vector3.up * 90 * Time.fixedDeltaTime * horizontalDirection, Space.World);
        temp += 90 * Time.fixedDeltaTime;
        if (temp >= 90)
        {
            temp = 0;
            horizontalDirection = 0;
            verticalDirection = 0;
            isRotating = false;
        }
        turnPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        turnPlayer = true;
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