using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float Speed = 1.5f;

    private Dir nextTurn = Dir.Up;
    private TouchSwipeManager swipeManager = new TouchSwipeManager();

    void Start () {
    }
    
    void Update () {
        nextTurn = swipeManager.DetectSwipe();
        turnPlayer();

        transform.Translate(0f, 0f, Speed * Time.deltaTime);
	}

    void turnPlayer()
    {
        if (nextTurn == Dir.Left) transform.Rotate(0.0f, -90.0f, 0.0f);
        if (nextTurn == Dir.Right) transform.Rotate(0.0f, 90.0f, 0.0f);
        if (nextTurn == Dir.Down) transform.Rotate(0.0f, 180.0f, 0.0f);
        if (nextTurn == Dir.Up) transform.Rotate(0.0f, 0f, 0.0f);
        nextTurn = Dir.Up;
    }
}

public enum Dir { Up, Right, Left, Down }

public class TouchSwipeManager
{
    public float MaxTime = 0.5f;
    public float MinSwipeDist = 0.2f;

    private float startTime;
    private Vector3 startPos;

    public Dir DetectSwipe()
    {
        if (Input.touchCount == 0) return Dir.Up;

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
        return Dir.Up;
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