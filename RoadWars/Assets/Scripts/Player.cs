using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Dir nextTurn = Dir.Up;
    private Car pCar;
    private TouchSwipeManager swipeManager = new TouchSwipeManager();
    private Animator anim;

    void Start()
    {
        Debug.Log("starting player script");
        // get players car
        anim = GetComponent<Animator>();
        pCar = CarFactory.GetPizza(gameObject);
        pCar.Start();
    }
	
	void Update () {
        nextTurn = swipeManager.DetectSwipe() ?? nextTurn;
        nextTurn = getTurnFromAxis();

    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: handle collider tags - future enemies and oter
        if (nextTurn == Dir.Left && !pCar.IsTurning)
        {
            StartCoroutine(pCar.Turn(nextTurn));
            anim.Play("TurnLeft");
        }
        if (nextTurn == Dir.Right && !pCar.IsTurning)
        {
            StartCoroutine(pCar.Turn(nextTurn));
            //StartCoroutine(RotateMe(Vector3.up * 90, (Mathf.PI * turnRadious) / (2 * rb.velocity.magnitude)));
            anim.Play("TurnRight");
        }
        nextTurn = Dir.Up;
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
