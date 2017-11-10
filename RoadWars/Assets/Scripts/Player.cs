using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Car pCar;

    void Start()
    {
        pCar = CarFactory.GetPizza(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
