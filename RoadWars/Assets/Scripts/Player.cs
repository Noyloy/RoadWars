using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Dir nextTurn = Dir.Up;
    private Car pCar;

    void Start()
    {
        // get players car
        pCar = CarFactory.GetPizza(gameObject);
    }
	
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //TODO: handle collider tags - future enemies and oter
        if (nextTurn == Dir.Left && !isRotating)
        {
            StartCoroutine(pCar.Turn(nextTurn));
            StartCoroutine(RotateMe(Vector3.up * -90, (Mathf.PI * turnRadious) / (2 * rb.velocity.magnitude)));
            anim.Play("TurnLeft");
        }
        if (nextTurn == Dir.Right && !isRotating)
        {
            StartCoroutine(RotateMe(Vector3.up * 90, (Mathf.PI * turnRadious) / (2 * rb.velocity.magnitude)));
            anim.Play("TurnRight");
        }
    }
}
